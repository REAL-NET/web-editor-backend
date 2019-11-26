using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {

        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpGet("hello")]
        public ActionResult<string> Hello()
        {
            return Ok("Hello");
        }
    }
}