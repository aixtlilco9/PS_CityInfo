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
        public IActionResult GetCities()
        {
            return Ok(CitiesDataStore.Current.Cities);
            //a not found is not necessary because even an empty collection is a valid response
             
            //the below is for now not needed since the cities data store was created which is used to hold our city data
            //return new JsonResult(new List<object>()
            //{
            //    new {id=1, Name="New York City"},
            //    new {id=2, Name="Chicago" }
            //});
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id)
        {
            var cityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);
            if (cityToReturn == null)
            {
                return NotFound();
            }

            return Ok(cityToReturn);
        }

       
    }
}
