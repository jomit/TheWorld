using AutoMapper;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModel;

namespace TheWorld.Controllers.Api
{
    [Authorize]
    [Route("api/trips/{tripName}/stops")]
    public class StopController : Controller
    {
        private CoordService coordService;
        private ILogger<TripController> logger;
        private IWorldRepository repository;

        public StopController(IWorldRepository repo, ILogger<TripController> log, CoordService coordServ)
        {
            repository = repo;
            logger = log;
            coordService = coordServ;
        }

        [HttpGet("")]
        public JsonResult Get(string tripName)
        {
            try
            {
                var results = repository.GetTripByName(tripName,User.Identity.Name);
                if(results == null)
                {
                    return Json(null);
                }
                return Json(Mapper.Map<IEnumerable<StopViewModel>>(results.Stops.OrderBy(s => s.Order)));
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get stops for {tripName}", ex);
                Response.StatusCode = (int)HttpStatusCode.Created;
                return Json("Error Occured");

                throw;
            }
        }

        [HttpPost("")]
        public async Task<JsonResult> Post(string tripName, [FromBody] StopViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Map to entity
                    var newStop = Mapper.Map<Stop>(vm);

                    //Looking up Geocoordinates
                    var coordResult = await coordService.Lookup(newStop.Name);

                    if (!coordResult.Success)
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return Json(coordResult.Message);
                    }

                    newStop.Latitude = coordResult.Latitude;
                    newStop.Longitude = coordResult.Longitude;

                    //Save to the database
                    logger.LogInformation("Starting to Save a new Stop");

                    repository.AddStop(tripName,User.Identity.Name, newStop);

                    if (repository.SaveAll())
                    {
                        Response.StatusCode = (int)HttpStatusCode.Created;
                        return Json(Mapper.Map<StopViewModel>(newStop));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Failed to Save New Stop", ex);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = ex.Message });
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { Message = "Failed", ModelState = ModelState });
        }
    }
}
