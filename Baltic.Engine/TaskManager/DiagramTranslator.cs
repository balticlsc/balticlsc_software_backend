using System;
using System.Collections.Generic;
using Baltic.DataModel.CAL;
using Baltic.DataModel.Diagram;
using Baltic.DataModel.Types;
using Baltic.Types.DataAccess;

namespace Baltic.Engine.TaskManager {
	public class DiagramTranslator {
		
		/// <summary>
		/// string - diagram Element Uid
		/// </summary>
		private IDictionary<string,DataPin> _diagram2DomainMap;
		private ComputationApplicationRelease _appRelease;
		private CALDiagram _diagram;
		private IUnitProcessing _unitRegistry;

		/// 
		/// <param name="release"></param>
		/// <param name="unitRegistry"></param>
		/// <param name="diagramRegistry"></param>
		public DiagramTranslator(ComputationApplicationRelease release, IUnitProcessing unitRegistry, IDiagram diagramRegistry){
			_diagram2DomainMap = new Dictionary<string, DataPin>();
			_unitRegistry = unitRegistry;
			_diagram = diagramRegistry.GetDiagram(release.DiagramUid);
			_appRelease = release;
		}

		public void TranslateDiagram()
		{
			InitiateDeclaredPinMap();
			CreateUnitCalls();
			CreateDataFlows();
		}

		public void TranslateDiagramPartial(){
			CreateDeclaredPins();
		}
		
		public void TranslateDiagramFull(){
			CreateDeclaredPins();
			CreateUnitCalls();
			CreateDataFlows();
		}
		
		private void CreateDeclaredPins(){
			DeclaredDataPin pin;
			foreach (Box box in _diagram.Boxes)
			{
				pin = null;
				if ("RequiredDataPin" == box.ElementType){
					pin = new DeclaredDataPin(){
						Name = GetCompartmentValue(box,"RequiredDataPinName"),
						Uid = box.Uid, // TODO - check: Guid.NewGuid().ToString(),
						TokenMultiplicity = "true" == GetCompartmentValue(box,"RequiredDataPinMultiplicity") ? CMultiplicity.Multiple : CMultiplicity.Single,
						DataMultiplicity = "true" == GetCompartmentValue(box,"RequiredDataPinDataMultiplicity") ? CMultiplicity.Multiple : CMultiplicity.Single,
						Binding = "true" == GetCompartmentValue(box,"RequiredDataPinMandatory") ? DataBinding.RequiredStrong : DataBinding.RequiredWeak,
						Type = _unitRegistry.GetDataType(GetCompartmentValue(box,"RequiredDataPinDataType")),
						Access = _unitRegistry.GetAccessType(GetCompartmentValue(box,"RequiredDataPinAccessType"))
					};
				};
				if ("ProvidedDataPin" == box.ElementType) {
					pin = new DeclaredDataPin(){
						Name = GetCompartmentValue(box,"ProvidedDataPinName"),
						Uid = box.Uid, // TODO - check: Guid.NewGuid().ToString(),
						TokenMultiplicity = "true" == GetCompartmentValue(box,"ProvidedDataPinMultiplicity") ? CMultiplicity.Multiple : CMultiplicity.Single,
						DataMultiplicity = "true" == GetCompartmentValue(box,"ProvidedDataPinDataMultiplicity") ? CMultiplicity.Multiple : CMultiplicity.Single,
						Binding = DataBinding.Provided,
						Type = _unitRegistry.GetDataType(GetCompartmentValue(box,"ProvidedDataPinDataType")),
						Access = _unitRegistry.GetAccessType(GetCompartmentValue(box,"ProvidedDataPinAccessType"))
					};
				};
				if (null != pin) {
					_appRelease.DeclaredPins.Add(pin);
					_diagram2DomainMap.Add(box.Uid,pin);
				}
			}
		}

		private void InitiateDeclaredPinMap()
		{
			// TODO - update to work also with Guid.NewGuid for DeclaredDataPins
			foreach(DeclaredDataPin pin in _appRelease.DeclaredPins)
				if (!_diagram2DomainMap.ContainsKey(pin.Uid)) // this should always be the case, but...
					_diagram2DomainMap.Add(pin.Uid,pin);
		}

		private void CreateUnitCalls(){
			_appRelease.Calls = new List<UnitCall>();
			foreach (Box box in _diagram.Boxes){
				if ("RequiredDataPin" != box.ElementType && "ProvidedDataPin" != box.ElementType){
					UnitCall unitCall = new UnitCall(){
						Name = GetCompartmentValue(box,box.ElementType+"Name"),
						Strength = "true" == GetCompartmentValue(box,box.ElementType+"Binding") ? UnitStrength.Strong : UnitStrength.Weak,
						Pins = new List<ComputedDataPin>(),
						Unit = _unitRegistry.GetUnitRelease(box.Data["callable_unit_uid"])
					};
					CreateComputedPins(box,unitCall);
					_appRelease.Calls.Add(unitCall);
					// Diagram2DomainMap.Add(box.Uid,unitCall); TODO - verify if this is needed
				}
			}
		}

		/// 
		/// <param name="box"></param>
		/// <param name="unitCall"></param>
		private void CreateComputedPins(Box box, UnitCall unitCall){
			Dictionary<string,PinGroup> groups = new Dictionary<string, PinGroup>();
			foreach (Port port in _diagram.Ports){
				if (port.Parent == box){
					ComputedDataPin pin = new ComputedDataPin(){
						Uid = Guid.NewGuid().ToString(),
						// Name = GetCompartmentValue(port,"PortName"), // TODO - verify if name of computed pin same as declared pin
						Declared = unitCall.Unit.DeclaredPins.Find(p => p.Uid == port.Data["declared_pin_uid"])
					};
					unitCall.Pins.Add(pin);
					pin.Call = unitCall;
					_diagram2DomainMap.Add(port.Uid,pin);
					string pinGroupName;
					if (null != (pinGroupName = GetCompartmentValue(port,"PortGroupName"))) {
						PinGroup group;
						if (!groups.ContainsKey(pinGroupName)){
							group = new PinGroup(){Name = pinGroupName};
							string[] depths = GetCompartmentValue(port,"PortGroupDepths").Split(new []{','});
							foreach (string depth in depths) 
								group.Depths.Add(int.Parse(depth));
							groups.Add(pinGroupName,group);
						} else
							group = groups[pinGroupName];
						pin.Group = group;
					}
				}
			}
		}

		private void CreateDataFlows(){
			DataPin start,end;
			_appRelease.Flows = new List<DataFlow>();
			foreach (Line line in _diagram.Lines) {
				start = _diagram2DomainMap[line.StartElement.Uid];
				end = _diagram2DomainMap[line.EndElement.Uid];
				DataFlow flow = new DataFlow(){
					Source = start,
					Target = end
				};
				start.Outgoing = flow; // TODO - check if OK; added
				end.Incoming = flow; // TODO
				_appRelease.Flows.Add(flow);
			}
		}

		/// 
		/// <param name="element"></param>
		/// <param name="name"></param>
		private string GetCompartmentValue(Element element, string name){
			Compartment comp = element.Compartments.Find(cmp => cmp.CompartmentType == name);
			return null == comp ? null: comp.Value;
		}

	}
}