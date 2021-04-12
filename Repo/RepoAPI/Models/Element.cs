using System;
using System.Collections.Generic;

namespace RepoAPI.Models
{
    public class Element : DeepContext
    {
        public string Name { get; set; }
        //public IEnumerable<ElementInfo> OutgoingEdges { get; set; }
        public IEnumerable<ElementInfo> OutgoingAssociations { get; set; }
        public IEnumerable<ElementInfo> IncomingAssociations { get; set; }
        public IEnumerable<ElementInfo> DirectSupertypes { get; set; }
        public IEnumerable<Slot> Slots { get; set; }
        public ModelInfo Model { get; set; }
        public ElementInfo Metatype { get; set; }
    }
}
