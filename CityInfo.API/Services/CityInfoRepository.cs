using CityInfo.API.DbContext;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CityInfo.API.Services;

public class CityInfoRepository : ICityInfoRepository
{
    private readonly CityInfoContext _context;

    public CityInfoRepository(CityInfoContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<City>> GetCitiesAsync()
    {
        var cities = await _context.Cities.ToListAsync();
        return cities;
    }

    public async Task<IEnumerable<City>> GetCitiesAsync(string? name, string? searchQuery)
    {
        if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(searchQuery)) 
            return await GetCitiesAsync();

        var collection = _context.Cities as IQueryable<City>;

        if (!string.IsNullOrWhiteSpace(name))
        {
            name = name.Trim();
            collection = collection.Where(c => c.Name == name);
        }

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            searchQuery = searchQuery.Trim();
            collection = collection
                .Where(a => a.Name.Contains(searchQuery)
                            || (a.Description != null && a.Description.Contains(searchQuery)));
        }

        return await collection.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<City?> GetCityAsync(int CityId, bool inculdePointsOfInterest)
    {
        if (inculdePointsOfInterest)
            return await _context.Cities.Include(c => c.PointsOfInterest)
                .FirstOrDefaultAsync(c => c.Id == CityId);

        return await _context.Cities
            .FirstOrDefaultAsync(c => c.Id == CityId);
    }

    public async Task<bool> CityExistsAsync(int cityId)
    {
        return await _context.Cities.AnyAsync(c => c.Id == cityId);
    }

    public async Task<IEnumerable<PointOfInterest>> GetPointOfInterestForCityAsync(int cityId)
    {
        return await _context.PointsOfInterests
            .Where(p => p.CityId == cityId)
            .ToListAsync();
    }

    public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
    {
        return await _context.PointsOfInterests
            .Where(p => p.CityId == cityId && p.Id == pointOfInterestId)
            .FirstOrDefaultAsync();
    }

    public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
    {
        var city = await GetCityAsync(cityId, false);

        if (city is not null) city.PointsOfInterest.Add(pointOfInterest);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() >= 0;
    }

    public void DeletePointOfInterest(PointOfInterest pointOfInterest)
    {
        _context.PointsOfInterests.Remove(pointOfInterest);
    }
}