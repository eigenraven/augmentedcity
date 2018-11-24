using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using backend.CityModel.Entities;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/city")]
    public class CityController : ControllerBase
    {
        private readonly CityModel.City _model;

        public CityController(IOptions<CityModel.City> model)
        {
            _model = model.Value;
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            return _model.Config.Name;
        }

        [HttpGet("stops")]
        public ActionResult<IEnumerable<Stop>> GetStops()
        {
            return _model.GtfsModel.Stops;
        }

        [HttpGet("routes")]
        public ActionResult<IEnumerable<Route>> GetRoutes()
        {
            return _model.GtfsModel.Routes;
        }
    }
}