using System;
using System.Collections.Generic;
using System.Linq;
using Baltic.Core.Utils;
using Baltic.DataModel.Accounts;
using Baltic.DataModel.CAL;
using Baltic.DataModel.Execution;
using Baltic.DataModel.Types;
using Baltic.Types.DataAccess;
using Baltic.Types.Models;

namespace Baltic.UnitManager.Models {
	public abstract class XUnitReleaseCrude {
		public string Version { get; set; }
		public string Uid { get; set; }
		public UnitReleaseStatus Status { get; set; }
		public DateTime Date { get; set; }
		public string Description { get; set; }
		public bool OpenSource { get; set; }
		public long UsageCounter { get; set; }
		public IEnumerable<XDeclaredPin> Pins { get; set; }
		public XReservationRange SupportedResourcesRange { get; set; }
		
		protected XUnitReleaseCrude(ComputationUnitRelease release = null)
		{
			if (null == release) return;
			DBMapper.Map<XUnitReleaseCrude>(release, this);
			
			if (null != release.Descriptor)
				DBMapper.Map<XUnitReleaseCrude>(release.Descriptor, this);
			
			SupportedResourcesRange = new XReservationRange(release.SupportedResourcesRange);
			
			Pins = release.DeclaredPins.Select(p => new XDeclaredPin(p)).ToList();
		}

		public abstract ComputationUnitRelease ToModelObject(IUnitManagement unitRegistry, ComputationUnit unit);

		protected ComputationUnitRelease ToModelObject(ComputationUnitRelease release, IUnitManagement unitRegistry)
		{
			DBMapper.Map<ComputationUnitRelease>(this, release);
			release.Descriptor = DBMapper.Map<ReleaseDescriptor>(this, new ReleaseDescriptor());
			release.SupportedResourcesRange = SupportedResourcesRange?.ToModelObject();
			release.DeclaredPins = Pins.Select(p => p.ToModelObject(unitRegistry)).ToList();
			return release;
		}
	}

}