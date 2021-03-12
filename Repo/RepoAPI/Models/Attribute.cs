using System;
namespace RepoAPI.Models
{
    public class Attribute : DeepContext
    {
        public string Name { get; set; }
        public ElementInfo Type { get; set; }
    }
}
