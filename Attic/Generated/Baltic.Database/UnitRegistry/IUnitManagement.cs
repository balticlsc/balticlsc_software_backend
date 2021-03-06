///////////////////////////////////////////////////////////
//  IUnitManagement.cs
//  Implementation of the Interface IUnitManagement
//  Generated by Enterprise Architect
//  Created on:      17-kwi-2020 11:16:54
//  Original author: smialek
///////////////////////////////////////////////////////////




using System.Collections.Generic;
using Baltic.DataModel.Accounts;
using Baltic.DataModel.CAL;

namespace Baltic.Database.UnitRegistry {
	public interface IUnitManagement  {

		/// 
		/// <param name="name"></param>
		string CreateApp(string name);

		/// 
		/// <param name="name"></param>
		string CreateModule(string name);

		/// 
		/// <param name="releaseUid"></param>
		ComputationUnitRelease GetUnitRelease(string releaseUid);

		string CreateDiagram();

		/// 
		/// <param name="query"></param>
		List<ComputationUnit> FindUnits(UnitQuery query);

		/// 
		/// <param name="diagramUid"></param>
		string CopyDiagram(string diagramUid);

		/// 
		/// <param name="unitUid"></param>
		ComputationUnit GetUnit(string unitUid);

		/// 
		/// <param name="unit"></param>
		short UpdateUnit(ComputationUnit unit);

		/// 
		/// <param name="release"></param>
		short UpdateUnitRelease(ComputationUnitRelease release);

		/// 
		/// <param name="unitUid"></param>
		short DeleteUnit(string unitUid);

		/// 
		/// <param name="unitUid"></param>
		short DeleteUnitRelease(string unitUid);
	}//end IUnitManagement

}//end namespace Baltic.Database.UnitRegistry