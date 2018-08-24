using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    //when doing this rotuing at controller level 
    //there is no need to be specific in the method attributes
    public class CitiesController: Controller
    {
        private ICityInfoRepository _cityInfoRepo;

        public CitiesController(ICityInfoRepository cityInfoRepo)
        {
            _cityInfoRepo = cityInfoRepo;
        }
        //[HttpGet("api/cities")]
        [HttpGet()]
        public IActionResult GetCities()
        {
            //(this was initially used first )the below is for now not needed since the cities data store was created which is used to hold our city data
            //return new JsonResult(new List<object>()
            //{
            //    new {id=1, Name="New York City"},
            //    new {id=2, Name="Chicago" }
            //});

            //the below line was implemented second
            //return Ok(CitiesDataStore.Current.Cities);
            //a not found is not necessary because even an empty collection is a valid response

            var cityEntities = _cityInfoRepo.GetCities();
            var results = new List<CityWithoutPointsOfInterestDto>();

            //this method works by getting all the cities from the context then creating a newlist using the new dto and
            //creating the new items in a specific way as determined by the c.w.p.o.i.dto and then returns them
            foreach (var cityEntity in cityEntities)
            {
                results.Add(new CityWithoutPointsOfInterestDto
                {
                    Id = cityEntity.Id,
                    Description = cityEntity.Description,
                    Name = cityEntity.Name
                });
            }
            return Ok(results);
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id,bool includePointsOfInterest = false)
        {
            var city = _cityInfoRepo.GetCity(id, includePointsOfInterest);
            if(city == null)
            {
                return NotFound();
            }
            if (includePointsOfInterest)
            {
                var cityResult = new CityDto()
                {
                    Id = city.Id,
                    Name = city.Name,
                    Description = city.Description
                };

                foreach (var poi in city.PointsOfInterest)
                {
                    cityResult.PointsOfInterest.Add(
                        new PointOfInterestDto()
                        {
                            Id = poi.Id,
                            Name = poi.Name,
                            Description = poi.Description
                        });
                }
                return Ok(cityResult);
            }
            else
            {
                var cityWithOutPointOfInterestResult =
                    new CityWithoutPointsOfInterestDto()
                    {
                        Id = city.Id,
                        Description = city.Description,
                        Name = city.Name
                    };
                return Ok(cityWithOutPointOfInterestResult);
            }
            //---old cold that used inmemorydb-------------------------------
            //var cityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);
            //if (cityToReturn == null)
            //{
            //    return NotFound();
            //}

            //return Ok(cityToReturn);
        }

       
    }
}
