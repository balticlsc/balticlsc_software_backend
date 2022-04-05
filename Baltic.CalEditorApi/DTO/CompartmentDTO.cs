using System;
using Baltic.CalEditorRegistry.Model;

namespace Baltic.CalEditorRegistry.DTO
{
    public class CompartmentDTO
    {
        private string _id { get; set; }
        private string _elementId { get; set; }
        private string _compartmentUuid { get; set; }
        private Guid _elementUuid { get; set; }

        public string Id
        {
            get => _id;
            set => _id = value ?? Guid.NewGuid().ToString("N");
        }

        public string CompartmentUuid
        {
            get => _compartmentUuid;
            set
            {
                if (value != null)
                {
                    _compartmentUuid = value;
                    _id = value;
                }
            }
        }
        public string ElementId
        {
            get => _elementId;
            set
            {
                if (value != null)
                {
                    _elementId = value;
                    var result = Guid.TryParse(value, out var guid);

                    if (result)
                    {
                        _elementUuid = guid;
                    }
                }
            }
        }
        public Guid ElementUuid
        {
            get => _elementUuid;
            set
            {
                if (value != null)
                {
                    _elementUuid = value;
                    _elementId = value.ToString();
                }
            }
        }
        public string CompartmentTypeId { get; set; }
        public string Input { get; set; }
        public string Value { get; set; }
        public CompartmentStyle Style { get; set; } = new CompartmentStyle();
    }
}