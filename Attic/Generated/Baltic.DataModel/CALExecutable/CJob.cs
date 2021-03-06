///////////////////////////////////////////////////////////
//  CJob.cs
//  Implementation of the Class CJob
//  Generated by Enterprise Architect
//  Created on:      01-mar-2020 18:40:56
//  Original author: smialek
///////////////////////////////////////////////////////////


using System.Collections.Generic;
using Baltic.DataModel.Execution;

namespace Baltic.DataModel.CALExecutable {
	public abstract class CJob
	{

		public string Uid;
		public string UnitUid;
		public int Multiplicity;
		public string BatchUid;
		public string YAML;
		public abstract List<CDataToken> Tokens {get; set;}
		public abstract List<JobExecution> JobInstances {get; set;}

		public CJob()
		{
			
		}

		~CJob()
		{

		}

		public virtual void Dispose()
		{

		}

	}
//end CJob

}