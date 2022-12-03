using CityInfo;
using CityInfo.DbContexts;
using CityInfo.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Serilog;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Reflection;
using Microsoft.OpenApi.Models;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/cityinfo.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();



var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();


// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
}
    ).AddNewtonsoftJson()
    .AddXmlDataContractSerializerFormatters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); // aspnetcore tool that exposes our apis and how to interact with them 
builder.Services.AddSwaggerGen(setupAction=>
{
    var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var fullXmlCommentFilePath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
    setupAction.IncludeXmlComments(fullXmlCommentFilePath);
    setupAction.AddSecurityDefinition("CityInfoApiBearerAuth", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.Http,
        Scheme="Bearer",
        Description="Input a valid token to access this api"
    });
    setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference=new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="CityInfoApiBearerAuth"
                }
            }

            ,new List<string>()
        }
    });
    ;

}); // registers services that are used from generation the specification
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
builder.Services.AddSingleton<CitiesDataStore>();
builder.Services.AddDbContext<CityInfoContext>(
    dbContextOptions => dbContextOptions.UseSqlite(
        builder.Configuration["ConnectionStrings:CityInfoDBConnectionString"])); //registered with scoped lifetime (one per request)
builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"]))
        };
    }
    );
builder.Services.AddAuthorization( options=>
{
    options.AddPolicy("MustBeFromAntwerp",policy=>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim("city", "Antwerp");
        }
        

        );
});

builder.Services.AddApiVersioning(setupAction=>
{
    setupAction.AssumeDefaultVersionWhenUnspecified = true;
    setupAction.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    setupAction.ReportApiVersions = true;
});

// Transient services will be instantiated everytime they're injected into another object ; works best for lightweight,stateless services
#if DEBUG //if we're in  a debug build
builder.Services.AddTransient<IMailService, LocalMailService>();
#else
builder.Services.AddTransient<IMailService,CloudMailService>();
#endif

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); //the middleware that generates the openapi specification
    app.UseSwaggerUI(); //adds the middleware that uses the specification to generate swagger ui documentation 
}

//request pipeline :

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
