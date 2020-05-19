using Microsoft.AspNetCore.Mvc;
using RepoAPI.Models;

namespace Generator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GeneratorController : ControllerBase
    {

        /// <summary>
        /// Generates artifacts from model with parameters 
        /// </summary>
        /// <param name="parameters">Model and parameters</param>
        /// <returns></returns>
        [HttpPost("")]
        public ActionResult AuthUser([FromBody] GenerationParameters parameters)
        {
            // todo: put here you generator request
            return Ok();
        }
    }
}