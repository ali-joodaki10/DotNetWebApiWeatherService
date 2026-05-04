using Microsoft.AspNetCore.Mvc;
using WeatherApi.Data;
using WeatherApi.Services;

namespace WeatherApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWeather(CancellationToken cancellationToken)
        {
            var result = await _weatherService.GetWeather(cancellationToken);

            if (result == null)
                return NoContent();

            return Ok(result);
        }
    }
}
