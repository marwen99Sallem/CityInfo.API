using AutoMapper;
using CityInfo.Models;
using CityInfo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.Controllers;

[ApiController]
[Authorize(Policy= "MustBeFromAntwerp")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/cities")]
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
    public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities( 
        [FromQuery(Name ="filterOnName")] string? name, [FromQuery(Name = "search")] string? search, int pageNumber=1, int pageSize=1)//formquery attribute is uncesseray in this case since the routing is empty so it's automatically binded as a query variable
    {
        if(pageSize>maxCitiesPageSize)
            pageSize= maxCitiesPageSize;



       var ( cityEntities,paginationMetaData )=await _cityInfoRepository.GetCitiesAsync(name,search,pageSize,pageNumber);
        Response.Headers.Add("X-pagination",JsonSerializer.Serialize(paginationMetaData));

        return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));


    }
    /// <summary>
    /// Get a city by Id
    /// </summary>
    /// <param name="id">The id of the city to get </param>
    /// <param name="includePointsOfInterest"> wether to include a point of interest or not </param>
    /// <returns>IAction result </returns>
    /// <response code="200">Returns the requested city </response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest=false )
    {

        var cityToReturn = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);


        if (cityToReturn == null) return NotFound();
        if(includePointsOfInterest)
            return Ok(_mapper.Map<CityDto>(cityToReturn));
        return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(cityToReturn));

            
    }



}
