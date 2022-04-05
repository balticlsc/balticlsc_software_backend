using Microsoft.AspNetCore.Mvc;

namespace Baltic.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]    
    public class StatusController
    {
        [HttpGet]
        public ActionResult<string> Index()
        {
            return GetType().FullName;
        }
    }
}