﻿using CityInfo.API.Entities;

namespace CityInfo.API.Services;

public interface ICityInfoRepository
{
    Task<IEnumerable<City>> GetCitiesAsync();
    Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery , int pageNumber , int pageSize);
    Task<City?> GetCityAsync(int CityId, bool inculdePointsOfInterest);
    Task<IEnumerable<PointOfInterest>> GetPointOfInterestForCityAsync(int cityId);
    Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId , int pointOfInterestId); Task<bool> CityExistsAsync(int cityId);
    Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);
    Task<bool> SaveChangesAsync();
    void DeletePointOfInterest(PointOfInterest pointOfInterest);
    Task<bool> CityNameMatchCityId(string? cityName , int cityId);
}