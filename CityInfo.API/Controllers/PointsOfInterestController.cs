using CityInfo.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
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

    [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
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

    [HttpPost]
    public ActionResult<PointOfInterestDto> CreatePointOfInterest(
        int cityId,
        PointOfInterestForCreationDto pointOfInterest)
    {
        var city = CityDataStore.Current.Cities
            .FirstOrDefault(c => c.Id == cityId);

        if (city is null)
            return NotFound();

        var maxPointOfInterestId = city.PointsOfInterest
            .Max(p => p.Id);

        var finalPointOfInterest = new PointOfInterestDto
        {
            Name = pointOfInterest.Name,
            Description = pointOfInterest.Description,
            Id = maxPointOfInterestId + 1
        };

        city.PointsOfInterest.Add(finalPointOfInterest);

        return CreatedAtRoute(
            "GetPointOfInterest",
            new
            {
                cityId = cityId,
                pointOfInterestId = finalPointOfInterest.Id
            },
            finalPointOfInterest);
    }

    [HttpPut("{pointOfInterestId}")]
    public ActionResult UpdatePointOfInterest(
        int cityId,
        int pointOfInterestId,
        PointOfInterestForUpdateDto pointOfInterest)
    {
        var city = CityDataStore.Current.Cities
            .FirstOrDefault(c => c.Id == cityId);

        if (city is null)
            return NotFound();

        var pointOfInterestFromStore = city.PointsOfInterest
            .FirstOrDefault(p => p.Id == pointOfInterestId);

        if (pointOfInterestFromStore is null)
            return NotFound();

        pointOfInterestFromStore.Name = pointOfInterest.Name;
        pointOfInterestFromStore.Description = pointOfInterest.Description;

        return NoContent();
    }

    // check what happen when there is something wrong with the data sended 
    [HttpPatch("{pointOfInterestId}")]
    public ActionResult PartialUpdatePointOfInterest(
        int cityId,
        int pointOfInterestId,
        JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
    {
        var city = CityDataStore.Current.Cities
            .FirstOrDefault(c => c.Id == cityId);

        if (city is null)
            return NotFound();

        var pointOfInterestFromStore = city.PointsOfInterest
            .FirstOrDefault(p => p.Id == pointOfInterestId);

        if (pointOfInterestFromStore is null)
            return NotFound();

        var pointOfInterestToPatch =
            new PointOfInterestForUpdateDto
            {
                Name = pointOfInterestFromStore.Name,
                Description = pointOfInterestFromStore.Description
            };

        patchDocument.ApplyTo(pointOfInterestToPatch , ModelState);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
        pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

        if (!TryValidateModel(pointOfInterestToPatch))
            return BadRequest(ModelState);

        return NoContent();
    }

}

