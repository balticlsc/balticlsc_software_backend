/*
 * Created by SharpDevelop.
 * User: smialek
 * Date: 20.04.2020
 * Time: 12:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Baltic.Database.Entities
{
	/// <summary>
	/// Description of JobBatchEntity.
	/// </summary>
	public class CJobBatchEntity
	{
		public string Uid;
		public int DepthLevel;
		public string TaskUid;
		
		CJobBatchEntity()
		{
		}
	}
}
