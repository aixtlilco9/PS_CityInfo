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
        //[HttpGet("api/cities")]
        [HttpGet()]
        public JsonResult GetCities()
        {
            return new JsonResult(CitiesDataStore.Current.Cities);

            //the below is for now not needed since the cities data store was created which is used to hold our city data
            //return new JsonResult(new List<object>()
            //{
            //    new {id=1, Name="New York City"},
            //    new {id=2, Name="Chicago" }
            //});
        }

        [HttpGet("{id}")]
        public JsonResult GetCity(int id)
        {
            return new JsonResult(
            CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id)
            );
        }
    }
}
