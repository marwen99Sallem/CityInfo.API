using CityInfo.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.Controllers;

[ApiController]
[Route("api/cities")]
public class CitiesController : ControllerBase
{
    readonly private CitiesDataStore _citiesDataStore;
    public CitiesController(CitiesDataStore citiesDataStore)
    {
        _citiesDataStore = citiesDataStore ??throw new ArgumentNullException(nameof(CitiesDataStore)) ;
    }
    [HttpGet()]
    public ActionResult<IEnumerable<CityDto>> GetCities()
    {

        return Ok(_citiesDataStore.Cities);
    }

    [HttpGet("{id}")]
    public ActionResult<CityDto> GetCity(int id)
    {

        CityDto cityToReturn = _citiesDataStore.Cities.FirstOrDefault(e => e.Id == id);
        if (cityToReturn == null) return NotFound();
        return Ok(cityToReturn);


    }



}
