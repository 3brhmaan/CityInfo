﻿using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CityInfo.API.Controllers;

[Route("api/Cities")]
[ApiController]
public class CityController : ControllerBase
{
    private readonly ICityInfoRepository _cityInfoRepository;
    private readonly IMapper _mapper;
    private int maxCitiesPageSize = 20;

    public CityController(
        ICityInfoRepository cityInfoRepository, 
        IMapper mapper)
    {
        _cityInfoRepository = cityInfoRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities(
        string? name,
        string? searchQuery ,
        int pageNumber = 1 ,
        int pageSize = 10)
    {
        pageSize = Math.Min(pageSize, maxCitiesPageSize);

        var (cityEntities , paginationMetadata) = await _cityInfoRepository
            .GetCitiesAsync(name , searchQuery , pageNumber , pageSize);

        Response.Headers.Add(
            "X-Pagination" ,
            JsonSerializer.Serialize(paginationMetadata)
            );

        var results = _mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities);

        return Ok(results);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCity(
        int id ,
        bool inculdePointsOfInterest = false)
    {
        var city = await _cityInfoRepository.GetCityAsync(id , inculdePointsOfInterest);

        if (city is null)
            return NotFound();

        if (inculdePointsOfInterest)
            return Ok(_mapper.Map<CityDto>(city));

        return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city));
    }
}