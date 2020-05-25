using Generator.Services;
using Microsoft.AspNetCore.Mvc;
using RepoAPI.Models;
using RazorEngine.Templating;

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
        [HttpPost("generate")]
        public ActionResult AuthUser([FromBody] GenerationParameters parameters)
        {
            Model model = new RepoLoader().LoadModel(parameters.ModelName);
            
            var template = System.IO.File.ReadAllText("temp.cshtml");
            var result = RazorEngine.Engine
                 .Razor
                 .RunCompile(template,
                     "key",
                     null,
                     model);
             
            return Ok(result);
        }
    }
}