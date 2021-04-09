using System.Threading.Tasks;
using Microservice.BackgroundJob.Messages;
using Microservice.BackgroundJob.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Microservice.BackgroundJob.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly KafkaProducer _bus;
        private readonly ILogger<TestController> _logger;

        public TestController(KafkaProducer bus, ILogger<TestController> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Send(ActualizarSaldo request)
        {
            await _bus.Send<ActualizarSaldo>(request);
            
            return Ok();
        }
    }
}
