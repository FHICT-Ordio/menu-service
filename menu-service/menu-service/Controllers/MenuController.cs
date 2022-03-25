using Microsoft.AspNetCore.Mvc;

namespace menu_service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly ILogger<MenuController> _logger;

        public MenuController(ILogger<MenuController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetMenu")]
        public IActionResult? GetMenu()
        {
            return null;
        }
    }
}