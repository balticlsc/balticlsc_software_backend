///////////////////////////////////////////////////////////
//  CDataTokenImpl.cs
//  Implementation of the Class CDataTokenImpl
//  Generated by Enterprise Architect
//  Created on:      20-kwi-2020 16:59:41
//  Original author: smialek
///////////////////////////////////////////////////////////




using Baltic.DataModel.CALExecutable;
namespace Baltic.Database.Entities {
	public class CDataTokenImpl : CDataToken {

		public CDataTokenImpl(){

		}

		~CDataTokenImpl(){

		}
		
		public string OwnerUid;
		
		private CDataSet _DataSet;
		public override CDataSet DataSet{
			get{
				if (null == _DataSet) {
					// TODO pobrać z bazy danych na podstawie Uid
				}
				return _DataSet;
			}
			set{
				_DataSet = value;
			}
		}

	}//end CDataTokenImpl

}//end namespace Baltic.Database.Entities