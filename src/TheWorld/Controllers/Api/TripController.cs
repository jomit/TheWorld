using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using TheWorld.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TheWorld.Controllers.Api
{
    public class TripController : Controller
    {
        private IWorldRepository repository;

        public TripController(IWorldRepository repo)
        {
            repository = repo;
        }

        [HttpGet("api/trips")]
        public JsonResult Get()
        {
            var results = repository.GetAllTripsWtihStops();
            return Json(results);
        }
    }
}
