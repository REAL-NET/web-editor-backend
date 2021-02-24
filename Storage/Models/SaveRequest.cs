using System;
using System.ComponentModel.DataAnnotations;

namespace Storage.Models
{
    public class SaveRequest
    {
        [Required]
        public string User { get; set; }
        
        public DateTime LastModified { get; set; }
        
        public string Info { get; set; }
        
    }
}