using CityInfo.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        //returns t/f if a city exist
        bool CityExists(int cityId);
        //returns all the cities
        IEnumerable<City> GetCities();

        //returns a single city
        City GetCity(int cityId, bool includePointsOfInterest);

        //returns the points of interest fora city
        IEnumerable<PointOfInterest> GetPointsOfInterest(int cityId);

        //returns a specific point of interest
        PointOfInterest GetPointOfInterest(int cityId, int PointofInterestId);

        void AddPointOfInterestForCity(int cityId, PointOfInterest pointOfInterest);

        bool Save();
    }
}
