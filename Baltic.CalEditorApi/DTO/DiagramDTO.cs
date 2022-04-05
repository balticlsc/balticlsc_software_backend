using System;
using System.Collections.Generic;
using Baltic.CalEditorRegistry.Model;

namespace Baltic.CalEditorRegistry.DTO
{
    public class DiagramDTO
    {
        private string _id { get; set; }
        private Guid _diagramId { get; set; }
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
                        _diagramId = guid;
                    }
                }
                else
                {
                    _id = Guid.NewGuid().ToString();
                    _diagramId = Guid.Parse(_id);
                }
            }
        }
        public Guid DiagramUuid
        {
            get => _diagramId;
            set
            {
                _diagramId = value;
                _id = _diagramId.ToString();
            }
        }
        public string Name { get; set; }
        public string Data { get; set; }
        public List<Box> Boxes { get; set; } = new List<Box>();
        public List<Line> Lines { get; set; } = new List<Line>();
        public List<Port> Ports { get; set; } = new List<Port>();
    }
}