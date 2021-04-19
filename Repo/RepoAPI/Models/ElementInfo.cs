using System;
namespace RepoAPI.Models
{
    public class ElementInfo : DeepContext
    {
        public ModelInfo Model { get; set; }
        public string Name { get; set; }
    }
}
