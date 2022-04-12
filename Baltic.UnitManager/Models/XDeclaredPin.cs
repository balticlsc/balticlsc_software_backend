using System;
using Baltic.Core.Utils;
using Baltic.DataModel.CAL;
using Baltic.DataModel.Types;
using Baltic.Types.DataAccess;

namespace Baltic.UnitManager.Models
{
    public class XDeclaredPin : IComparable<XDeclaredPin>
    {
        public string Uid { get; set; }
        public string Name { get; set; }
        public DataBinding Binding { get; set; }
        public CMultiplicity TokenMultiplicity { get; set; }
        public CMultiplicity DataMultiplicity { get; set; }
        public string DataTypeUid { get; set; }
        public string DataTypeName { get; set; }
        public string DataStructureUid { get; set; }
        public string DataStructureName { get; set; }
        public string AccessTypeUid { get; set; }
        public string AccessTypeName { get; set; }

        public XDeclaredPin(DeclaredDataPin pin = null)
        {
            if (null == pin) return;
            DBMapper.Map<XDeclaredPin>(pin, this);
            DataTypeName = pin.Type?.Name;
            DataTypeUid = pin.Type?.Uid;
            DataStructureName = pin.Structure?.Name;
            DataStructureUid = pin.Structure?.Uid;
            AccessTypeName = pin.Access?.Name;
            AccessTypeUid = pin.Access?.Uid;
        }

        public DeclaredDataPin ToModelObject(IUnitManagement unitRegistry)
        {
            DeclaredDataPin ret = DBMapper.Map<DeclaredDataPin>(this, new DeclaredDataPin()
            {
                Access = unitRegistry.GetAccessType(AccessTypeUid),
                Type = unitRegistry.GetDataType(DataTypeUid),
                Structure = unitRegistry.GetDataStructure(DataStructureUid)
            });
            if (string.IsNullOrEmpty(Uid))
                ret.Uid = Guid.NewGuid().ToString();
            return ret;
        }

        public int CompareTo(XDeclaredPin other)
        {
            return String.Compare(Name, other?.Name, StringComparison.Ordinal);
        }
    }
}