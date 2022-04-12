///////////////////////////////////////////////////////////
//  DataPin.cs
//  Implementation of the Class DataPin
//  Generated by Enterprise Architect
//  Created on:      01-mar-2020 18:40:56
//  Original author: smialek
///////////////////////////////////////////////////////////




namespace Baltic.DataModel.CAL
{
	public abstract class DataPin {

		public string Uid;
		public long TokenNo;

		public abstract string Name {get; set;}
		public abstract DataBinding Binding {get; set;}
		public abstract DataMultiplicity Multiplicity {get; set;}
		public abstract DataType Type {get; set;}
		public abstract AccessType Access {get; set;}
	
		public DataFlow Incoming;
		public DataFlow Outgoing;

		public DataPin(){

		}

		~DataPin(){

		}

		public virtual void Dispose(){

		}

	}
}//end DataPin