using System;
using System.Collections.Generic;
using System.Linq;
using Baltic.CalEditorRegistry.DTO;
using Baltic.CalEditorRegistry.Entities;
using Baltic.CalEditorRegistry.Model;
using Baltic.CalEditorRegistry.Tables;
using Baltic.CalEditorRegistry.Tables.CompartmentStyleTables;
using Baltic.CalEditorRegistry.Tables.CompartmentTables;
using Baltic.CalEditorRegistry.Tables.ElementStyleTables;
using Baltic.CalEditorRegistry.Tables.LineStyleTables;
using Baltic.CalEditorRegistry.Tables.LocationTables;
using Baltic.Core.Utils;
using Newtonsoft.Json;

namespace Baltic.CalEditorRegistry
{
    public class DiagramRepository
    {
        public string AddDiagram(DiagramDTO diagramDto)
        {
            var diagramTable = new DiagramTable();
            var diagramEntity = DBMapper.Map<DiagramEntity>(diagramDto, new DiagramEntity());
            
            diagramTable.Insert(new
            {
                DiagramUuid = diagramEntity.DiagramUuid,
                Name = diagramEntity.Name,
                Data = diagramEntity.Data
            });
            
            foreach (var box in diagramDto.Boxes)
            {
                box.DiagramId = diagramDto.Id;
                InsertBox(DBMapper.Map<BoxDTO>(box, new BoxDTO(){ElementUuid = Guid.Parse(box.Id)}));
            }

            foreach (var line in diagramDto.Lines)
            {
                line.DiagramId = diagramDto.Id;
                InsertLine(DBMapper.Map<LineDTO>(line, new LineDTO(){ElementUuid = Guid.Parse(line.Id)}));
            }

            foreach (var port in diagramDto.Ports)
            {
                port.DiagramId = diagramDto.Id;
                InsertPort(DBMapper.Map<PortDTO>(port, new PortDTO(){ElementUuid = Guid.Parse(port.Id)}));
            }

            return diagramDto.Id;
        }

        public string InsertLine(LineDTO lineDto)
        {
            var lineTable = new LineTable();
            var lineEntity = DBMapper.Map<LineEntity>(lineDto, new LineEntity());
            int diagramId;

            try
            {
                diagramId = new DiagramTable().Single(new { DiagramUuid = lineEntity.DiagramUuid })._Id;
            }
            catch (Exception)
            {
                throw new Exception("Could not create line, diagram with given id does not exist");
            }

            lineTable.Insert(new
            {
                _DiagramId = diagramId,
                ElementUuid = lineEntity.ElementUuid,
                DiagramUuid = lineEntity.DiagramUuid,
                ElementTypeId = lineEntity.ElementTypeId,
                Data = lineEntity.Data,
                StartElement = lineEntity.StartElement,
                EndElement = lineEntity.EndElement,
                Points = lineEntity.Points
            });

            var lineId = lineTable.Single(new { ElementUuid = lineEntity.ElementUuid })._Id;

            var compartmentTable = new LineCompartmentTable();
            var compartmentStyleTable = new LineCompartmentStyleTable();

            foreach (var compartment in lineDto.Compartments)
            {
                var compartmentDto = DBMapper.Map<CompartmentDTO>(compartment, new CompartmentDTO());
                compartmentDto.ElementId = lineDto.Id;
                InsertCompartment(compartmentDto, lineId, compartmentTable, compartmentStyleTable);
            }

            InsertLineStyle(lineDto.Style, lineId);

            return lineDto.Id;
        }

        public string InsertBox(BoxDTO boxDto)
        {
            var boxTable = new BoxTable();
            var boxEntity = DBMapper.Map<BoxEntity>(boxDto, new BoxEntity());
            int diagramId;

            try
            {
                diagramId = new DiagramTable().Single(new { DiagramUuid = boxEntity.DiagramUuid })._Id;
            }
            catch (Exception)
            {
                throw new Exception("Could not create box, diagram with given id does not exist");
            }

            boxTable.Insert(new
            {
                _DiagramId = diagramId,
                ElementUuid = boxEntity.ElementUuid,
                DiagramUuid = boxEntity.DiagramUuid,
                ElementTypeId = boxEntity.ElementTypeId,
                Data = boxEntity.Data
            });

            var boxId = boxTable.Single(new { ElementUuid = boxEntity.ElementUuid })._Id;

            var compartmentTable = new BoxCompartmentTable();
            var compartmentStyleTable = new BoxCompartmentStyleTable();

            foreach (var compartment in boxDto.Compartments)
            {
                var compartmentDto = DBMapper.Map<CompartmentDTO>(compartment, new CompartmentDTO());
                compartmentDto.ElementId = boxDto.Id;
                InsertCompartment(compartmentDto, boxId, compartmentTable, compartmentStyleTable);
            }

            InsertElementStyle(boxDto.Style.ElementStyle, boxId, new BoxElementStyleTable());
            InsertLocation(boxDto.Location, boxId, new BoxLocationTable());

            return boxDto.Id;
        }

        public string InsertPort(PortDTO portDto)
        {
            var portTable = new PortTable();
            var portEntity = DBMapper.Map<PortEntity>(portDto, new PortEntity());
            int diagramId;

            try
            {
                diagramId = new DiagramTable().Single(new { DiagramUuid = portEntity.DiagramUuid })._Id;
            }
            catch (Exception)
            {
                throw new Exception("Could not create port, diagram with given id does not exist");
            }

            portTable.Insert(new
            {
                _DiagramId = diagramId,
                ElementUuid = portEntity.ElementUuid,
                DiagramUuid = portEntity.DiagramUuid,
                ElementTypeId = portEntity.ElementTypeId,
                Data = portEntity.Data,
                ParentId = portEntity.ParentId
            });
            
            var portId = portTable.Single(new { ElementUuid = portEntity.ElementUuid })._Id;

            var compartmentTable = new PortCompartmentTable();
            var compartmentStyleTable = new PortCompartmentStyleTable();
           
            foreach (var compartment in portDto.Compartments)
            {
                var compartmentDto = DBMapper.Map<CompartmentDTO>(compartment, new CompartmentDTO());
                compartmentDto.ElementId = portDto.Id;
                InsertCompartment(compartmentDto, portId, compartmentTable, compartmentStyleTable);
            }

            InsertElementStyle(portDto.Style.ElementStyle, portId, new PortElementStyleTable());
            InsertLocation(portDto.Location, portId, new PortLocationTable());
            return portDto.Id;
        }

        private void InsertLocation(Location location, int elementId, dynamic locationTable)
        {
            if (location == null)
            {
                return;
            }

            var locationEntity = DBMapper.Map<LocationEntity>(location, new LocationEntity());
            locationEntity.ElementId = elementId;
            locationEntity.Id = elementId;
            locationTable.Insert(locationEntity);
        }

        private void InsertCompartment(CompartmentDTO compartmentDto, int elementId, dynamic compartmentTable, dynamic compartmentStyleTable)
        {
            var compartmentStyleEntity =
                DBMapper.Map<CompartmentStyleEntity>(compartmentDto.Style, new CompartmentStyleEntity());
            compartmentTable.Insert(new
            {
                _ElementId = elementId,
                CompartmentUuid = compartmentDto.Id,
                ElementUuid = compartmentDto.ElementUuid,
                Input = compartmentDto.Input,
                Value = compartmentDto.Value,
                CompartmentTypeId = compartmentDto.CompartmentTypeId
            });

            var compId = compartmentTable.Single(new { CompartmentUuid = compartmentDto.Id })._Id;

            compartmentStyleEntity.Id = compId;
            compartmentStyleEntity.CompartmentId = compId;
            compartmentStyleTable.Insert(compartmentStyleEntity);
        }

        private void InsertLineStyle(LineStyle lineStyle, int lineId)
        {
            if (lineStyle == null)
            {
                return;
            }

            var lineStyleTable = new LineStyleTable();
            var lineEndStyleTable = new LineEndStyleEndShapeTable();
            var lineStartStyleTable = new LineEndStyleStartShapeTable();

            var lineStartEntity = DBMapper.Map<LineEndStyleEntity>(lineStyle.StartShapeStyle, new LineEndStyleEntity());
            var lineEndEntity = DBMapper.Map<LineEndStyleEntity>(lineStyle.EndShapeStyle, new LineEndStyleEntity());

            lineStartEntity.LineId = lineId;
            lineStartEntity.Id = lineId;
            lineEndEntity.LineId = lineId;
            lineEndEntity.Id = lineId;

            lineStyleTable.Insert(new { Id = lineId, LineId = lineId, LineType = lineStyle.LineType });
            lineStartStyleTable.Insert(lineStartEntity);
            lineEndStyleTable.Insert(lineEndEntity);

            InsertElementStyle(lineStyle.ElementStyle, lineId, new LineElementStyleTable());
        }

        private void InsertElementStyle(ElementStyle elementStyle, int elementId, dynamic styleTable)
        {
            if (elementStyle == null)
            {
                return;
            }

            var elementStyleEntity = DBMapper.Map<ElementStyleEntity>(elementStyle, new ElementStyleEntity());
            elementStyleEntity.ElementId = elementId;
            elementStyleEntity.Id = elementId;
            styleTable.Insert(elementStyleEntity);
        }

        public bool DeleteDiagram(string id)
        {
            var diagramTable = new DiagramTable();
            var portTable = new PortTable();
            var lineTable = new PortTable();
            var boxTable = new BoxTable();

            var result = Guid.TryParse(id, out var uuid);

            if (!result)
            {
                return false;
            }

            var diagram = diagramTable.Single(new { DiagramUuid = uuid });
            
            if (diagram == null)
            {
                return false;
            }

            boxTable.All(new { _DiagramId = diagram._Id }).Select(x => x._Id).ToList()
                .ForEach(x => boxTable.Delete(new { _Id = x }));
            portTable.All(new { _DiagramId = diagram._Id }).Select(x => x._Id).ToList()
                .ForEach(x => portTable.Delete(new { _Id = x }));
            lineTable.All(new { _DiagramId = diagram._Id }).Select(x => x._Id).ToList()
                .ForEach(x => lineTable.Delete(new { _Id = x }));

            diagramTable.Delete(new { _Id = diagram._Id });

            return true;
        }

        public bool DeleteBox(string id)
        {
            var boxTable = new BoxTable();
            var result = Guid.TryParse(id, out var uuid);

            if (!result)
            {
                return false;
            }

            var boxToDelete = boxTable.Single(new {ElementUuid = uuid});
           
            if (boxToDelete == null)
            {
                return false;
            }

            return boxTable.Delete(new { _Id = boxToDelete._Id }) != 0;
        }

        public bool DeletePort(string id)
        {
            var portTable = new PortTable();
            var result = Guid.TryParse(id, out var uuid);

            if (!result)
            {
                return false;
            }

            var portToDelete = portTable.Single(new {ElementUuid = uuid});

            if (portToDelete == null)
            {
                return false;
            }

            return portTable.Delete(new { _Id = portToDelete._Id }) != 0;
        }

        public bool DeleteLine(string id)
        {
            var lineTable = new LineTable();
            var result = Guid.TryParse(id, out var uuid);

            if (!result)
            {
                return false;
            }

            var lineToDelete = lineTable.Single(new {ElementUuid = uuid});

            if (lineToDelete == null)
            {
                return false;
            }

            return lineTable.Delete(new { lineToDelete._Id }) != 0;
        }

        public DiagramDTO GetDiagramById(string id)
        {
            var diagramTable = new DiagramTable();
            var diagramEntity = diagramTable.Single(new {DiagramUuid = Guid.Parse(id)});
            DiagramDTO diagramDto = null;

            if (diagramEntity != null)
            {
                diagramDto = new DiagramDTO();
                DBMapper.Map<DiagramDTO>(diagramEntity, diagramDto);
                diagramDto.Boxes = GetBoxes(diagramEntity._Id);
                diagramDto.Ports = GetPorts(diagramEntity._Id);
                diagramDto.Lines = GetLines(diagramEntity._Id);
            }

            return diagramDto;
        }

        private List<Box> GetBoxes(int diagramId)
        {
            var boxTable = new BoxTable();
            var boxEntities = boxTable.All(new { _DiagramId = diagramId }).ToList();
            var boxes = new List<Box>();
            boxEntities.ForEach(x => boxes.Add(GetBox(x)));

            return boxes;
        }

        private List<Port> GetPorts(int diagramId)
        {
            var portTable = new PortTable();
            var portEntities = portTable.All(new { _DiagramId = diagramId }).ToList();
            var ports = new List<Port>();
            portEntities.ForEach(x => ports.Add(GetPort(x)));

            return ports;
        }

        private List<Line> GetLines(int diagramId)
        {
            var lineTable = new LineTable();
            var lineEntities = lineTable.All(new { _DiagramId = diagramId }).ToList();
            var lines = new List<Line>();
            lineEntities.ForEach(x => lines.Add(GetLine(x)));

            return lines;
        }

        private Box GetBox(BoxEntity boxEntity)
        {
            var boxDto = new BoxDTO();

            if (boxEntity != null)
            {
                DBMapper.Map<BoxDTO>(boxEntity, boxDto);
                boxDto.Location = GetLocation(boxEntity._Id, new BoxLocationTable());
                boxDto.Style.ElementStyle = GetElementStyle(boxEntity._Id, new BoxElementStyleTable());
                boxDto.Compartments = GetCompartments(boxEntity._Id, new BoxCompartmentTable(),
                    new BoxCompartmentStyleTable());
            }

            return DBMapper.Map<Box>(boxDto, new Box());
        }

        private Port GetPort(PortEntity portEntity)
        {
            var portDto = new PortDTO();

            if (portEntity != null)
            {
                DBMapper.Map<PortDTO>(portEntity, portDto);
                portDto.Location = GetLocation(portEntity._Id, new PortLocationTable());
                portDto.Style.ElementStyle = GetElementStyle(portEntity._Id, new PortElementStyleTable());
                portDto.Compartments = GetCompartments(portEntity._Id, new PortCompartmentTable(),
                    new PortCompartmentStyleTable());
            }

            return DBMapper.Map<Port>(portDto, new Port());
        }

        private Line GetLine(LineEntity lineEntity)
        {
            var lineDto = new LineDTO();
            var lineEndStyleTable = new LineEndStyleEndShapeTable();
            var lineStartStyleTable = new LineEndStyleStartShapeTable();
            var lineStyleTable = new LineStyleTable();

            if (lineEntity != null)
            {
                DBMapper.Map<LineDTO>(lineEntity, lineDto);
                lineDto.Style = DBMapper.Map<LineStyle>(lineStyleTable.Single(new { LineId = lineEntity._Id }),
                    new LineStyle());
                lineDto.Style.ElementStyle = GetElementStyle(lineEntity._Id, new LineElementStyleTable());
                lineDto.Style.StartShapeStyle =
                    DBMapper.Map<LineEndStyle>(lineStartStyleTable.Single(new { LineId = lineEntity._Id }),
                        new LineEndStyle());
                lineDto.Style.EndShapeStyle = DBMapper.Map<LineEndStyle>(lineEndStyleTable.Single(new { LineId = lineEntity._Id }),
                    new LineEndStyle());
                lineDto.Compartments = GetCompartments(lineEntity._Id, new LineCompartmentTable(),
                    new LineCompartmentStyleTable());
            }

            return DBMapper.Map<Line>(lineDto, new Line());
        }

        private ElementStyle GetElementStyle(int id, dynamic elementStyleTable)
        {
            return DBMapper.Map<ElementStyle>(elementStyleTable.Single(new { ElementId = id }), new ElementStyle());
        }

        private Location GetLocation(int id, dynamic locationTable)
        {
            return DBMapper.Map<Location>(locationTable.Single(new { ElementId = id }), new Location());
        }

        private List<Compartment> GetCompartments(int id, dynamic compartmentTable, dynamic compartmentStyleTable)
        {
            var compartments = new List<CompartmentDTO>();
            var compartmentsDictionary = new Dictionary<int, CompartmentEntity>();

            var compartmentsEntities = (compartmentTable.All(new { _ElementId = id }));

            if (compartmentsEntities == null)
            {
                return null;
            }

            foreach (var compartmentsEntity in compartmentsEntities)
            {
                compartmentsDictionary.Add(compartmentsEntity._Id, compartmentsEntity);
            }

            foreach (var (compartmentId, compartmentEntity) in compartmentsDictionary)
            {
                var compartment = DBMapper.Map<CompartmentDTO>(compartmentEntity, new CompartmentDTO());
                compartment.Style = GetCompartmentStyle(compartmentId, compartmentStyleTable);
                compartments.Add(compartment);
            }

            return DBMapper.MapList<Compartment>(compartments);
        }

        private CompartmentStyle GetCompartmentStyle(int id, dynamic compartmentStyleTable)
        {
            return DBMapper.Map<CompartmentStyle>(compartmentStyleTable.Single(new { CompartmentId = id }),
                new CompartmentStyle());
        }

        public bool UpdateDiagram(string id, dynamic delta)
        {
            var diagramTable = new DiagramTable();
            var diagramEntity = diagramTable.Single(new { DiagramUuid = Guid.Parse(id) });

            if (diagramEntity == null)
            {
                return false;
            }

            DBMapper.MapWithDelta<DiagramEntity>(JsonConvert.DeserializeObject<DiagramEntity>(delta.ToString()), diagramEntity,
                delta.ToString());

            diagramTable.Update(diagramEntity);

            var dynamicDelta = JsonConvert.DeserializeObject(delta.ToString());

            if (dynamicDelta?.boxes != null)
            {
                foreach (var box in dynamicDelta.boxes)
                {
                    UpdateBox(box.id, box);
                }
            }

            if (dynamicDelta?.lines != null)
            {
                foreach (var line in dynamicDelta.lines)
                {
                    UpdateBox(line.id, line);
                }
            }

            if (dynamicDelta?.ports != null)
            {
                foreach (var port in dynamicDelta.port)
                {
                    UpdateBox(port.id, port);
                }
            }

            return true;
        }

        public bool UpdateBox(string id, dynamic delta)
        {
            var boxTable = new BoxTable();
            var boxEntity = boxTable.Single(new { ElementUuid = Guid.Parse(id) });

            if (boxEntity == null)
            {
                return false;
            }

            DBMapper.MapWithDelta<BoxEntity>(JsonConvert.DeserializeObject<BoxEntity>(delta.ToString()), boxEntity,
                delta.ToString());
            boxTable.Update(boxEntity);

            var dynamicDelta = JsonConvert.DeserializeObject(delta.ToString());

            if (dynamicDelta?.compartments != null)
            {
                var compartmentTable = new BoxCompartmentTable();
                foreach (var compartment in dynamicDelta.compartments)
                {
                    UpdateCompartment(compartment.Id, compartment, compartmentTable);
                }
            }

            if (dynamicDelta?.style?.elementStyle != null)
            {
                UpdateElementStyle(boxEntity._Id, dynamicDelta.style.elementStyle.ToString(), new BoxElementStyleTable());
            }

            if (dynamicDelta?.location != null)
            {
                UpdateLocation(boxEntity._Id, dynamicDelta.location.ToString(), new BoxLocationTable());
            }

            return true;
        }

        public bool UpdateLine(string id, dynamic delta)
        {
            var lineTable = new LineTable();
            var lineEntity = lineTable.Single(new { ElementUuid = Guid.Parse(id) });

            if (lineEntity == null)
            {
                return false;
            }

            DBMapper.MapWithDelta<LineEntity>(JsonConvert.DeserializeObject<LineEntity>(delta.ToString()), lineEntity,
                delta.ToString());
            lineTable.Update(lineEntity);

            var dynamicDelta = JsonConvert.DeserializeObject(delta.ToString());

            lineTable.Update(lineEntity);

            if (dynamicDelta?.compartments != null)
            {
                var compartmentTable = new LineCompartmentTable();
                foreach (var compartment in dynamicDelta.compartments)
                {
                    UpdateCompartment(compartment.Id, compartment, compartmentTable);
                }
            }

            if (dynamicDelta?.style != null)
            {
                UpdateLineStyle(lineEntity._Id, dynamicDelta.style);
            }

            return true;
        }

        public bool UpdatePort(string id, dynamic delta)
        {
            var portTable = new PortTable();
            var portEntity = portTable.Single(new { ElementUuid = Guid.Parse(id) });

            if (portEntity == null)
            {
                return false;
            }

            DBMapper.MapWithDelta<PortEntity>(JsonConvert.DeserializeObject<PortEntity>(delta.ToString()), portEntity,
                delta.ToString());
            portTable.Update(portEntity);

            portTable.Update(portEntity);

            var dynamicDelta = JsonConvert.DeserializeObject(delta.ToString());

            if (dynamicDelta?.compartments != null)
            {
                var compartmentTable = new PortCompartmentTable();
                foreach (var compartment in dynamicDelta.compartments)
                {
                    UpdateCompartment(compartment.Id, compartment.ToString(), compartmentTable);
                }
            }

            if (dynamicDelta?.style?.elementStyle != null)
            {
                UpdateElementStyle(portEntity._Id, dynamicDelta.style.elementStyle.ToString(), new PortElementStyleTable());
            }

            if (dynamicDelta?.location != null)
            {
                UpdateLocation(portEntity._Id, dynamicDelta.location.ToString(), new PortLocationTable());
            }

            return true;
        }

        public bool UpdateCompartment(string id, dynamic delta, dynamic compartmentTable)
        {
            if (compartmentTable == null)
            {
                if (new BoxCompartmentTable().Single(new { CompartmentUuid = id }) != null)
                {
                    compartmentTable = new BoxCompartmentTable();
                }
                else if (new PortCompartmentTable().Single(new { CompartmentUuid = id }) != null)
                {
                    compartmentTable = new PortCompartmentTable();
                }
                else
                {
                    compartmentTable = new LineCompartmentTable();
                }
            }

            dynamic compartmentStyleTable = compartmentTable switch
            {
                BoxCompartmentTable _ => new BoxCompartmentStyleTable(),
                PortCompartmentTable _ => new PortCompartmentStyleTable(),
                LineCompartmentTable _ => new LineCompartmentStyleTable()
            };

            var compartmentEntity = compartmentTable.Single(new { CompartmentUuid = id });

            if (compartmentEntity == null)
            {
                return false;
            }

            var compartmentDelta = JsonConvert.DeserializeObject<CompartmentEntity>(delta.ToString());
            DBMapper.MapWithDelta<CompartmentEntity>(compartmentDelta, compartmentEntity, delta.ToString());

            compartmentTable.Update(compartmentEntity);
            var dynamicDelta = JsonConvert.DeserializeObject(delta.ToString());

            if (dynamicDelta?.style != null)
            {
                UpdateCompartmentStyle(compartmentEntity._Id, dynamicDelta.style.ToString(), compartmentStyleTable);
            }

            return true;
        }

        private void UpdateLocation(int id, string delta, dynamic locationTable)
        {
            var locationEntity = locationTable.Single(new { Id = id });

            if (locationEntity == null)
            {
                return;
            }

            var locationDelta = JsonConvert.DeserializeObject<LocationEntity>(delta);

            DBMapper.MapWithDelta<LocationEntity>(locationDelta, locationEntity, delta);
            locationTable.Update(locationEntity);
        }

        private void UpdateElementStyle(int id, string delta, dynamic styleTable)
        {
            var elementStyleEntity = styleTable.Single(new { Id = id });

            if (elementStyleEntity == null)
            {
                return;
            }

            var styleDelta = JsonConvert.DeserializeObject<ElementStyle>(delta);

            DBMapper.MapWithDelta<ElementStyleEntity>(styleDelta, elementStyleEntity, delta);
            styleTable.Update(elementStyleEntity);
        }

        private void UpdateLineStyle(int id, dynamic delta)
        {
            var lineStyleTable = new LineStyleTable();
            var startShapeTable = new LineEndStyleStartShapeTable();
            var endShapeTable = new LineEndStyleEndShapeTable();

            var dynamicDelta = JsonConvert.DeserializeObject(delta.ToString());

            var lineStyleEntity = lineStyleTable.Single(new { Id = id });
            var lineStyleDelta = JsonConvert.DeserializeObject<LineStyleEntity>(delta.ToString());

            DBMapper.MapWithDelta<LineStyleEntity>(lineStyleDelta, lineStyleEntity, delta.ToString());
            lineStyleTable.Update(lineStyleEntity);

            if (dynamicDelta?.startShapeStyle != null)
            {
                var startShapeEntity = startShapeTable.Single(new { Id = id });
                var startShapeDelta =
                    JsonConvert.DeserializeObject<LineEndStyleEntity>(delta.startShapeStyle.ToString());
                DBMapper.MapWithDelta<LineEndStyleEntity>(startShapeDelta, startShapeEntity,
                    delta.startShapeStyle.ToString());
                startShapeTable.Update(startShapeEntity);
            }

            if (dynamicDelta?.endShapeStyle != null)
            {
                var endShapeEntity = endShapeTable.Single(new { Id = id });
                var endShapeDelta =
                    JsonConvert.DeserializeObject<LineEndStyleEntity>(delta.endShapeStyle.ToString());
                DBMapper.MapWithDelta<LineEndStyleEntity>(endShapeDelta, endShapeEntity,
                    delta.endShapeStyle.ToString());
                endShapeTable.Update(endShapeEntity);
            }

            if (dynamicDelta?.elementStyle != null)
            {
                UpdateElementStyle(id, delta.elementStyle.ToString(), new LineElementStyleTable());
            }
        }

        private void UpdateCompartmentStyle(int id, string delta, dynamic compartmentStyleTable)
        {
            var compartmentStyleEntity = compartmentStyleTable.Single(new { Id = id });

            if (compartmentStyleEntity == null)
            {
                return;
            }

            var styleDelta = JsonConvert.DeserializeObject<CompartmentStyleEntity>(delta);

            DBMapper.MapWithDelta<CompartmentStyleEntity>(styleDelta, compartmentStyleEntity, delta);
            compartmentStyleTable.Update(compartmentStyleEntity);
        }
    }
}
