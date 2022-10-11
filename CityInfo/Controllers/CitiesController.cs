using Microsoft.AspNetCore.Mvc;

namespace CityInfo.Controllers;

[ApiController]
[Route("api/cities")]
public class CitiesController : ControllerBase
{

    [HttpGet()]
    public JsonResult GetCities()
    {
        return new JsonResult(
           CititesDataStore.Current.Cities
            ) ;
    }

    [HttpGet("{id}")]
    public JsonResult GetCity(int id)
    {
        return new JsonResult(
            CititesDataStore.Current.Cities.FirstOrDefault(e => e.Id == id)
            ) ;

    }



}
