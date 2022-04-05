using System.Linq;
using Baltic.CalEditorRegistry.Tables;
using Microsoft.AspNetCore.Mvc;

namespace Baltic.CalEditorRegistry.Controllers
{
    [ApiController]
    [Route("debug")]
    public class DebugController : Controller
    {
        [HttpGet]
        [Route("diagram")]
        public IActionResult GetDiagramsId()
        {
            var diagramTable = new DiagramTable();

            return Ok(diagramTable.All().Select(x => x.DiagramUuid).ToList());
        }
    }
}