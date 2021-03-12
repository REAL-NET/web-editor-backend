using System;
using System.Collections.Generic;

namespace RepoAPI.Models
{
    public class Element : DeepContext
    {
        public string Name { get; set; }
        public IEnumerable<Relationship> OutgoingEdges { get; set; }
        public IEnumerable<Association> OutgoingAssociations { get; set; }
        public IEnumerable<Association> IncomingAssociations { get; set; }
        public IEnumerable<ElementInfo> DirectSupertypes { get; set; }
        public IEnumerable<Slot> Slots { get; set; }
        public ModelInfo Model { get; set; }
        public bool HasMetatype { get; set; }
        public ElementInfo Metatype { get; set; }
    }
}
