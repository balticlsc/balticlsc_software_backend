///////////////////////////////////////////////////////////
//  CJobBatch.cs
//  Implementation of the Class CJobBatch
//  Generated by Enterprise Architect
//  Created on:      01-mar-2020 18:40:56
//  Original author: smialek
///////////////////////////////////////////////////////////


using System.Collections.Generic;
using Baltic.DataModel.Execution;

namespace Baltic.DataModel.CALExecutable {
	public abstract class CJobBatch {

		public string Uid;
		public int DepthLevel;
		public abstract List<CJob> Jobs {get; set;}
		public abstract List<CDataToken> Tokens {get; set;}
		public abstract List<BatchExecution> BatchInstances {get; set;}

		public CJobBatch(){
			DepthLevel = -1;
		}

		~CJobBatch(){

		}

		public virtual void Dispose(){

		}
		
	}//end CJobBatch

}