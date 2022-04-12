///////////////////////////////////////////////////////////
//  DevelopmentController.cs
//  Implementation of the Class DevelopmentController
//  Generated by Enterprise Architect
//  Created on:      17-kwi-2020 17:46:09
//  Original author: smialek
///////////////////////////////////////////////////////////




using System.Collections.Generic;
using Baltic.Service.Models;
using Baltic.Engine.UnitManager;
using Baltic.Database.UnitRegistry;

namespace Baltic.Service.Controllers {
	public class DevelopmentController {

		private IUnitDevManager DevManager;
		private IUnitManagement UnitRegistry;

		public DevelopmentController(){

		}

		~DevelopmentController(){

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
		/// <param name="appUid"></param>
		/// <param name="version"></param>
		public string CreateAppRelease(string appUid, string version){

			return "";
		}

		/// 
		/// <param name="moduleUid"></param>
		/// <param name="version"></param>
		/// <param name="YAML"></param>
		public string CreateModuleRelease(string moduleUid, string version, string YAML){

			return "";
		}

		/// 
		/// <param name="query"></param>
		public List<XComputationUnit> FindUnits(XUnitQuery query){

			return null;
		}

		public List<XComputationUnit> GetUserUnits(){

			return null;
		}

		/// 
		/// <param name="unitUid"></param>
		public XComputationUnit GetUnit(string unitUid){

			return null;
		}

		public IEnumerable<XUnitRelease> GetToolboxUnits(){

			return null;
		}

		/// 
		/// <param name="releaseUid"></param>
		public void AddUnitToToolbox(string releaseUid){

		}

		/// 
		/// <param name="releaseUid"></param>
		public void RemoveUnitFromToolbox(string releaseUid){

		}

	}//end DevelopmentController

}//end namespace Baltic.Service.Controllers