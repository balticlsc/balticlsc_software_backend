using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Baltic.Core.Utils;
using Baltic.DataModel.Accounts;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Types;
using Baltic.TaskManager.Models;
using Baltic.Types.DataAccess;
using Baltic.Web.Common;
using Baltic.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Baltic.TaskManager.Controllers {
	
	[ApiController]
	[Authorize]
	[Route("task")]
	public class DataShelfController : BalticController{

		private IUnitProcessing _unitRegistry;

		public DataShelfController(IUnitProcessing unitRegistry, IUserSessionRoutine userSession):base(userSession){
			_unitRegistry = unitRegistry;
		}

		/// <summary>
		/// [AddDataSetToShelf] Adds a new TaskDataSet to the Data Shelf of the current user.
		/// </summary>
		/// <param name="dataSet">The TaskData set to be added.</param>
		/// <returns>THe Uid of the newly created TaskDataSet.</returns>
		[HttpPut("dataSet")]
		public IActionResult AddDataSetToShelf([FromBody] XTaskDataSet dataSet)
		{
			try
			{
				if (null == dataSet)
					return HandleError("Data Set is empty", HttpStatusCode.BadRequest);
				string userUid = UserName;
				var tDataSet = DBMapper.Map<TaskDataSet>(dataSet, new TaskDataSet()
				{
					Type = _unitRegistry.GetDataType(dataSet.DataTypeUid),
					Structure = _unitRegistry.GetDataStructure(dataSet.DataStructureUid),
					Access = _unitRegistry.GetAccessType(dataSet.AccessTypeUid),
					Data = DBMapper.Map<CDataSet>(dataSet, new CDataSet()),
					AccessData = null != dataSet.AccessValues ? new CDataSet(){Values = dataSet.AccessValues} : null,
					OwnerUid = userUid
				});
				
				string result = _unitRegistry.AddDataSetToShelf(tDataSet);
				return Ok(result,"ok");
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}

		/// <summary>
		/// [DeleteDataSet] Deletes the TaskDataSet with the given Uid.
		/// </summary>
		/// <param name="dataSetUid">The Uid of the TaskDataSet to be removed.</param>
		/// <returns></returns>
		[HttpDelete("dataSet")]
		public IActionResult DeleteDataSet([FromQuery] string dataSetUid)
		{
			try
			{
				string userUid = UserName;
				if (!CheckTaskDataSetOwnership(dataSetUid, userUid))
					return HandleError("Data Set not available for the user", HttpStatusCode.Unauthorized);
				if (0 != _unitRegistry.DeleteDataSet(dataSetUid))
					return HandleError("Data Set not found",HttpStatusCode.BadRequest);
				return Ok();
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}

		/// <summary>
		/// [UpdateDataSet] Updates the given TaskDataSet.
		/// </summary>
		/// <param name="dataSet">The TaskDataSet data to be updated.</param>
		/// <returns></returns>
		[HttpPost("dataSet")]
		public IActionResult UpdateDataSet([FromBody] XTaskDataSet dataSet)
		{
			try
			{
				if (null == dataSet)
					return HandleError("Data Set is empty", HttpStatusCode.BadRequest);
				string userUid = UserName;
				if (!CheckTaskDataSetOwnership(dataSet.Uid, userUid))
					return HandleError("Data Set not available for the user", HttpStatusCode.Unauthorized);
				var tDataSet = DBMapper.Map<TaskDataSet>(dataSet, new TaskDataSet()
				{
					Type = _unitRegistry.GetDataType(dataSet.DataTypeUid),
					Structure = _unitRegistry.GetDataStructure(dataSet.DataStructureUid),
					Access = _unitRegistry.GetAccessType(dataSet.AccessTypeUid),
					Data = DBMapper.Map<CDataSet>(dataSet, new CDataSet()),
					AccessData = null != dataSet.AccessValues ? new CDataSet(){Values = dataSet.AccessValues} : null,
					OwnerUid = userUid
				});
				if (0 != _unitRegistry.UpdateDataSet(tDataSet))
					return HandleError("Data Set not found", HttpStatusCode.InternalServerError);
				return Ok();
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}

		/// <summary>
		/// [GetDataSet] Get the TaskDataSet with the given Uid.
		/// </summary>
		/// <param name="dataSetUid">The Uid of the TaskDataSet to be retrieved.</param>
		/// <returns></returns>
		[HttpGet("dataSet")]
		public IActionResult GetDataSet([FromQuery] string dataSetUid)
		{
			try
			{
				var dataSet = _unitRegistry.GetDataSet(dataSetUid);
				if (null == dataSet)
					return HandleError("Data Set not found", HttpStatusCode.BadRequest);
				string userUid = UserName;
				if (!CheckTaskDataSetOwnership(dataSet, userUid))
					return HandleError("Data Set not available for the user", HttpStatusCode.Unauthorized);
				var xDataSet = MapTaskDataSet(dataSet);
				
				return Ok(xDataSet);
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}

		/// <summary>
		/// [GetDataShelf] Get a list of TaskDataSet-s in the Data Shelf of the current user.
		/// </summary>
		/// <returns>The list of TaskDataSets.</returns>
		[HttpGet("dataShelf")]
		public IActionResult GetDataShelf()
		{
			try
			{
				string userUid = UserName;
				IEnumerable<TaskDataSet> dataShelf = _unitRegistry.GetDataShelf(userUid);
				var result = dataShelf.Select(MapTaskDataSet).ToList();
				return Ok(result);
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}

		/// <summary>
		/// [GetAvailableDataTypes] Get the list of all DataTypes available to the current user.
		/// </summary>
		/// <returns>The list of available DataTypes.</returns>
		[HttpGet("dataTypes")]
		public IActionResult GetAvailableDataTypes()
		{
			try
			{
				IEnumerable<DataType> dataTypes = _unitRegistry.GetDataTypes();
				var result = dataTypes.Select(dt => DBMapper.Map<XDataType>(dt,
					new XDataType())).ToList();
				return Ok(result);
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}

		/// <summary>
		/// [GetDataType] Get the DataType with the given Uid.
		/// </summary>
		/// <param name="uid">The Uid of the specific DataType.</param>
		/// <returns></returns>
		[HttpGet("dataType")]
		public IActionResult GetDataType([FromQuery] string uid){
			try
			{
				DataType type = _unitRegistry.GetDataType(uid);
				if (null == type)
					return HandleError("Data Type not found", HttpStatusCode.BadRequest);
				var result = DBMapper.Map<XDataType>(type, new XDataType());
				return Ok(result);
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}

		/// <summary>
		/// [GetAvailableDataStructures] Get the list of all DataStructures available to the current user.
		/// </summary>
		/// <returns>The list of available DataStructures.</returns>
		[HttpGet("dataStructures")]
		public IActionResult GetAvailableDataStructures()
		{
			try
			{
				IEnumerable<DataStructure> dataStructures = _unitRegistry.GetDataStructures();
				var result = dataStructures.Select(ds => DBMapper.Map<XDataStructure>(ds,
					new XDataStructure())).ToList();
				return Ok(result);
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}

		/// <summary>
		/// [GetDataStructure] Get the DataStructure with the given Uid.
		/// </summary>
		/// <param name="uid">The Uid of the specific DataStructure.</param>
		/// <returns></returns>
		[HttpGet("dataStructure")]
		public IActionResult GetDataStructure([FromQuery] string uid)
		{
			try
			{
				DataStructure structure = _unitRegistry.GetDataStructure(uid);
				if (null == structure)
					return HandleError("Data Structure not found", HttpStatusCode.BadRequest);
				var result = DBMapper.Map<XDataStructure>(structure, new XDataType());
				return Ok(result);
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}

		/// <summary>
		/// [GetAvailableAccessTypes] Get the list of all AccessTypes available to the current user.
		/// </summary>
		/// <returns>The list of available AccessTypes.</returns>
		[HttpGet("accessTypes")]
		public IActionResult GetAvailableAccessTypes()
		{
			try
			{
				IEnumerable<AccessType> accessTypes = _unitRegistry.GetAccessTypes();
				var result = accessTypes.Select(dt => DBMapper.Map<XAccessType>(dt,
					new XAccessType())).ToList();
				return Ok(result);
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}

		/// <summary>
		/// [GetDataType] Get the AccessType with the given Uid.
		/// </summary>
		/// <param name="uid">The Uid of the specific AccessType.</param>
		/// <returns></returns>
		[HttpGet("accessType")]
		public IActionResult GetAccessType([FromQuery] string uid)
		{
			try
			{
				AccessType type = _unitRegistry.GetAccessType(uid);
				if (null == type)
					return HandleError("Access Type not found", HttpStatusCode.BadRequest);
				var result = DBMapper.Map<XAccessType>(type, new XAccessType());
				return Ok(result);
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}
		
		private XTaskDataSet MapTaskDataSet(TaskDataSet dataSet)
		{
			return DBMapper.Map<XTaskDataSet>(dataSet, new XTaskDataSet()
				{
					DataTypeUid = dataSet.Type?.Uid,
					DataTypeName = dataSet.Type?.Name,
					DataTypeVersion = dataSet.Type?.Version,
					DataStructureUid = dataSet.Structure?.Uid,
					DataStructureName = dataSet.Structure?.Name,
					DataStructureVersion = dataSet.Structure?.Version,
					AccessTypeUid = dataSet.Access?.Uid,
					AccessTypeName = dataSet.Access?.Name,
					AccessTypeVersion = dataSet.Access?.Version,
					Values = dataSet.Data?.Values,
					AccessValues = dataSet.AccessData?.Values
				});
		}
		
		private bool CheckTaskDataSetOwnership(TaskDataSet taskDataSet, string ownerUid)
		{
			return ownerUid == taskDataSet?.OwnerUid;
		}
		
		private bool CheckTaskDataSetOwnership(string taskDataSetUid, string ownerUid)
		{
			return CheckTaskDataSetOwnership(_unitRegistry.GetDataSet(taskDataSetUid),ownerUid);
		}

	}
}