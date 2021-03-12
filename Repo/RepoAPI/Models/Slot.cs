namespace RepoAPI.Models
{
    public class Slot : DeepContext
    {
        public Attribute Attribute { get; set; }
        public ElementInfo Value { get; set; }
    }
}