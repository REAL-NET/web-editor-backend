using System.Collections.Generic;

namespace RepoAPI.Models
{
    public class GenerationParameters
    {
        public string ModelName { get; set; }
        public Dictionary<string, string> Dictionary { get; set; }
    }
}