using AutoMapper;
using CityInfo.Models;
using CityInfo.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.Controllers;

[ApiController]
[Route("api/cities")]
public class CitiesController : ControllerBase
{
    private readonly ICityInfoRepository _cityInfoRepository;
    private readonly IMapper _mapper;
    const int maxCitiesPageSize = 20;

    public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
    {
        _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
        _mapper = mapper;
    }

   
    [HttpGet()]
    public async Task<ActionResult<(IEnumerable<CityWithoutPointsOfInterestDto>,PaginationMetadata)>> GetCities(
        [FromQuery(Name ="filterOnName")] string? name, [FromQuery(Name = "search")] string? search, int pageNumber=1, int pageSize=1)//formquery attribute is uncesseray in this case since the routing is empty so it's automatically binded as a query variable
    {
        if(pageSize>maxCitiesPageSize)
            pageSize= maxCitiesPageSize;



       var ( cityEntities,paginationMetaData )=await _cityInfoRepository.GetCitiesAsync(name,search,pageSize,pageNumber);
        Response.Headers.Add("X-pagination",JsonSerializer.Serialize(paginationMetaData));

        return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));


    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest=false )
    {

        var cityToReturn = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);


        if (cityToReturn == null) return NotFound();
        if(includePointsOfInterest)
            return Ok(_mapper.Map<CityDto>(cityToReturn));
        return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(cityToReturn));

            
    }



}
