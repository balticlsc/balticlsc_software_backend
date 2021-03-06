using System;
using System.Collections.Generic;
using Baltic.Core.Utils;
using Baltic.DataModel.Accounts;
using Baltic.DataModel.CAL;

namespace Baltic.UnitManager.Models {
	public class XComputationUnitCrude : IComparable<XComputationUnitCrude> {
		public string Name { get; set; }
		public string Uid { get; set; }
		public string PClass { get; set; }
		public string ShortDescription { get; set; }
		public string LongDescription { get; set; }
		public IEnumerable<string> Keywords { get; set; }
		public string Icon { get; set; }
		public bool IsApp { get; set; }
		public bool IsService { get; set; }
		
		public XComputationUnitCrude(ComputationUnit unit = null)
		{
			if (null == unit) return;
			DBMapper.Map<XComputationUnitCrude>(unit, this);
			if (null != unit.Descriptor)
				DBMapper.Map<XComputationUnitCrude>(unit.Descriptor, this);
			IsApp = unit is ComputationApplication;
			IsService = unit is ComputationModule cm && cm.IsService;
		}

		public ComputationUnit ToModelObject()
		{
			ComputationUnit unit = IsApp ? 
				(ComputationUnit) new ComputationApplication() : 
				(ComputationUnit) new ComputationModule()
				{
					IsService = IsService
				};
			DBMapper.Map<ComputationUnit>(this, unit);
			unit.Descriptor = DBMapper.Map<UnitDescriptor>(this, new UnitDescriptor());
			return unit;
		}

		public int CompareTo(XComputationUnitCrude other)
		{
			return String.Compare(Name, other?.Name, StringComparison.Ordinal);
		}
	}
}