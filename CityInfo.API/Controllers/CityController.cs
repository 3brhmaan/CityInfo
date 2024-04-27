using CityInfo.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[Route("api/Cities")]
[ApiController]
public class CityController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<CityDto>> GetCities()
    {
        return Ok(CityDataStore.Current.Cities);
    }

    [HttpGet("{id:int}")]
    public ActionResult<CityDto> GetCity(int id)
    {
        var city = CityDataStore.Current.Cities
            .FirstOrDefault(c => c.Id == id);

        if(city is null)
            return NotFound();
        else
            return Ok(city);
    }
}