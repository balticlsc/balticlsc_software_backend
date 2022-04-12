///////////////////////////////////////////////////////////
//  CTask.cs
//  Implementation of the Class CTask
//  Generated by Enterprise Architect
//  Created on:      01-mar-2020 18:40:56
//  Original author: smialek
///////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using Baltic.DataModel.Execution;

namespace Baltic.DataModel.CALExecutable {
	public abstract class CTask {

		public string Uid;
		public string AppUid;
		public abstract List<CJobBatch> Batches {get; set;}
		public abstract List<CDataToken> Tokens {get; set;}
		public abstract TaskExecution Execution {get; set;}

		public CTask(){
			
		}

		~CTask(){

		}

		public virtual void Dispose(){

		}
		
	}//end CTask

}//end namespace CAL_Executable