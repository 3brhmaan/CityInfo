using CityInfo.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[Route("api/Cities")]
[ApiController]
public class CityController : ControllerBase
{
    private readonly CityDataStore _citiesDataStore;

    public CityController(CityDataStore citiesDataStore)
    {
        _citiesDataStore = citiesDataStore;
    }
    [HttpGet]
    public ActionResult<IEnumerable<CityDto>> GetCities()
    {
        return Ok(_citiesDataStore.Cities);
    }

    [HttpGet("{id:int}")]
    public ActionResult<CityDto> GetCity(int id)
    {
        var city = _citiesDataStore.Cities
            .FirstOrDefault(c => c.Id == id);

        if(city is null)
            return NotFound();
        else
            return Ok(city);
    }
}