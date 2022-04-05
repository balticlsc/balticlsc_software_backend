using System.Collections.Generic;
using System.Linq;
using Baltic.Core.Utils;
using Baltic.Database;
using Baltic.DataModel.Diagram;
using Mighty;
using Newtonsoft.Json;

namespace Baltic.DiagramRegistry
{
    public class DiagramRegistryRepository
    {
        private string _connectionString = DB.ConnectionString;

        private MightyOrm _diagramTable;
        private MightyOrm _boxTable;
        private MightyOrm _lineTable;
        private MightyOrm _portTable;
        private MightyOrm _boxCompartmentTable;
        private MightyOrm _portCompartmentTable;
        private MightyOrm _lineCompartmentTable;


        public DiagramRegistryRepository(string connectionString = null)
        {
            if (connectionString != null)
            {
                _connectionString = connectionString;
            }

            CreateTables();
        }

        private void CreateTables()
        {
            _diagramTable = new MightyOrm(_connectionString, "diagrams", "_Id");
            _boxTable = new MightyOrm(_connectionString, "boxes", "_Id");
            _lineTable = new MightyOrm(_connectionString, "lines", "_Id");
            _portTable = new MightyOrm(_connectionString, "ports", "_Id");
            _boxCompartmentTable = new MightyOrm(_connectionString, "box_compartments", "_Id");
            _portCompartmentTable = new MightyOrm(_connectionString, "port_compartments", "_Id");
            _lineCompartmentTable = new MightyOrm(_connectionString, "line_compartments", "_Id");
        }

        public CALDiagram GetDiagram(string Uid)
        {
            var diagram = _diagramTable.Single(new { DiagramUuid = Uid });
            var calDiagram = new CALDiagram()
            {
                Uid = diagram.diagramuuid,
                Name = diagram.name,
            };

            GetElements(calDiagram);

            return calDiagram;
        }

        private void GetElements(CALDiagram diagram)
        {
            GetBoxes(diagram);
            GetLines(diagram);
            GetPorts(diagram);
            MergeBoxWithPort(diagram);
            MergeLineWithConnectableElement(diagram);
        }

        private void GetBoxes(CALDiagram diagram)
        {
            var boxesToMap = _boxTable.All(new { DiagramUuid = diagram.Uid });

            diagram.Boxes.AddRange(boxesToMap.Select(x => new Box
            {
                Uid = x.elementuuid,
                Data = JsonConvert.DeserializeObject<Dictionary<string, string>>(x?.data == null ? "" : x.data),
                ElementType = x.elementtypeid,
                Compartments = GetCompartments(x.elementuuid, _boxCompartmentTable)
            }));
        }

        private void GetPorts(CALDiagram diagram)
        {
            var portsToMap = _portTable.All(new { DiagramUuid = diagram.Uid });

            diagram.Ports.AddRange(portsToMap.Select(x => new Port()
            {
                Uid = x.elementuuid,
                Data = JsonConvert.DeserializeObject<Dictionary<string, string>>(x?.data == null ? "" : x.data),
                ElementType = x.elementtypeid,
                Compartments = GetCompartments(x.elementuuid, _portCompartmentTable)
            }));
        }

        private void GetLines(CALDiagram diagram)
        {
            var linesToMap = _lineTable.All(new { DiagramUuid = diagram.Uid });

            diagram.Lines.AddRange(linesToMap.Select(x => new Line()
            {
                Uid = x.elementuuid,
                ElementType = x.elementtypeid,
                Compartments = GetCompartments(x.elementuuid, _lineCompartmentTable)
            }));
        }

        private List<Compartment> GetCompartments(string Uid, dynamic compartmentTable)
        {
            var compartments = new List<Compartment>();
            var compartmentsToMap = compartmentTable.All(new { ElementUuid = Uid });

            foreach (var compartment in compartmentsToMap)
            {
                compartments.Add(new Compartment
                {
                    Uid = compartment.compartmentuuid,
                    CompartmentType = compartment.compartmenttypeid,
                    Input = compartment.input,
                    Value = compartment.value
                });
            }

            return compartments;
        }

        private void MergeBoxWithPort(CALDiagram diagram)
        {
            var ports = _portTable.All(new { DiagramUuid = diagram.Uid });

            foreach (var portEntity in ports)
            {
                var box = diagram.Boxes.FirstOrDefault(x => x.Uid == portEntity.parentid);
                var port = diagram.Ports.FirstOrDefault(x => x.Uid == portEntity.elementuuid);
                if (box != null)
                {
                    box.Ports.Add(port);
                    if (port != null) port.Parent = box;
                }
            }
        }

        private void MergeLineWithConnectableElement(CALDiagram diagram)
        {
            var lines = _lineTable.All(new { DiagramUuid = diagram.Uid });

            foreach (var lineEntity in lines)
            {

                var startElement = GetConnectableElement(diagram, lineEntity.startelement);
                var endElement = GetConnectableElement(diagram, lineEntity.endelement);
                var line = diagram.Lines.FirstOrDefault(x => x.Uid == lineEntity.elementuuid);

                startElement.Outgoing = line;
                endElement.Incoming = line;

                if (line != null)
                {
                    line.StartElement = MapConnectableElement(startElement);
                    line.EndElement = MapConnectableElement(endElement);
                }
            }
        }

        private ConnectableElement MapConnectableElement(dynamic element)
        {
            return DBMapper.Map<ConnectableElement>(element, new ConnectableElement());
        }

        private dynamic GetConnectableElement(CALDiagram diagram, string elementId)
        {
            return diagram.Ports.FirstOrDefault(x => x.Uid == elementId) ?? diagram.Boxes.FirstOrDefault(x => x.Uid == elementId) as dynamic;
        }
    }
}
