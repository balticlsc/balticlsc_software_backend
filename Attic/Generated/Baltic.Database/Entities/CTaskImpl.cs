///////////////////////////////////////////////////////////
//  CTaskImpl.cs
//  Implementation of the Class CTaskImpl
//  Generated by Enterprise Architect
//  Created on:      20-kwi-2020 15:01:37
//  Original author: smialek
///////////////////////////////////////////////////////////



using System.Collections.Generic;
using System.Text.RegularExpressions;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Execution;

namespace Baltic.Database.Entities {
	public class CTaskImpl : CTask {
		
		public string AccountUid;
		
		public CTaskImpl(){

		}

		~CTaskImpl(){

		}
		
		private List<CJobBatch> _Batches;
		public override List<CJobBatch> Batches{
			get{
				if (null == _Batches) {
					// TODO pobrać z bazy danych na podstawie Uid
				}
				return _Batches;
			}
			set{
				_Batches = value;
			}
		}
		private List<CDataToken> _Tokens;
		public override List<CDataToken> Tokens{
			get{
				if (null == _Tokens) {
					// TODO pobrać z bazy danych na podstawie Uid
				}
				return _Tokens;
			}
			set{
				_Tokens = value;
			}
		}
		private TaskExecution _Execution;
		public override TaskExecution Execution{
			get{
				if (null == _Execution) {
					// TODO pobrać z bazy danych na podstawie Uid
				}
				return _Execution;
			}
			set{
				_Execution = value;
			}
		}
		
		public override string ToString() {
			string ret = "Task " + Uid + "\n";
			foreach (CDataToken token in Tokens)
				ret = ret + "\t" + Regex.Replace(token.ToString(), @"\n\t", "\n\t\t");
			foreach (CJobBatch batch in Batches)
				ret = ret + "\t" + Regex.Replace(batch.ToString(), @"\n\t", "\n\t\t");
			return ret;
		}

	}//end CTaskImpl

}//end namespace Baltic.Database.Entities