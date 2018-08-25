using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{//it appears ienemerables need to execute the query with .tolist where as 
    //the other methods use firstofdefault
    public class CityInfoRepository : ICityInfoRepository
    {
        private CityInfoContext _ctx;
        public CityInfoRepository(CityInfoContext ctx)
        {
            _ctx = ctx;
        }

        public void AddPointOfInterestForCity(int cityId, PointOfInterest pointOfInterest)
        {
            var city = GetCity(cityId, false);
            city.PointsOfInterest.Add(pointOfInterest);
        }

        public bool CityExists(int cityId)
        {
            return _ctx.Cities.Any(c => c.Id == cityId);
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _ctx.PointOfInterest.Remove(pointOfInterest);
        }

        public IEnumerable<City> GetCities()
        {
            //calling tolist ensure the quesry is executed, calling tolist means iteration has to happen
            return _ctx.Cities.OrderBy(c => c.Name).ToList();
        }

        public City GetCity(int cityId, bool includePointsOfInterest)
        {
            //calling .FirstOrDefault effectively executes the query
            //to my understanding we use include to join the cities table with the points of interest 
            //as in the initial in-memorydb when we did a get request for a city/cities we got both the city/cities along with the points of interest as they were together
            //but now they are in seperate tables
            if (includePointsOfInterest)
            {
                return _ctx.Cities.Include(c => c.PointsOfInterest).Where(c => c.Id == cityId).FirstOrDefault();
            }
            return _ctx.Cities.Where(c => c.Id == cityId).FirstOrDefault();

        }

        public PointOfInterest GetPointOfInterest(int cityId, int PointofInterestId)
        {
            return _ctx.PointOfInterest
                .Where(p => p.Id == PointofInterestId && p.CityId == cityId).FirstOrDefault();
        }

        public IEnumerable<PointOfInterest> GetPointsOfInterest(int cityId)
        {
            return _ctx.PointOfInterest
                            .Where(p => p.CityId == cityId).ToList();
        }

        public bool Save()
        {
            return (_ctx.SaveChanges() >= 0);
        }
    }
}
