namespace RepoAPI.Models
{
    public class Slot : DeepContext
    {
        public Attribute Attribute { get; set; }
        public ElementInfo Value { get; set; }
        public bool IsSimple { get; set; }
        public string SimpleValue { get; set; }
    }
}