using System;
using Baltic.DataModel.CAL;
using Baltic.Types.DataAccess;

namespace Baltic.UnitManager.Models
{
    public abstract class XUnitRelease : XUnitReleaseCrude, IComparable<XUnitRelease>
    {
		public XComputationUnitCrude Unit { get; set; }
        
        protected XUnitRelease(ComputationUnitRelease release = null) : base(release)
        {
            if (null == release) return;
            Unit = new XComputationUnitCrude(release.Unit);
        }
        
        protected new ComputationUnitRelease ToModelObject(ComputationUnitRelease release, IUnitManagement unitRegistry)
        {
            base.ToModelObject(release, unitRegistry);
            release.Unit = Unit.ToModelObject();
            return release;
        }

        public int CompareTo(XUnitRelease other)
        {
            return Unit?.CompareTo(other?.Unit) ?? -1;
        }
    }
}