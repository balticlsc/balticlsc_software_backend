///////////////////////////////////////////////////////////
//  ComputationUnit.cs
//  Implementation of the Class ComputationUnit
//  Generated by Enterprise Architect
//  Created on:      19-mar-2020 11:46:51
//  Original author: smialek
///////////////////////////////////////////////////////////


using System.Collections.Generic;
using Baltic.DataModel.Accounts;

namespace Baltic.DataModel.CAL
{
	public abstract class ComputationUnit {

		public string Name;
		public string Uid;
		public ProblemClass PClass;
		public ComputationUnit ForkParent;
		public UnitDescriptor Descriptor;
		public UserAccount Author;

		public abstract List<ComputationUnitRelease> Releases {get; set;}
	
		public ComputationUnit(){

		}

		~ComputationUnit(){

		}	

	}
}//end ComputationUnit