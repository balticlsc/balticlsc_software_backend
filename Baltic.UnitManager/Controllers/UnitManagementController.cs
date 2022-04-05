using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Baltic.Core.Utils;
using Baltic.DataModel.Accounts;
using Baltic.DataModel.CAL;
using Baltic.Types.DataAccess;
using Baltic.UnitManager.Models;
using Baltic.Web.Common;
using Baltic.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Baltic.UnitManager.Controllers {
	public abstract class UnitManagementController : BalticController {
		
		protected IConfiguration _config;
		protected IUnitManagement _unitRegistry;

		public UnitManagementController(IConfiguration config, IUnitManagement unitRegistry, IUserSessionRoutine userSession):base(userSession)
		{
			_config = config;
			_unitRegistry = unitRegistry;
		}
		
		protected IActionResult FindUnits(UnitQuery unitQuery)
        {
            var units = _unitRegistry.FindUnits(unitQuery);

            if (units != null)
            {
                var xUnits = new List<XComputationUnit>();
                foreach (var unit in units)
                    xUnits.Add(MapUnit(unit));
                return Ok(xUnits);
            }

            return HandleError("Units not found");
        }

        protected XComputationUnit MapUnit(ComputationUnit unit)
        {
            return (unit is ComputationApplication app) ?
                new XComputationApplication(app) :
                new XComputationUnit(unit);
        }

        protected IActionResult FindUnitReleases(UnitQuery unitQuery)
        {
            var releases = _unitRegistry.FindUnitReleases(unitQuery);

            if (releases != null)
            {
                var xReleases = new List<XUnitRelease>();
                foreach (var release in releases)
                    xReleases.Add(MapUnitRelease(release));
                return Ok(xReleases);
            }

            return HandleError("Unit Releases not found");
        }
        
        protected XUnitRelease MapUnitRelease(ComputationUnitRelease release)
        {
            return (release is ComputationApplicationRelease app) ?
                (XUnitRelease) new XApplicationRelease(app) :
                (XUnitRelease) new XModuleRelease((ComputationModuleRelease) release);
        }

    }
}