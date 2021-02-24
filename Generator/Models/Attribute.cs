using System;
namespace RepoAPI.Models
{
    public class Attribute
    {
        public string Name { get; set; }
        public AttributeKind Kind { get; set; }
        public bool IsInstantiable { get; set; }
        public Element Type { get; set; }
        public string StringValue { get; set; }
        public Element ReferenceValue { get; set; }
    }
}
