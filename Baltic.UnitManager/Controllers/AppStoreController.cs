using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Baltic.Core.Utils;
using Baltic.DataModel.Accounts;
using Baltic.DataModel.CAL;
using Baltic.Types.DataAccess;
using Baltic.UnitManager.Models;
using Baltic.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Baltic.UnitManager.Controllers {
	[ApiController]
    [Authorize]
	[Route("app")]
	public class AppStoreController : UnitManagementController {

        public AppStoreController(IConfiguration config, IUnitManagement unitRegistry, IUserSessionRoutine userSession) : base(config,unitRegistry,userSession)
        {
        }


		/// <summary>
		/// [GetShelfApps] Get data of ComputationApplicationRelease-s for a given user.
		/// </summary>
		/// <returns>A list of ComputationApplicationRelease records.</returns>
		[HttpGet("shelf")]
        public IActionResult GetShelfApps()
        {
            var appReleases = new List<XApplicationRelease>();

            try
            {
                // Set the UnitQuery to find app releases in the app shelf
                var unitQuery = new UnitQuery()
                {
                    IsApp = true,
                    AllUnits = false,
                    UserUid = UserName,
                    IsInToolbox = false
                };
                return FindUnitReleases(unitQuery);
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }


        /// <summary>
        /// [AddAppToShelf] Adds a selected ComputationApplicationRelease to the AppShelf of a given user.
        /// </summary>
        /// <param name="releaseUid">The Uid of the selected ComputationApplicationRelease.</param>
        [HttpPost("shelf")]
        public IActionResult AddAppToShelf([FromQuery,Required] string releaseUid)
        {
            try
            {
                var userUid = UserName;
                var res = _unitRegistry.AddUnitToShelf(releaseUid, userUid, false);
                if (res == 0)
                    return Ok();
                return HandleError("App not added", HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

        /// <summary>
        /// [RemoveAppFromShelf] Removes a selected ComputationApplicationRelease from the AppShelf of a given user.
        /// </summary>
        /// <param name="releaseUid">The Uid of the selected ComputationApplicationRelease.</param>
        [HttpDelete("shelf")]
		public IActionResult RemoveAppFromShelf([FromQuery,Required] string releaseUid){
            try
            {
                var userUid = UserName;
                if (0 == _unitRegistry.RemoveUnitFromShelf(releaseUid, userUid, false))
                    return Ok();
                return HandleError("App not removed from App Shelf", HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
		}

		/// <summary>
		/// [FindApps] Get data of ComputationApplication-s according to a UnitQuery.
		/// </summary>
		/// <param name="query">Query parameters (UnitQuery).</param>
		/// <returns>A list of ComputationApplication records.</returns>
		[HttpPost("list")]
		public IActionResult FindApps([FromBody,Required] XUnitQuery query){
            try
            {
                if (null == query)
                    return HandleError("Query is empty", HttpStatusCode.BadRequest);
                query.IsApp = true;
                query.AllUnits = false;
                var unitQuery = DBMapper.Map<UnitQuery>(query, new UnitQuery());
                return FindUnits(unitQuery);
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }

		/// <summary>
		/// [GetApp] Get data of a given ComputationApplication.
		/// </summary>
		/// <param name="appUid">The Uid of the given ComputationApplication.</param>
		/// <returns>A single ComputationApplication record.</returns>
		[HttpGet]
        public IActionResult GetApp([FromQuery,Required] string appUid)
        {
            try
            {
                var unit = _unitRegistry.GetUnit(appUid);

                if (unit == null)
                    return HandleError("Unit not found", HttpStatusCode.BadRequest);
                if (!(unit is ComputationApplication))
                    return HandleError("This unit is not an application", HttpStatusCode.BadRequest);
                return Ok(MapUnit(unit));
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }


        /// <summary>
		/// [RateApp] Update the ComputationApplication with a new rating value.
		/// </summary>
		/// <param name="appUid">The Uid of the given ComputationApplication.</param>
		/// <param name="rate">The rating value.</param>
		[HttpPut("rate")]
		public IActionResult RateApp([FromQuery,Required] string appUid, [FromQuery,Required] int rate){
            // TODO
			return Ok();
		}

        /// <summary>
        /// [FindAppReleases] Get data of ComputationApplicationRelease-s according to a UnitQuery.
        /// </summary>
        /// <param name="query">Query parameters (UnitQuery).</param>
        /// <returns>A list of ComputationApplicationRelease records.</returns>
        [HttpPost("release/list")]
        public IActionResult FindAppReleases([FromBody,Required] XUnitQuery query)
        {
            try
            {
                if (null == query)
                    return HandleError("Query is empty", HttpStatusCode.BadRequest);
                var unitQuery = DBMapper.Map<UnitQuery>(query, new UnitQuery());
                unitQuery.IsApp = true;
                unitQuery.AllUnits = false;
                return FindUnitReleases(unitQuery);
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }
    }
}