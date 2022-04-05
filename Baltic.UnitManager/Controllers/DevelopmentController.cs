using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using Baltic.Core.Utils;
using Baltic.DataModel.Accounts;
using Baltic.DataModel.CAL;
using Baltic.DataModel.Execution;
using Baltic.Types.DataAccess;
using Baltic.UnitManager.Models;
using Baltic.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Baltic.UnitManager.Controllers
{
    [ApiController] 
    [Authorize]
    [Route("dev")]
    public class DevelopmentController : UnitManagementController
    {
        private IUnitDevManager _devManager;

        public DevelopmentController(IConfiguration config, IUnitDevManager devManager, IUnitManagement unitRegistry, IUserSessionRoutine userSession) : base(config,unitRegistry, userSession)
        {
            _devManager = devManager;
        }

        /// <summary>
        /// [CreateApp] Creates a new ComputationApplication with the given name.
        /// </summary>
        /// <param name="name">Name for the ComputationApplication to be created.</param>
        /// <returns>The Uid of the newly created ComputationApplication.</returns>
        [HttpPut("app")]
        public IActionResult CreateApp([FromQuery,Required] string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    return HandleError("Name is empty", HttpStatusCode.BadRequest);
                var userUid = UserName;
                var appUid = _devManager.CreateApp(name, userUid);
                if (null == appUid)
                    return HandleError("Not possible to create an app", HttpStatusCode.InternalServerError);
                return Ok(appUid,"ok");
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        /// <summary>
        /// [CreateModule] Creates a new ComputationModule with the given name.
        /// </summary>
        /// <param name="name">Name for the ComputationModule to be created.</param>
        /// <returns>The Uid of the newly created ComputationModule.</returns>
        [HttpPut("module")]
        public IActionResult CreateModule([FromQuery,Required] string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    return HandleError("Name is empty", HttpStatusCode.BadRequest);
                
                var userUid = UserName;
                var moduleUid = _unitRegistry.CreateModule(name, userUid);
                
                if (null == moduleUid)
                    return HandleError("Not possible to create a module", HttpStatusCode.InternalServerError);
                return Ok(moduleUid,"ok");
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        /// <summary>
        /// [CreateAppRelease] Creates a new ComputationApplicationRelease for a given ComputationApplication with the given version.
        /// </summary>
        /// <param name="appUid">The Uid of the ComputationApplication.</param>
        /// <param name="version">The Version designation for the ComputationApplicationRelease.</param>
        /// <returns></returns>
        [HttpPut("appRelease")]
        public IActionResult CreateAppRelease([FromQuery,Required] string appUid, [FromQuery,Required] string version)
        {
            try
            {
                if (string.IsNullOrEmpty(version))
                    return HandleError("Version is empty", HttpStatusCode.BadRequest);
                var userUid = UserName;
                if (!CheckUnitOwnership(appUid, userUid))
                    return HandleError("Application not authored by the user.", HttpStatusCode.BadRequest);
                
                var releaseUid = _devManager.CreateAppRelease(appUid, version);
                
                if (null == releaseUid)
                    return HandleError("Not possible to create an app release", HttpStatusCode.InternalServerError);
                return Ok(releaseUid,"ok");
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        /// <summary>
        /// [CreateModuleRelease] Creates a new ComputationModuleRelease for a given ComputationModule with the given data.
        /// </summary>
        /// <param name="moduleUid">The Uid of the ComputationModule.</param>
        /// <param name="release">Data of the release to be added, with a list of DeclaredDataPins.</param>
        /// <returns>The Uid of the newly created ComputationModuleRelease.</returns>
        [HttpPut("moduleRelease")]
        public IActionResult CreateModuleRelease([FromQuery,Required] string moduleUid, [FromBody,Required] XModuleReleaseCrude release)
        {
            try
            {
                if (null == release)
                    return HandleError("Release data is empty.", HttpStatusCode.BadRequest);
                if (string.IsNullOrEmpty(release.Version))
                    return HandleError("Version is empty", HttpStatusCode.BadRequest); 
                ComputationUnit unit = _unitRegistry.GetUnit(moduleUid);
                var userUid = UserName;
                if (!CheckUnitOwnership(unit, userUid))
                    return HandleError("Application not authored by the user.", HttpStatusCode.BadRequest);
                
                var releaseUid = _unitRegistry.AddReleaseToUnit(moduleUid,release.ToModelObject(_unitRegistry, unit));
                
                if (null == releaseUid)
                    return HandleError("Not possible to create a module release", HttpStatusCode.InternalServerError);
                return Ok(releaseUid,"ok");
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        /// <summary>
        /// [FindUnits] Get data of ComputationUnit-s according to a UnitQuery.
        /// </summary>
        /// <param name="query">Query parameters (UnitQuery).</param>
        /// <returns>A list of ComputationUnit records.</returns>
        [HttpPost("unit/list")]
        public IActionResult FindUnits([FromBody,Required] XUnitQuery query)
        {
            try
            {
                if (null == query)
                    return HandleError("Query is empty", HttpStatusCode.BadRequest);
                
                var unitQuery = DBMapper.Map<UnitQuery>(query, new UnitQuery());
                return FindUnits(unitQuery);
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        /// <summary>
        /// [GetUserUnits] Get data of ComputationUnit-s authored by the current user.
        /// </summary>
        /// <returns>A list of ComputationUnit records.</returns>
        [HttpGet("shelf")]
        public IActionResult GetUserUnits()
        {
            try
            {
                var query = new UnitQuery()
                {
                    AuthorUid = UserName
                };

                return FindUnits(query);
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }
        
        /// <summary>
        /// [FindUnitReleases] Get data of ComputationUnitRelease-s according to a UnitQuery.
        /// </summary>
        /// <returns>A list of ComputationUnitRelease records.</returns>
        [HttpPost("release/list")]
        public IActionResult FindUnitReleases([FromBody,Required] XUnitQuery query)
        {
            try
            {
                if (null == query)
                    return HandleError("Query is empty", HttpStatusCode.BadRequest);
                
                var unitQuery = DBMapper.Map<UnitQuery>(query, new UnitQuery());
                return FindUnitReleases(unitQuery);
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }
        
        /// <summary>
        /// [GetToolboxUnits] Get data of ComputationUnitReleases-s in the Toolbox of the current user.
        /// </summary>
        /// <returns>A list of ComputationUnitRelease records.</returns>
        [HttpGet("toolbox")]
        public IActionResult GetToolboxUnits()
        {
            try
            {
                var query = new UnitQuery()
                {
                    AllUnits = true,
                    UserUid = UserName,
                    IsInToolbox = true
                };

                return FindUnitReleases(query);
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        /// <summary>
        /// [GetUnit] Get data of a given ComputationUnit.
        /// </summary>
        /// <param name="unitUid">The Uid of the ComputationUnit.</param>
        /// <returns>A single ComputationUnit record.</returns>
        [HttpGet("unit")]
        public IActionResult GetUnit([FromQuery,Required] string unitUid)
        {
            try
            {
                var unit = _unitRegistry.GetUnit(unitUid);

                if (unit == null)
                    return HandleError("Unit not found", HttpStatusCode.BadRequest);
                var userUid = UserName;
                if (!CheckUnitOwnership(unit, userUid))
                    return HandleError("Unit not available to the user.", HttpStatusCode.Unauthorized);
                
                return Ok(MapUnit(unit));
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }
        
        /// <summary>
        /// [GetUnitRelease] Get data of a given ComputationUnitRelease.
        /// </summary>
        /// <param name="releaseUid">The Uid of the ComputationUnitRelease.</param>
        /// <returns>A single ComputationUnitRelease record.</returns>
        [HttpGet("release")]
        public IActionResult GetUnitRelease([FromQuery,Required] string releaseUid)
        {
            try
            {
                var release = _unitRegistry.GetUnitRelease(releaseUid);

                if (release == null)
                    return HandleError("Unit Release not found", HttpStatusCode.BadRequest);
                var userUid = UserName;
                if (!CheckUnitOwnership(release.Unit, userUid))
                    return HandleError("Unit Release not available to the user.", HttpStatusCode.Unauthorized);
                
                return Ok(MapUnitRelease(release));
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }
        
        /// <summary>
        /// [UpdateUnit] Update data for a given ComputationUnit.
        /// </summary>
        /// <param name="unit">The new ComputationUnit data.</param>
        /// <returns></returns>
        [HttpPost("unit")]
        public IActionResult UpdateUnit([FromBody,Required] XComputationUnitCrude unit)
        {
            try
            {
                if (null == unit)
                    return HandleError("Unit is empty", HttpStatusCode.BadRequest);
                
                var userUid = UserName;
                if (!CheckUnitOwnership(unit.Uid, userUid))
                    return HandleError("Unit not available to the user.", HttpStatusCode.Unauthorized);
                
                if (0 != _unitRegistry.UpdateUnit(unit.ToModelObject()))
                    return HandleError("Unit could not be updated", HttpStatusCode.InternalServerError);
                return Ok();
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        /// <summary>
        /// [UpdateAppRelease] Update data for a given ComputationApplicationRelease.
        /// </summary>
        /// <param name="release">The new ComputationApplicationRelease data.</param>
        /// <returns></returns>
        [HttpPost("appRelease")]
        public IActionResult UpdateAppRelease([FromBody,Required] XUnitReleaseCrude release)
        {
            try
            {
                if (null == release)
                    return HandleError("App Release is empty", HttpStatusCode.BadRequest);
                
                ComputationUnitRelease currRelease = _unitRegistry.GetUnitRelease(release.Uid);
                
                if (null == currRelease)
                    return HandleError("App Release does not exist", HttpStatusCode.BadRequest);
                var userUid = UserName;
                if (!CheckUnitOwnership(currRelease.Unit, userUid))
                    return HandleError("App Release not available to the user.", HttpStatusCode.Unauthorized);

                ComputationApplicationRelease updRelease =
                    (ComputationApplicationRelease) release.ToModelObject(_unitRegistry, currRelease.Unit);
                
                if (0 != _unitRegistry.UpdateUnitRelease(updRelease))
                    return HandleError("App Release could not be updated", HttpStatusCode.InternalServerError);
                return Ok();
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }
        
        /// <summary>
        /// [UpdateModuleRelease] Update data for a given ComputationModuleRelease.
        /// </summary>
        /// <param name="release">The new ComputationModuleRelease data.</param>
        /// <returns></returns>
        [HttpPost("moduleRelease")]
        public IActionResult UpdateModuleRelease([FromBody,Required] XModuleReleaseCrude release)
        {
            try
            {
                if (null == release)
                    return HandleError("Module Release is empty", HttpStatusCode.BadRequest);
                
                ComputationUnitRelease currRelease = _unitRegistry.GetUnitRelease(release.Uid);
                
                if (null == currRelease)
                    return HandleError("Module Release does not exist", HttpStatusCode.BadRequest);
                var userUid = UserName;
                if (!CheckUnitOwnership(currRelease.Unit, userUid))
                    return HandleError("Module Release not available to the user.", HttpStatusCode.Unauthorized);
                
                if (0 != _unitRegistry.UpdateUnitRelease(release.ToModelObject(_unitRegistry,currRelease.Unit)))
                    return HandleError("Unit Release could not be updated", HttpStatusCode.InternalServerError);
                return Ok();
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        /// <summary>
        /// [DeleteUnit] Remove data of a given ComputationUnit.
        /// </summary>
        /// <param name="unitUid">The Uid of the ComputationUnit.</param>
        /// <returns></returns>
        [HttpDelete("unit")]
        public IActionResult DeleteUnit([FromQuery,Required] string unitUid)
        {
            try
            {
                var userUid = UserName;
                if (!CheckUnitOwnership(unitUid, userUid))
                    return HandleError("Unit not available to the user.", HttpStatusCode.Unauthorized);
                
                if (0 == _devManager.DeleteUnit(unitUid))
                    return Ok();
                
                return HandleError("Unit could not be deleted", HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        /// <summary>
        /// [DeleteUnitRelease] Remove data of a given ComputationUnitRelease.
        /// </summary>
        /// <param name="releaseUid">The Uid of the ComputationUnitRelease.</param>
        /// <returns></returns>
        [HttpDelete("release")]
        public IActionResult DeleteUnitRelease([FromQuery,Required] string releaseUid)
        {
            try
            {
                var userUid = UserName;
                ComputationUnitRelease release = _unitRegistry.GetUnitRelease(releaseUid);
                
                if (!CheckUnitOwnership(release?.Unit, userUid))
                    return HandleError("Unit Release not available to the user.", HttpStatusCode.Unauthorized);
                
                if (0 == _devManager.DeleteUnitRelease(releaseUid))
                    return Ok();
                
                return HandleError("Unit Release could not be deleted", HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        /// <summary>
        /// [AddUnitToToolbox] Adds a selected ComputationUnitRelease to the Toolbox of a given user.
        /// </summary>
        /// <param name="releaseUid">The Uid of the ComputationUnitRelease.</param>
        [HttpPut("toolbox")]
        public IActionResult AddUnitToToolbox([FromQuery,Required] string releaseUid)
        {
            try
            {
                var userUid = UserName;
                var res = _unitRegistry.AddUnitToShelf(releaseUid, userUid, true);
                
                if (res == 0)
                    return Ok();
                return HandleError("Unit not added to toolbox", HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        /// <summary>
        /// [RemoveUnitFromToolbox] Removes a selected ComputationUnitRelease from the Toolbox of a given user.
        /// </summary>
        /// <param name="releaseUid">The Uid of the ComputationUnitRelease.</param>
        [HttpDelete("toolbox")]
        public IActionResult RemoveUnitFromToolbox([FromQuery,Required] string releaseUid)
        {
            try
            {
                var userUid = UserName;
                if (0 == _unitRegistry.RemoveUnitFromShelf(releaseUid, userUid, true))
                    return Ok();
                
                return HandleError("Unit not removed from toolbox", HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        private bool CheckUnitOwnership(ComputationUnit unit, string ownerUid)
        {
            return ownerUid == unit?.AuthorUid;
        }
		
        private bool CheckUnitOwnership(string unitUid, string ownerUid)
        {
            return CheckUnitOwnership(_unitRegistry.GetUnit(unitUid),ownerUid);
        }
        
    }
}