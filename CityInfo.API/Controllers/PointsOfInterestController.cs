using AutoMapper;
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
                if (!_cityInfoRepo.CityExists(cityId))
                {
                    _logger.LogInformation($"City with id {cityId} was not found when accessing points of interest.");
                    return NotFound();
                }
                var pointOfInterestForCity = _cityInfoRepo.GetPointsOfInterest(cityId);

                var pointsOfInterestforCityResults = Mapper.Map<IEnumerable<PointOfInterestDto>>(pointOfInterestForCity);
                
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
            var pointOfInterestResult = Mapper.Map<PointOfInterestDto>(pointOfInterest);

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
            if(!_cityInfoRepo.CityExists(cityId))
            {
                return NotFound();
            }

            //demo purpose--to be improved

            var finalPointOfInterest = Mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            _cityInfoRepo.AddPointOfInterestForCity(cityId, finalPointOfInterest);

            if (!_cityInfoRepo.Save())
            {
                return StatusCode(500, "A problem happend while handeling your request.");
            }

            var createdPointOfInterestToReturn = Mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest", new
            { cityId = cityId, id = createdPointOfInterestToReturn.Id }, createdPointOfInterestToReturn);
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
            if (!_cityInfoRepo.CityExists(cityId))
            {
                return NotFound();
            }

            //check if the particular pointOfInterest exist
            var pointOfInterestEntity = _cityInfoRepo.GetPointOfInterest(cityId,id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            Mapper.Map(pointOfInterest,pointOfInterestEntity);

            if (!_cityInfoRepo.Save())
            {
                return StatusCode(500, "A problem happend while handling your request");
            }
        
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
            if (!_cityInfoRepo.CityExists(cityId))
            {
                return NotFound();
            }

            //check if the particular pointOfInterest exist
            var pointOfInterestEntity = _cityInfoRepo.GetPointOfInterest(cityId,id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }
            //----------basic validation above
            var pointOfInterestToPatch = Mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);
         
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

            Mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);

            if (!_cityInfoRepo.Save())
            {
                return StatusCode(500, "a problem happend while handling your request");
            }
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
