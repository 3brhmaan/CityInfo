using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[Route("api/Cities")]
[ApiController]
public class CityController : ControllerBase
{
    private readonly ICityInfoRepository _cityInfoRepository;
    private readonly IMapper _mapper;

    public CityController(ICityInfoRepository cityInfoRepository, IMapper mapper)
    {
        _cityInfoRepository = cityInfoRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities()
    {
        var cityEntities = await _cityInfoRepository.GetCitiesAsync();

        var results = _mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities);

        return Ok(results);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCity(int id , bool inculdePointsOfInterest = false)
    {
        var city = await _cityInfoRepository.GetCityAsync(id , inculdePointsOfInterest);

        if (city is null)
            return NotFound();

        if (inculdePointsOfInterest)
            return Ok(_mapper.Map<CityDto>(city));

        return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city));
    }
}