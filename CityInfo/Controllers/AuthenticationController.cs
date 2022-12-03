using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CityInfo.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        //this class won't be used outside this controller , so we can scope it to this namespace
        public class AuthenticationRequestBody
        {
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }

        private class CityInfoUser
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string City { get; set; }

            public CityInfoUser(int userId, string userName, string firstName, string lastName, string city)
            {
                UserId = userId;
                UserName = userName;
                FirstName = firstName;
                LastName = lastName;
                City = city;
            }   
        }

        [HttpPost("authenticate")]
        public  ActionResult<String> authenticate(AuthenticationRequestBody requestBody)
        {
            //step1 : validate the username/password
            var user = ValidateUserCredentials(requestBody.UserName,requestBody.Password);

             if (user == null)
                return Unauthorized();


            //step2: create a token 
            var securityKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"])
                    );
            var signingCredentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub",user.UserId.ToString())); // sub is a standardized key for the unique user identifier
            claimsForToken.Add(new Claim("given_name",user.UserName));
            claimsForToken.Add(new Claim("family_name",user.LastName));
            claimsForToken.Add(new Claim("city",user.City));

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials
                );

            var tokenToReturn=new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return  Ok(tokenToReturn);

        }


        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        private CityInfoUser ValidateUserCredentials(string? userName, string? password)
        {
            //values would normally come from the user db/table
            return new CityInfoUser(1,userName?? "","Kevin","Dox","Antwerp"); 
        }
    }
}
