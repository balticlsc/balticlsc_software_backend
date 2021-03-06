///////////////////////////////////////////////////////////
//  ComputedDataPin.cs
//  Implementation of the Class ComputedDataPin
//  Generated by Enterprise Architect
//  Created on:      01-mar-2020 18:40:56
//  Original author: smialek
///////////////////////////////////////////////////////////




namespace Baltic.DataModel.CAL
{
	public class ComputedDataPin : DataPin {
	
		public override string Name {get{return Declared.Name;} set{Declared.Name=value;}}
		public override DataBinding Binding {get{return Declared.Binding;} set{Declared.Binding=value;}}
		public override DataMultiplicity Multiplicity {get{return Declared.Multiplicity;} set{Declared.Multiplicity=value;}}
		public override DataType Type {get{return Declared.Type;} set{Declared.Type=value;}}
		public override AccessType Access {get{return Declared.Access;} set{Declared.Access=value;}}
	
		public DeclaredDataPin Declared;
		public PinGroup Group;

		public ComputedDataPin(){

		}

		~ComputedDataPin(){

		}

		public override void Dispose(){

		}

	}
}//end ComputedDataPin