using System;
using System.Collections.Generic;
using Baltic.CalEditorRegistry.Model;

namespace Baltic.CalEditorRegistry.DTO
{
    public class ElementDTO
    {
        private string _id { get; set; }
        private string _diagramId { get; set; }
        private Guid _elementUuid { get; set; }
        private Guid _diagramUuid { get; set; }
        public string Id
        {
            get => _id;
            set
            {
                if (value != null)
                {
                    _id = value;
                    var result = Guid.TryParse(value, out var guid);

                    if (result)
                    {
                        _elementUuid = guid;
                    }
                }
                else
                {
                    _id = Guid.NewGuid().ToString();
                    _elementUuid = Guid.Parse(_id);
                }
            }
        }
        public Guid ElementUuid
        {
            get => _elementUuid;
            set
            {
                _elementUuid = value;
                _id = value.ToString();
            }
        }
        public string DiagramId
        {
            get => _diagramId;
            set
            {
                if (value != null)
                {
                    _diagramId = value;
                    var result = Guid.TryParse(value, out var guid);
                    
                    if (result)
                    {
                        _diagramUuid = guid;
                    }
                }
            }
        }
        public Guid DiagramUuid
        {
            get => _diagramUuid;
            set
            {
                _diagramUuid = value;
                _diagramId = value.ToString();
            }
        }
        public string ElementTypeId { get; set; }
        public string Data { get; set; }
        public DiagramElementType Type { get; set; }
        public List<Compartment> Compartments { get; set; } = new List<Compartment>();
    }
}