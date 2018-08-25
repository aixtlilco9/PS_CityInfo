using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {
        private ILogger<PointsOfInterestController> _logger;
        private IMailService _mailService;
        private ICityInfoRepository _cityInfoRepo;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger,
            IMailService mailService,
            ICityInfoRepository cityInfoRepo)
        {
            _logger = logger;
            //when constructor injection is not feasible we can request an instance from the container directly 
            //HttpContext.RequestServices.GetService()
            _mailService = mailService;
            _cityInfoRepo = cityInfoRepo;

        }

        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                //throw new Exception("exception test to test the catch block and logger");
                //old code for using in memory db
                //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
                //if (city == null)
                //{
                //    _logger.LogInformation($"City with id {cityId} was not found when accesing points of interest.");
                //    return NotFound();
                //}
                //return Ok(city.PointsOfInterest);

                if (!_cityInfoRepo.CityExists(cityId))
                {
                    _logger.LogInformation($"City with id {cityId} was not found when accessing points of interest.");
                    return NotFound();
                }
                var pointOfInterestForCity = _cityInfoRepo.GetPointsOfInterest(cityId);

                var pointsOfInterestforCityResults = new List<PointOfInterestDto>();
                foreach (var poi in pointOfInterestForCity)
                {
                    pointsOfInterestforCityResults.Add(new PointOfInterestDto()
                    {
                        Id= poi.Id,
                        Name = poi.Name,
                        Description = poi.Description
                    });
                }
                return Ok(pointsOfInterestforCityResults);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}.", ex);
                return StatusCode(500,"A problem happend while handling your request");
            }           
        }

        [HttpGet("{cityId}/pointsofinterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            ////first we are checking if the city exist if so them if that particular point exist
            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            //if (city == null)
            //{
            //    return NotFound();
            //}

            //var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

            //if (pointOfInterest == null)
            //{
            //    return NotFound();
            //}
            //return Ok(pointOfInterest);
            if (!_cityInfoRepo.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = _cityInfoRepo.GetPointOfInterest(cityId, id);
            if (pointOfInterest == null)
            {
                return NotFound();
            }
            var pointOfInterestResult = new PointOfInterestDto()
            {
                Id = pointOfInterest.Id,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            return Ok(pointOfInterestResult);
        }

        [HttpPost("{cityId}/pointsofinterest")]
        public IActionResult CreatePointOfInterest(int cityId,
            [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            //------basic validation below
            if (pointOfInterest == null)
            {
                return BadRequest();
            }

            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("descriptrion", "name should not be the same as description");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //------basic validation above

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            //demo purpose--to be improved

            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(
                c => c.PointsOfInterest).Max(p => p.Id);

            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            city.PointsOfInterest.Add(finalPointOfInterest);
            return CreatedAtRoute("GetPointOfInterest", new
            { cityId = cityId, id = finalPointOfInterest.Id }, finalPointOfInterest);
        }

        //for full updates we use HttpPut
        [HttpPut("{cityId}/pointsofinterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id,
            [FromBody] PointOfInterestForUpdateDto pointOfInterest)
        {
            //------basic validation below
            if (pointOfInterest == null)
            {//checks if the content body is null
                return BadRequest();
            }

            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("descriptrion", "name should not be the same as description");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //------basic validation above

            //check if the city exist
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            //check if the particular pointOfInterest exist
            var pointOfInterestToBeFullyUpdated = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            if (pointOfInterestToBeFullyUpdated == null)
            {
                return NotFound();
            }

            //http should fully update the resource meaning all values...if a value is not provided it should have a default value of what it was 
            pointOfInterestToBeFullyUpdated.Name = pointOfInterest.Name;
            pointOfInterestToBeFullyUpdated.Description = pointOfInterest.Description;

            //for HttpPuts return NoContent() is typically used but a 200 okay
            return NoContent();
        }

        [HttpPatch("{cityId}/pointsofinterest/{id}")]
        public IActionResult PartialUpdatePointOfInterest(int cityId, int id,
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            //----------basic validation below
            //check if the city exist
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            //check if the particular pointOfInterest exist
            var pointOfInterestToBeFullyUpdated = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            if (pointOfInterestToBeFullyUpdated == null)
            {
                return NotFound();
            }
            //----------basic validation above

            var pointOfInterestToPatch =
                new PointOfInterestForUpdateDto()
                {
                    Name = pointOfInterestToBeFullyUpdated.Name,
                    Description = pointOfInterestToBeFullyUpdated.Description
                };

            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pointOfInterestToPatch.Name == pointOfInterestToPatch.Description)
            {
                ModelState.AddModelError("descriptrion", "name should not be the same as description");
            }

            TryValidateModel(pointOfInterestToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            pointOfInterestToBeFullyUpdated.Name = pointOfInterestToPatch.Name;
            pointOfInterestToBeFullyUpdated.Description = pointOfInterestToPatch.Description;

            return NoContent();

        }

        [HttpDelete("{cityId}/pointsofinterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            // ----------basic validation below
            //check if the city exist
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            //check if the particular pointOfInterest exist
            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }
            //----------basic validation above

            city.PointsOfInterest.Remove(pointOfInterestFromStore);

            //implementation of using mail service below
            _mailService.Send("Point of interest deleted.",
                $"Point of Interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} was deleted");

            return NoContent();
        }
    }
}
