using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZanyCash.Glue;
using ZanyStreams;
using static ZanyCash.Core.Types;

namespace ZanyCash.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IScopedServiceLocator serviceLocator;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IScopedServiceLocator serviceLocator)
        {
            _logger = logger;
            this.serviceLocator = serviceLocator;
        }

        [HttpGet]
        public void Get()
        {
            var core = serviceLocator.GetRequiredService<CoreAdapter>("123");
            core.AddOnetimeTransaction(new OnetimeTransaction("aa", DateTime.Now, "Wasgeht", 69.0));
        }
    }
}
