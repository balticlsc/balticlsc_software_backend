///////////////////////////////////////////////////////////
//  UnitManagementProxy.cs
//  Implementation of the Class UnitManagementProxy
//  Generated by Enterprise Architect
//  Created on:      17-kwi-2020 11:17:08
//  Original author: smialek
///////////////////////////////////////////////////////////



using System.Collections.Generic;
using Baltic.DataModel.Accounts;
using Baltic.DataModel.CAL;
using Baltic.Database.UnitRegistry;

namespace Baltic.Database.UnitRegistry {
	public class UnitManagementProxy : IUnitManagement {

		public UnitManagementProxy(){

		}

		~UnitManagementProxy(){

		}

		/// 
		/// <param name="name"></param>
		public string CreateApp(string name){

			return "";
		}

		/// 
		/// <param name="name"></param>
		public string CreateModule(string name){

			return "";
		}

		/// 
		/// <param name="releaseUid"></param>
		public ComputationUnitRelease GetUnitRelease(string releaseUid){

			return null;
		}

		public string CreateDiagram(){

			return "";
		}

		/// 
		/// <param name="query"></param>
		public List<ComputationUnit> FindUnits(UnitQuery query){

			return null;
		}

		/// 
		/// <param name="diagramUid"></param>
		public string CopyDiagram(string diagramUid){

			return "";
		}

		/// 
		/// <param name="unitUid"></param>
		public ComputationUnit GetUnit(string unitUid){

			return null;
		}

		/// 
		/// <param name="unit"></param>
		public short UpdateUnit(ComputationUnit unit){

			return 0;
		}

		/// 
		/// <param name="release"></param>
		public short UpdateUnitRelease(ComputationUnitRelease release){

			return 0;
		}

		/// 
		/// <param name="unitUid"></param>
		public short DeleteUnit(string unitUid){

			return 0;
		}

		/// 
		/// <param name="unitUid"></param>
		public short DeleteUnitRelease(string unitUid){

			return 0;
		}

	}//end UnitManagementProxy

}//end namespace Baltic.Database.UnitRegistry