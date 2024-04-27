using CityInfo.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[Route("api/Cities/{CityId}/PointsOfInterest")]
[ApiController]
public class PointsOfInterestController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int CityId)
    {
        var city = CityDataStore.Current.Cities
            .FirstOrDefault(c => c.Id == CityId);

        if (city is null)
            return NotFound();

        return Ok(city.PointsOfInterest);
    }

    [HttpGet("{pointOfInterestId}")]
    public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
    {
        var city = CityDataStore.Current.Cities
            .FirstOrDefault(c => c.Id == cityId);

        if (city is null)
            return NotFound();

        var pointOfInterest = city.PointsOfInterest
            .FirstOrDefault(p => p.Id == pointOfInterestId);

        if (pointOfInterest is null)
            return NotFound();

        return Ok(pointOfInterest);
    }

}