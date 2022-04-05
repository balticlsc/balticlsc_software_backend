using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using Baltic.CalEditorRegistry.DTO;
using Baltic.CalEditorRegistry.Model;
using Baltic.Web.Common;
using Baltic.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static Baltic.Core.Utils.DBMapper;
using Exception = System.Exception;

namespace Baltic.CalEditorRegistry.Controllers
{
    [Route("editor")]
    [ApiController]
    public class CalEditorController : BalticController
    {
        private DiagramRepository _diagramRepository = new DiagramRepository();

        public CalEditorController(IUserSessionRoutine userSession) : base(userSession)
        {
        }
        
        [HttpGet]
        [Route("diagram/{id}")]
        public IActionResult GetDiagram(string id)
        {
            try
            {
                DiagramDTO diagram = _diagramRepository.GetDiagramById(id);
                if (null == diagram)
                    return HandleError("Diagram does not exist",HttpStatusCode.BadRequest);
                var xDiagram = Map<Diagram>(diagram, new Diagram());
                return Ok(xDiagram);
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        [HttpPost]
        [Route("diagram")]
        public IActionResult CreateDiagram([FromBody] object value)
        {
            try
            {
                if (null == value)
                    return HandleError("Empty diagram definition", HttpStatusCode.BadRequest);
                var diagram = JsonConvert.DeserializeObject<Diagram>(value.ToString());
                var diagramId = _diagramRepository.AddDiagram(Map<DiagramDTO>(diagram, new DiagramDTO()));
                return Ok(diagramId,"ok");
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        [HttpPut]
        [Route("diagram/{id}")]
        public IActionResult UpdateDiagram([FromBody] object delta, string id)
        {
            try
            {
                var result = _diagramRepository.UpdateDiagram(id, delta);
                return result ? Ok() : HandleError("Diagram does not exist", HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        [HttpDelete]
        [Route("diagram/{id}")]
        public IActionResult DeleteDiagram(string id)
        {
            try
            {
                var result = _diagramRepository.DeleteDiagram(id);
                return result ? Ok() : HandleError("Diagram does not exist", HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        [HttpPost]
        [Route("box")]
        public IActionResult CreateBox([FromBody] object box)
        {
            try
            {
                if (null == box)
                    return HandleError("Empty box definition", HttpStatusCode.BadRequest);
                var boxModel = JsonConvert.DeserializeObject<Box>(box.ToString());
                var boxDto = Map<BoxDTO>(boxModel, new BoxDTO());
                var boxId = _diagramRepository.InsertBox(boxDto);
                return Ok(boxId,"ok");
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        [HttpPut]
        [Route("box/{id}")]
        public IActionResult UpdateBox([FromBody] object delta, string id)
        {
            try
            {
                if (null == delta)
                    return HandleError("Empty box delta definition", HttpStatusCode.BadRequest);
                var result = _diagramRepository.UpdateBox(id, delta);
                return result ? Ok() : HandleError("Box does not exist", HttpStatusCode.BadRequest);
            }
            catch(Exception e)
            {
                return HandleError(e);
            }
        }

        [HttpDelete]
        [Route("box/{id}")]
        public IActionResult DeleteBox(string id)
        {
            try
            {
                var result = _diagramRepository.DeleteBox(id);
                return result ? Ok() : HandleError("Box does not exist", HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        [HttpPost]
        [Route("line")]
        public IActionResult CreateLine([FromBody] object line)
        {
            try
            {
                if (null == line)
                    return HandleError("Empty line definition", HttpStatusCode.BadRequest);
                var lineModel = JsonConvert.DeserializeObject<Line>(line.ToString());
                var lineDto = Map<LineDTO>(lineModel, new LineDTO());
                var lineId = _diagramRepository.InsertLine(lineDto);
                return Ok(lineId,"ok");
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        [HttpPut]
        [Route("line/{id}")]
        public IActionResult UpdateLine([FromBody] object delta, string id)
        {
            try
            {
                var result = _diagramRepository.UpdateLine(id, delta);
                return result ? Ok() : HandleError("Line does not exist",HttpStatusCode.BadRequest);
            }
            catch(Exception e)
            {
                return HandleError(e);
            }
        }

        [HttpDelete]
        [Route("line/{id}")]
        public IActionResult DeleteLine(string id)
        {
            try
            {
                var result = _diagramRepository.DeleteLine(id);
                return result ? Ok() : HandleError("Line does not exist",HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        [HttpPost]
        [Route("port")]
        public IActionResult CreatePort([FromBody] object port)
        {
            try
            {
                if (null == port)
                    return HandleError("Empty port definition", HttpStatusCode.BadRequest);
                var portModel = JsonConvert.DeserializeObject<Port>(port.ToString());
                var portDto = Map<PortDTO>(portModel, new PortDTO());
                var portId = _diagramRepository.InsertPort(portDto);
                return Ok(portId,"ok");
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        [HttpPut]
        [Route("port/{id}")]
        public IActionResult UpdatePort([FromBody] object delta, string id)
        {
            try
            {
                var result = _diagramRepository.UpdatePort(id, delta);
                return result ? Ok() : HandleError("Line does not exist", HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        [HttpDelete]
        [Route("port/{id}")]
        public IActionResult DeletePort(string id)
        {
            try
            {
                var result = _diagramRepository.DeletePort(id);
                return result ? Ok() : HandleError("Line does not exist", HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        [HttpPut]
        [Route("compartment/{id}")]
        public IActionResult UpdateCompartment([FromBody] object delta, string id)
        {
            try
            {
                var result = _diagramRepository.UpdateCompartment(id, delta, null);
                return result ? Ok() : HandleError("Compartment does not exist",HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        [HttpPost]
        [Route("element")]
        public IActionResult CreateElement([FromBody] object elementsList)
        {
            if (null == elementsList)
                return HandleError("Empty list of elements", HttpStatusCode.BadRequest);
            
            var boxesUuidList = new List<string>();
            var linesUuidList = new List<string>();
            var portsUuidList = new List<string>();

            dynamic elements = JsonConvert.DeserializeObject(elementsList.ToString());

            foreach (var element in elements)
            {
                try
                {
                    switch ((int)element.type)
                    {
                        case (int)DiagramElementType.Box:
                            var boxModel = JsonConvert.DeserializeObject<Box>(element.ToString()) as Box;
                            boxesUuidList.Add(_diagramRepository.InsertBox(Map<BoxDTO>(boxModel, new BoxDTO())));

                            break;

                        case (int)DiagramElementType.Line:
                            var lineModel = JsonConvert.DeserializeObject<Line>(element.ToString()) as Line;
                            linesUuidList.Add(_diagramRepository.InsertLine(Map<LineDTO>(lineModel, new LineDTO())));

                            break;

                        case (int)DiagramElementType.Port:
                            var portModel = JsonConvert.DeserializeObject<Port>(element.ToString()) as Port;
                            portsUuidList.Add(_diagramRepository.InsertPort(Map<PortDTO>(portModel, new PortDTO())));

                            break;
                    }
                }
                catch (Exception e)
                {
                    return HandleError(e);
                }
            }

            return Ok(new
            {
                boxesId = boxesUuidList,
                linesId = linesUuidList,
                portsId = portsUuidList
            });
        }

        [HttpPut]
        [Route("element")]
        public IActionResult UpdateElement([FromBody] object elementsDeltaLists)
        {
            if (null == elementsDeltaLists)
                return HandleError("Empty list of element deltas", HttpStatusCode.BadRequest);
            
            dynamic boxes = null;
            dynamic lines = null;
            dynamic ports = null;

            try
            {
                dynamic elements = JsonConvert.DeserializeObject(elementsDeltaLists.ToString());

                if (elements.boxes != null)
                {
                    boxes = JsonConvert.DeserializeObject(elements.boxes.ToString());
                }

                if (elements.lines != null)
                {
                    lines = JsonConvert.DeserializeObject(elements.lines.ToString());
                }

                if (elements.ports != null)
                {
                    ports = JsonConvert.DeserializeObject(elements.ports.ToString());
                }
            }
            catch (Exception e)
            {
                return HandleError(e);
            }

            var error = false;
            var message = "Elements with given id do not exist: ";

            foreach (var box in boxes ?? Enumerable.Empty<dynamic>())
            {
                try
                {
                    _diagramRepository.UpdateBox(box.id.ToString(), box);
                }
                catch (Exception e)
                {
                    error = true;
                    message += box.id.ToString() + "\n";
                }
            }

            foreach (var line in lines ?? Enumerable.Empty<dynamic>())
            {
                try
                {
                    _diagramRepository.UpdateLine(line.id.ToString(), line);
                }
                catch (Exception e)
                {
                    error = true;
                    message += line.id.ToString() + "\n";
                }
            }

            foreach (var port in ports ?? Enumerable.Empty<dynamic>())
            {
                try
                {
                    _diagramRepository.UpdatePort(port.id.ToString(), port);
                }
                catch (Exception e)
                {
                    error = true;
                    message += port.id.ToString() + "\n";

                }
            }

            return error ? HandleError(message,HttpStatusCode.BadRequest) : Ok();
        }

        [HttpDelete]
        [Route("element/{id}")]
        public IActionResult DeleteElement(string id)
        {
            try
            {
                var result = _diagramRepository.DeleteBox(id) || _diagramRepository.DeleteLine(id) ||
                             _diagramRepository.DeletePort(id);
                return result ? Ok() : HandleError("Element does not exist", HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

    }
}