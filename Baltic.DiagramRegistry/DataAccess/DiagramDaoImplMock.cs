using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Baltic.CalEditorRegistry;
using Baltic.CalEditorRegistry.DTO;
using Baltic.CalEditorRegistry.Model;
using Baltic.DataModel.Diagram;
using Baltic.Types.DataAccess;
using Box = Baltic.DataModel.Diagram.Box;
using Compartment = Baltic.DataModel.Diagram.Compartment;
using Line = Baltic.DataModel.Diagram.Line;
using Port = Baltic.DataModel.Diagram.Port;

namespace Baltic.DiagramRegistry.DataAccess {
	public class DiagramDaoImplMock : IDiagram
	{
		List<CALDiagram> _diag = new List<CALDiagram>();
		DiagramRepository _repo = new DiagramRepository();
		private static SemaphoreSlim semaphore = new SemaphoreSlim(1,1);

		public DiagramDaoImplMock()
		{
			semaphore.Wait();
			AddDiagramYAIP();
			AddDiagramSIP();
			DiagramDTO diagDto = _repo.GetDiagramById("d1234567-1234-1234-1234-1234567890ab");
			if (null == diagDto)
			{ 
				CALDiagram diag = _diag.Find(d => "d1234567-1234-1234-1234-1234567890ab" == d.Uid);
				diagDto = Diagram2DTO(diag);
				_repo.AddDiagram(diagDto);
			}
			diagDto = _repo.GetDiagramById("d2234567-1234-1234-1234-1234567890ab");
			if (null == diagDto)
			{ 
				CALDiagram diag = _diag.Find(d => "d2234567-1234-1234-1234-1234567890ab" == d.Uid);
				diagDto = Diagram2DTO(diag);
				_repo.AddDiagram(diagDto);
			}

			semaphore.Release();
		}


		public string CreateDiagram(){
			DiagramDTO diagram = new DiagramDTO()
			{
				DiagramUuid = Guid.NewGuid()
			};
			return _repo.AddDiagram(diagram);
		}

		/// 
		/// <param name="diagramUid"></param>
		public CALDiagram GetDiagram(string diagramUid){
			
			//return _diag.Find(d => diagramUid == d.Uid);

			DiagramDTO diagDto = _repo.GetDiagramById(diagramUid);
			if (null == diagDto || 0 == diagDto.Boxes.Count)
			{
				// find diagram in the "MOCK" repository if not found in the database
				// TODO change to "return null" and remove the "0 ==" part above
				CALDiagram mockDiag = _diag.Find(d => diagramUid == d.Uid);
				if (null != mockDiag || null == diagDto)
					return mockDiag;
			}

			return DTO2Diagram(diagDto);
		}

		/// 
		/// <param name="diagramUid"></param>
		public string CopyDiagram(string diagramUid){
			DiagramDTO diagram = _repo.GetDiagramById(diagramUid);
			if (null == diagram)
				return null;
			Dictionary<string,string> lineEndingMap = new Dictionary<string, string>();
			diagram.DiagramUuid = Guid.NewGuid();
			foreach (Baltic.CalEditorRegistry.Model.Box box in diagram.Boxes)
			{
				string oldGuid = box.Id;
				box.Id = Guid.NewGuid().ToString();
				if ("RequiredDataPin" == box.ElementTypeId || "ProvidedDataPin" == box.ElementTypeId)
					lineEndingMap.Add(oldGuid,box.Id);
				box.DiagramId = diagram.Id;
				foreach (Baltic.CalEditorRegistry.Model.Compartment cmpr in box.Compartments)
				{
					cmpr.Id = Guid.NewGuid().ToString();
					cmpr.ElementId = box.Id;
				}

				foreach (Baltic.CalEditorRegistry.Model.Port port in diagram.Ports.FindAll(child =>
					child.ParentId == oldGuid))
				{
					string oldPortGuid = port.Id;
					port.Id = Guid.NewGuid().ToString();
					lineEndingMap.Add(oldPortGuid,port.Id);
					port.DiagramId = diagram.Id;
					port.ParentId = box.Id;
					foreach (Baltic.CalEditorRegistry.Model.Compartment cmpr in port.Compartments)
					{
						cmpr.Id = Guid.NewGuid().ToString();
						cmpr.ElementId = port.Id;
					}
				}
			}

			foreach (Baltic.CalEditorRegistry.Model.Line line in diagram.Lines)
			{
				line.Id = Guid.NewGuid().ToString();
				line.DiagramId = diagram.Id;
				line.StartElement = lineEndingMap[line.StartElement];
				line.EndElement = lineEndingMap[line.EndElement];
				foreach (Baltic.CalEditorRegistry.Model.Compartment cmpr in line.Compartments)
				{
					cmpr.Id = Guid.NewGuid().ToString();
					cmpr.ElementId = line.Id;
				}
			}

			return _repo.AddDiagram(diagram);
		}

		private CALDiagram DTO2Diagram(DiagramDTO dto)
		{
			CALDiagram result = new CALDiagram(){
				Uid = dto.DiagramUuid.ToString(),
				Name = dto.Name,
				Boxes = new List<Box>(),
				Ports = new List<Port>(),
				Lines = new List<Line>()
			};
			
			foreach (Baltic.CalEditorRegistry.Model.Box b in dto.Boxes)
			{
				Box box = new Box()
				{
					Uid = b.Id,
					ElementType = b.ElementTypeId,
					Compartments = b.Compartments?.Select(MapCompartment).ToList(),
					Data = Data2Dictionary(b.Data),
					Ports = new List<Port>()
				};
				result.Boxes.Add(box);
				
				foreach (Baltic.CalEditorRegistry.Model.Port p in dto.Ports.FindAll(child => child.ParentId == b.Id))
				{
					Port port = new Port()
					{
						Uid = p.Id,
						ElementType = p.ElementTypeId,
						Compartments = p.Compartments?.Select(MapCompartment).ToList(),
						Data = Data2Dictionary(p.Data),
						Parent = box
					};
					box.Ports.Add(port);
					result.Ports.Add(port);
				}
			};

			List<ConnectableElement> elements = new List<ConnectableElement>();
			elements.AddRange(result.Boxes);
			elements.AddRange(result.Ports);
			foreach (Baltic.CalEditorRegistry.Model.Line l in dto.Lines)
			{
				ConnectableElement start = elements.Find(e => e.Uid == l.StartElement),
					end = elements.Find(e => e.Uid == l.EndElement);
				Line line = new Line()
				{
					Uid = l.Id,
					Compartments = l.Compartments?.Select(MapCompartment).ToList(),
					StartElement = start,
					EndElement = end,
					ElementType = l.ElementTypeId
				};
				start.Outgoing = line;
				end.Incoming = line;
				result.Lines.Add(line);
			}
			
			return result;
		}

		private Compartment MapCompartment(Baltic.CalEditorRegistry.Model.Compartment c)
		{
			return new Compartment()
			{
				Uid = c.Id,
				Input = c.Input,
				Value = c.Value,
				CompartmentType = c.CompartmentTypeId
			};
		}

		private Dictionary<string, string> Data2Dictionary(string data)
		{
			Dictionary<string,string> result = new Dictionary<string, string>();
			if (null == data)
				return result;
			MatchCollection mc = Regex.Matches(data, "\"([\\w-]*)\" *: *\"([\\w-]*)\"");
			foreach (Match match in mc) 
				result.Add(match.Groups[1].Value,match.Groups[2].Value);
			return result;
		}
		
		private DiagramDTO Diagram2DTO(CALDiagram diagram)
		{
			DiagramDTO result = new DiagramDTO()
			{
				Id = diagram.Uid,
				DiagramUuid = Guid.Parse(diagram.Uid),
				Name = diagram.Name,
				Boxes = new List<CalEditorRegistry.Model.Box>(),
				Ports = new List<CalEditorRegistry.Model.Port>(),
				Lines = new List<CalEditorRegistry.Model.Line>()
			};

			foreach (Box b in diagram.Boxes)
			{
				Baltic.CalEditorRegistry.Model.Box box = new CalEditorRegistry.Model.Box()
				{
					Id = b.Uid,
					Data = Dictionary2Data(b.Data),
					DiagramId = diagram.Uid,
					ElementTypeId = b.ElementType,
					Compartments = b.Compartments?.Select(UnmapCompartment).ToList()
				};
				result.Boxes.Add(box);
			}

			foreach (Port p in diagram.Ports)
			{
				Baltic.CalEditorRegistry.Model.Port port = new CalEditorRegistry.Model.Port()
				{
					Id = p.Uid,
					Data = Dictionary2Data(p.Data),
					DiagramId = diagram.Uid,
					ElementTypeId = p.ElementType,
					Compartments = p.Compartments?.Select(UnmapCompartment).ToList(),
					ParentId = p.Parent.Uid
				};
				result.Ports.Add(port);
			}
			
			foreach (Line l in diagram.Lines)
			{
				Baltic.CalEditorRegistry.Model.Line line = new CalEditorRegistry.Model.Line()
				{
					Id = l.Uid,
					Data = "",
					DiagramId = diagram.Uid,
					ElementTypeId = l.ElementType,
					Compartments = l.Compartments?.Select(UnmapCompartment).ToList(),
					StartElement = l.StartElement.Uid,
					EndElement = l.EndElement.Uid,
					Style = new LineStyle()
					{
						ElementStyle = new ElementStyle(),
						EndShapeStyle = new LineEndStyle(),
						StartShapeStyle = new LineEndStyle()
					}
				};
				result.Lines.Add(line);
			}
			
			return result;
		}
		
		private string Dictionary2Data(Dictionary<string, string> dict)
		{
			string result = "{ ";
			foreach (KeyValuePair<string, string> elem in dict)
				result += "\"" + elem.Key + "\": \"" + elem.Value + "\", ";
			
			return 2<result.Length?result.Substring(0,result.Length-2)+" }":"{}";
		}
		
		private Baltic.CalEditorRegistry.Model.Compartment UnmapCompartment(Compartment c)
		{
			return new Baltic.CalEditorRegistry.Model.Compartment()
			{
				Id = c.Uid,
				Input = c.Input,
				Value = c.Value,
				CompartmentTypeId = c.CompartmentType
			};
		}
		
		private void AddDiagramSIP(){
			CALDiagram diag = new CALDiagram(){
				Uid = "d2234567-1234-1234-1234-1234567890ab",
				Name = "SimpleImageProcessor",
				Boxes = new List<Box>(),
				Ports = new List<Port>(),
				Lines = new List<Line>()
			};
			_diag.Add(diag);
			
			// UnitCall boxes
			Box fs = new Box(){
				Uid = Guid.NewGuid().ToString(), // "ftp2mongodb_001",
				ElementType = "UnitCall",
				Compartments = new List<Compartment>(),
				Ports = new List<Port>()
			};
			fs.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallName",Value="step 1"});
			fs.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallBinding",Value="true"});
			fs.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallCUName",Value="ftp2mongodb"});
			fs.Data.Add("callable_unit_uid","ftp2mongodb_rel_001");
			diag.Boxes.Add(fs);
			
			Box ims = new Box(){
				Uid = Guid.NewGuid().ToString(), // "rgb2gray-mongo_001",
				ElementType = "UnitCall",
				Compartments = new List<Compartment>(),
				Ports = new List<Port>()
			};
			ims.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallName",Value="step 2"});
			ims.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallBinding",Value="true"});
			ims.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallCUName",Value="rgb2gray-mongo"});
			ims.Data.Add("callable_unit_uid","rgb2gray-mongo_rel_001");
			diag.Boxes.Add(ims);
			
			Box ip1 = new Box(){
				Uid = Guid.NewGuid().ToString(), // "mongodb2ft_001",
				ElementType = "UnitCall",
				Compartments = new List<Compartment>(),
				Ports = new List<Port>()
			};
			ip1.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallName",Value="step 3"});
			ip1.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallBinding",Value="true"});
			ip1.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallCUName",Value="mongodb2ftp"});
			ip1.Data.Add("callable_unit_uid","mongodb2ftp_rel_001");
			diag.Boxes.Add(ip1);

			// DeclaredPin boxes
			Box inpp = new Box(){
				Uid = "11234567-1234-1234-1234-1234567890ab", // "InputImpages",
				ElementType = "RequiredDataPin",
				Compartments = new List<Compartment>()
			};
			inpp.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="RequiredDataPinName",Value="InputImages"});
			inpp.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="RequiredDataPinMultiplicity",Value="false"});
			inpp.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="RequiredDataPinMandatory",Value="true"});
			inpp.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="RequiredDataPinDataType",Value="ftp"});
			inpp.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="RequiredDataPinAccessType",Value="FTP Folder Name"});
			diag.Boxes.Add(inpp);
			
			Box outp = new Box(){
				Uid = "10234567-1234-1234-1234-1234567890ab", // "OutputImages",
				ElementType = "ProvidedDataPin",
				Compartments = new List<Compartment>()
			};
			outp.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="ProvidedDataPinName",Value="OutputImages"});
			outp.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="ProvidedDataPinMultiplicity",Value="false"});
			outp.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="ProvidedDataPinDataType",Value="ftp"});
			outp.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="ProvidedDataPinAccessType",Value="FTP Server"});
			diag.Boxes.Add(outp);
			
			// ComputedDataPin ports
			Port port1 = new Port()
			{
				Uid = Guid.NewGuid().ToString(),
				Compartments = new List<Compartment>()
			};
			port1.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortName",Value="in_folder"});
			port1.Data.Add("declared_pin_uid","FTPDataReader01");
			port1.Parent = fs;
			fs.Ports.Add(port1);
			diag.Ports.Add(port1);
			
			Port port2 = new Port()
			{
				Uid = Guid.NewGuid().ToString(),
				Compartments = new List<Compartment>()
			};
			port2.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortName",Value="out_db"});
			port2.Data.Add("declared_pin_uid","MongoDBFileWriter01");
			port2.Parent = fs;
			fs.Ports.Add(port2);
			diag.Ports.Add(port2);
			
			// -----
			Port port3 = new Port()
			{
				Uid = Guid.NewGuid().ToString(),
				Compartments = new List<Compartment>()
			};
			port3.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortName",Value="in_images"});
			port3.Data.Add("declared_pin_uid","ImageReader01");
			port3.Parent = ims;
			ims.Ports.Add(port3);
			diag.Ports.Add(port3);
			
			Port port4 = new Port()
			{
				Uid = Guid.NewGuid().ToString(),
				Compartments = new List<Compartment>()
			};
			port4.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortName",Value="out_images"});
			port4.Data.Add("declared_pin_uid","ImageWriter01");
			port4.Parent = ims;
			ims.Ports.Add(port4);
			diag.Ports.Add(port4);
			
			// -----
			Port port7 = new Port()
			{
				Uid = Guid.NewGuid().ToString(),
				Compartments = new List<Compartment>()
			};
			port7.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortName",Value="db_in"});
			port7.Data.Add("declared_pin_uid","MongoDBFileReader01");
			port7.Parent = ip1;
			ip1.Ports.Add(port7);
			diag.Ports.Add(port7);
			
			Port portA = new Port()
			{
				Uid = Guid.NewGuid().ToString(),
				Compartments = new List<Compartment>()
			};
			portA.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortName",Value="folder_out"});
			portA.Data.Add("declared_pin_uid","FTPFileWriter01");
			portA.Parent = ip1;
			ip1.Ports.Add(portA);
			diag.Ports.Add(portA);
			
			// Lines
			Line line = new Line(){
				Uid = Guid.NewGuid().ToString(),
				StartElement = inpp, EndElement = port1
			};
			diag.Lines.Add(line);
			
			line = new Line(){
				Uid = Guid.NewGuid().ToString(),
				StartElement = port2, EndElement = port3
			};
			diag.Lines.Add(line);
			
			line = new Line(){
				Uid = Guid.NewGuid().ToString(),
				StartElement = port4, EndElement = port7
			};
			diag.Lines.Add(line);
			
			line = new Line(){
				Uid = Guid.NewGuid().ToString(),
				StartElement = portA, EndElement = outp
			};
			diag.Lines.Add(line);
			
		}
		
		private void AddDiagramYAIP(){
			CALDiagram diag = new CALDiagram(){
				Uid = "d1234567-1234-1234-1234-1234567890ab",
				Name = "YetAnotherImageProcessor",
				Boxes = new List<Box>(),
				Ports = new List<Port>(),
				Lines = new List<Line>()
			};
			_diag.Add(diag);
			
			// UnitCall boxes
			Box fs = new Box(){
				Uid = Guid.NewGuid().ToString(), // "fs_001",
				ElementType = "UnitCall",
				//Unit = _cmrs.Find(rel => "Frame Splitter" == rel.Unit.Name),
				Compartments = new List<Compartment>(),
				Ports = new List<Port>()
			};
			fs.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallName",Value="first splitter"});
			fs.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallBinding",Value="true"});
			fs.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallCUName",Value="fs001"});
			fs.Data.Add("callable_unit_uid","fs001");
			diag.Boxes.Add(fs);
			
			Box ims = new Box(){
				Uid = Guid.NewGuid().ToString(), // "is_001",
				ElementType = "UnitCall",
				//Unit = _cmrs.Find(rel => "Image Splitter" == rel.Unit.Name),
				Compartments = new List<Compartment>(),
				Ports = new List<Port>()
			};
			ims.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallName",Value="second splitter"});
			ims.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallBinding",Value="true"});
			ims.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallCUName",Value="is001"});
			ims.Data.Add("callable_unit_uid","is001");
			diag.Boxes.Add(ims);
			
			Box ip1 = new Box(){
				Uid = Guid.NewGuid().ToString(), // "ip_001",
				ElementType = "UnitCall",
				//Unit = _cmrs.Find(rel => "Image Processor" == rel.Unit.Name),
				Compartments = new List<Compartment>(),
				Ports = new List<Port>()
			};
			ip1.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallName",Value="processor1"});
			ip1.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallBinding",Value="true"});
			ip1.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallCUName",Value="ip002"});
			ip1.Data.Add("callable_unit_uid","ip002");
			diag.Boxes.Add(ip1);
			
			Box ip2 = new Box(){
				Uid = Guid.NewGuid().ToString(), // "ip_002",
				ElementType = "UnitCall",
				//Unit = _cmrs.Find(rel => "Image Processor" == rel.Unit.Name),
				Compartments = new List<Compartment>(),
				Ports = new List<Port>()
			};
			ip2.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallName",Value="processor2"});
			ip2.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallBinding",Value="true"});
			ip2.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallCUName",Value="ip002"});
			ip2.Data.Add("callable_unit_uid","ip002");
			diag.Boxes.Add(ip2);
			
			Box ip3 = new Box(){
				Uid = Guid.NewGuid().ToString(), // "ip_003",
				ElementType = "UnitCall",
				//Unit = _cmrs.Find(rel => "Image Processor" == rel.Unit.Name),
				Compartments = new List<Compartment>(),
				Ports = new List<Port>()
			};
			ip3.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallName",Value="processor3"});
			ip3.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallBinding",Value="true"});
			ip3.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallCUName",Value="ip002"});
			ip3.Data.Add("callable_unit_uid","ip002");
			diag.Boxes.Add(ip3);
			
			Box im = new Box(){
				Uid = Guid.NewGuid().ToString(), // "im_001",
				ElementType = "UnitCall",
				//Unit = _cmrs.Find(rel => "Image Merger" == rel.Unit.Name),
				Compartments = new List<Compartment>(),
				Ports = new List<Port>()
			};
			im.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallName",Value="merger"});
			im.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallBinding",Value="true"});
			im.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="UnitCallCUName",Value="im003"});
			im.Data.Add("callable_unit_uid","im003");
			diag.Boxes.Add(im);
			
			// DeclaredPin boxes
			Box inpp = new Box(){
				Uid = "d6234567-1234-1234-1234-1234567890ab", // "Film01",
				ElementType = "RequiredDataPin",
				Compartments = new List<Compartment>()
			};
			inpp.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="RequiredDataPinName",Value="Film"});
			inpp.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="RequiredDataPinMultiplicity",Value="false"});
			inpp.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="RequiredDataPinMandatory",Value="true"});
			inpp.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="RequiredDataPinDataType",Value="mpeg"});
			inpp.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="RequiredDataPinAccessType",Value="file"});
			diag.Boxes.Add(inpp);
			
			Box outp = new Box(){
				Uid = "06234567-1234-1234-1234-1234567890ab", // "Proc_Film01",
				ElementType = "ProvidedDataPin",
				Compartments = new List<Compartment>()
			};
			outp.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="ProvidedDataPinName",Value="Proc_Film"});
			outp.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="ProvidedDataPinMultiplicity",Value="false"});
			outp.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="ProvidedDataPinDataType",Value="mpeg"});
			outp.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="ProvidedDataPinAccessType",Value="file"});
			diag.Boxes.Add(outp);
			
			// ComputedDataPin ports
			Port port1 = new Port()
			{
				Uid = Guid.NewGuid().ToString(), // "film_001",
				Compartments = new List<Compartment>()
			};
			port1.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortName",Value="film"});
			port1.Data.Add("declared_pin_uid","film01");
			port1.Parent = fs;
			fs.Ports.Add(port1);
			diag.Ports.Add(port1);
			
			Port port2 = new Port()
			{
				Uid = Guid.NewGuid().ToString(), // "filmf1_001",
				Compartments = new List<Compartment>()
			};
			port2.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortName",Value="filmf1"});
			port2.Data.Add("declared_pin_uid","filmf01");
			port2.Parent = fs;
			fs.Ports.Add(port2);
			diag.Ports.Add(port2);
			
			// -----
			Port port3 = new Port()
			{
				Uid = Guid.NewGuid().ToString(), // "image_001",
				Compartments = new List<Compartment>()
			};
			port3.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortName",Value="image"});
			port3.Data.Add("declared_pin_uid","image01");
			port3.Parent = ims;
			ims.Ports.Add(port3);
			diag.Ports.Add(port3);
			
			Port port4 = new Port()
			{
				Uid = Guid.NewGuid().ToString(), // "imagep1_001",
				Compartments = new List<Compartment>()
			};
			port4.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortName",Value="imagep1"});
			port4.Data.Add("declared_pin_uid","imagep01");
			port4.Parent = ims;
			ims.Ports.Add(port4);
			diag.Ports.Add(port4);
			
			Port port5 = new Port()
			{
				Uid = Guid.NewGuid().ToString(), // "imagep2_001",
				Compartments = new List<Compartment>()
			};
			port5.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortName",Value="imagep2"});
			port5.Data.Add("declared_pin_uid","imagep02");
			port5.Parent = ims;
			ims.Ports.Add(port5);
			diag.Ports.Add(port5);
			
			Port port6 = new Port()
			{
				Uid = Guid.NewGuid().ToString(), // "imagep3_001",
				Compartments = new List<Compartment>()
			};
			port6.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortName",Value="imagep3"});
			port6.Data.Add("declared_pin_uid","imagep03");
			port6.Parent = ims;
			ims.Ports.Add(port6);
			diag.Ports.Add(port6);
			
			// -----
			Port port7 = new Port()
			{
				Uid = Guid.NewGuid().ToString(), // "imagep_001",
				Compartments = new List<Compartment>()
			};
			port7.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortName",Value="imagep"});
			port7.Data.Add("declared_pin_uid","imagep00");
			port7.Parent = ip1;
			ip1.Ports.Add(port7);
			diag.Ports.Add(port7);
			
			Port port8 = new Port()
			{
				Uid = Guid.NewGuid().ToString(), // "imagep_002",
				Compartments = new List<Compartment>()
			};
			port8.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortName",Value="imagep"});
			port8.Data.Add("declared_pin_uid","imagep00");
			port8.Parent = ip2;
			ip2.Ports.Add(port8);
			diag.Ports.Add(port8);
			
			Port port9 = new Port()
			{
				Uid = Guid.NewGuid().ToString(), // "imagep_003",
				Compartments = new List<Compartment>()
			};
			port9.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortName",Value="imagep"});
			port9.Data.Add("declared_pin_uid","imagep00");
			port9.Parent = ip3;
			ip3.Ports.Add(port9);
			diag.Ports.Add(port9);
			
			Port portA = new Port()
			{
				Uid = Guid.NewGuid().ToString(), // "imagepp_001",
				Compartments = new List<Compartment>()
			};
			portA.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortName",Value="imagepp"});
			portA.Data.Add("declared_pin_uid","imagepp00");
			portA.Parent = ip1;
			ip1.Ports.Add(portA);
			diag.Ports.Add(portA);
			
			Port portB = new Port()
			{
				Uid = Guid.NewGuid().ToString(), // "imagepp_002",
				Compartments = new List<Compartment>()
			};
			portB.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortName",Value="imagepp"});
			portB.Data.Add("declared_pin_uid","imagepp00");
			portB.Parent = ip2;
			ip2.Ports.Add(portB);
			diag.Ports.Add(portB);
			
			Port portC = new Port()
			{
				Uid = Guid.NewGuid().ToString(), // "imagepp_003",
				Compartments = new List<Compartment>()
			};
			portC.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortName",Value="imagepp"});
			portC.Data.Add("declared_pin_uid","imagepp00");
			portC.Parent = ip3;
			ip3.Ports.Add(portC);
			diag.Ports.Add(portC);
			
			// -----
			Port portD = new Port()
			{
				Uid = Guid.NewGuid().ToString(), // "imagerp1_001",
				Compartments = new List<Compartment>()
			};
			portD.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortName",Value="imagerp1"});
			portD.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortGroupName",Value="images"});
			portD.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortGroupDepths",Value="0,1"});
			portD.Data.Add("declared_pin_uid","imagerp01");
			portD.Parent = im;
			im.Ports.Add(portD);
			diag.Ports.Add(portD);
			
			Port portE = new Port()
			{
				Uid = Guid.NewGuid().ToString(), // "imagerp2_001",
				Compartments = new List<Compartment>()
			};
			portE.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortName",Value="imagerp2"});
			portE.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortGroupName",Value="images"});
			portE.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortGroupDepths",Value="0,1"});
			portE.Data.Add("declared_pin_uid","imagerp02");
			portE.Parent = im;
			im.Ports.Add(portE);
			diag.Ports.Add(portE);
			
			Port portF = new Port()
			{
				Uid = Guid.NewGuid().ToString(), // "imagerp3_001",
				Compartments = new List<Compartment>()
			};
			portF.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortName",Value="imagerp3"});
			portF.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortGroupName",Value="images"});
			portF.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortGroupDepths",Value="0,1"});
			portF.Data.Add("declared_pin_uid","imagerp03");
			portF.Parent = im;
			im.Ports.Add(portF);
			diag.Ports.Add(portF);
			
			Port portG = new Port()
			{
				Uid = Guid.NewGuid().ToString(), // "fimage_001",
				Compartments = new List<Compartment>()
			};
			portG.Compartments.Add(new Compartment(){Uid=Guid.NewGuid().ToString(), CompartmentType="PortName",Value="fimage"});
			portG.Data.Add("declared_pin_uid","fimage00");
			portG.Parent = im;
			im.Ports.Add(portG);
			diag.Ports.Add(portG);
			
			// Lines
			Line line = new Line(){
				Uid = Guid.NewGuid().ToString(),
				StartElement = inpp, EndElement = port1
			};
			diag.Lines.Add(line);
			
			line = new Line(){
				Uid = Guid.NewGuid().ToString(),
				StartElement = port2, EndElement = port3
			};
			diag.Lines.Add(line);
			
			line = new Line(){
				Uid = Guid.NewGuid().ToString(),
				StartElement = port4, EndElement = port7
			};
			diag.Lines.Add(line);
			
			line = new Line(){
				Uid = Guid.NewGuid().ToString(),
				StartElement = port5, EndElement = port8
			};
			diag.Lines.Add(line);
			
			line = new Line(){
				Uid = Guid.NewGuid().ToString(),
				StartElement = port6, EndElement = port9
			};
			diag.Lines.Add(line);
			
			line = new Line(){
				Uid = Guid.NewGuid().ToString(),
				StartElement = portA, EndElement = portD
			};
			diag.Lines.Add(line);
			
			line = new Line(){
				Uid = Guid.NewGuid().ToString(),
				StartElement = portB, EndElement = portE
			};
			diag.Lines.Add(line);
			
			line = new Line(){
				Uid = Guid.NewGuid().ToString(),
				StartElement = portC, EndElement = portF
			};
			diag.Lines.Add(line);
			
			line = new Line(){
				Uid = Guid.NewGuid().ToString(),
				StartElement = portG, EndElement = outp
			};
			diag.Lines.Add(line);
		}

	}
}