using CityInfo.API.DbContext;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
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

        public async Task<City?> GetCityAsync(int CityId , bool inculdePointsOfInterest)
        {

            if (inculdePointsOfInterest)
            {
                return await _context.Cities.Include(c => c.PointsOfInterest)
                    .FirstOrDefaultAsync(c => c.Id == CityId);
            }

            return await _context.Cities
                .FirstOrDefaultAsync(c => c.Id == CityId);
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointOfInterestForCityAsync(int cityId)
        {
            return await _context.PointsOfInterests
                .Where(p => p.CityId == cityId )
                .ToListAsync();
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
        {
            return await _context.PointsOfInterests
                .Where(p => p.CityId == cityId && p.Id == pointOfInterestId)
                .FirstOrDefaultAsync();
        }
    }
}
