using CityInfo.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.Controllers;

[ApiController]
[Route("api/cities")]
public class CitiesController : ControllerBase
{

    [HttpGet()]
    public ActionResult<IEnumerable<CityDto>> GetCities()
    {

        return Ok(CitiesDataStore.Current.Cities);
    }

    [HttpGet("{id}")]
    public ActionResult<CityDto> GetCity(int id)
    {

        CityDto cityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(e => e.Id == id);
        if (cityToReturn == null) return NotFound();
        return Ok(cityToReturn);


    }



}
