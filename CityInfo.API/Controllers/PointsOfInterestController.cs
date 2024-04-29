using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[Route("api/Cities/{CityId}/PointsOfInterest")]
[ApiController]
public class PointsOfInterestController : ControllerBase
{
    private readonly ILogger<PointsOfInterestController> _logger;
    private readonly LocalMailService _mailService;

    public PointsOfInterestController(
        ILogger<PointsOfInterestController> logger , 
        LocalMailService mailService)
    {
        _logger = logger;
        _mailService = mailService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int CityId)
    {
        try
        {
            var city = CityDataStore.Current.Cities
                .FirstOrDefault(c => c.Id == CityId);

            if (city is null)
                return NotFound();

            return Ok(city.PointsOfInterest);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(
                $"Exception while getting points of interest for city with id {CityId}." ,
                ex
             );

            return StatusCode(500, "A problem happen while handling your request.");
        }

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


    [HttpDelete("{pointOfInterestId}")]
    public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
    {
        var city = CityDataStore.Current.Cities
            .FirstOrDefault(c => c.Id == cityId);

        if (city is null)
            return NotFound();

        var pointOfInterestFromStore = city.PointsOfInterest
            .FirstOrDefault(p => p.Id == pointOfInterestId);

        if (pointOfInterestFromStore is null)
            return NotFound();

        city.PointsOfInterest.Remove(pointOfInterestFromStore);

        _mailService.Send("Point of interest deleted." ,
            $"Point of interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} was deleted.");

        return NoContent();
    }


}

