using System;
namespace RepoAPI.Models
{
    public class Edge : Element
    {
        public Element From { get; set; }
        public Element To { get; set; }
    }
}
