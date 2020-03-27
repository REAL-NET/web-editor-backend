using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Storage.Models.Database
{
    public class Record
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public string User { get; set; }
        
        [Required]
        public string SaveId { get; set; }
        
        [Required]
        public DateTime LastModified { get; set; }
        
        public string Info { get; set; }
    }
}