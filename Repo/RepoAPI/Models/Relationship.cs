namespace RepoAPI.Models
{
    public class Relationship : Element
    {
        public ElementInfo Source { get; set; }
        public ElementInfo Target { get; set; }
    }
}