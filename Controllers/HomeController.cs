using Blog.Attibutes;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        [HttpGet("")]
        [ApiKey]
        public IActionResult GetStatus()
        {

            return Ok();
        }
    }
}
