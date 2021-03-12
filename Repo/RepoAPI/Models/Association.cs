namespace RepoAPI.Models
{
    public class Association : Relationship
    {
        public int MinSource { get; set; }
        public int MaxSource { get; set; }
        public int MinTarget { get; set; }
        public int MaxTarget { get; set; }
    }
}