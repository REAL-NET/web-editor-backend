using System;
using System.Collections.Generic;

namespace RepoAPI.Models
{
    public class Model
    {
        public string Name { get; set; }
        public bool HasMetamodel { get; set; }
        public ModelInfo Metamodel { get; set; }
        public IEnumerable<ElementInfo> Elements { get; set; }
        public IEnumerable<ElementInfo> Nodes { get; set; }
        public IEnumerable<ElementInfo> Relationships { get; set; }
    }
}
