using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Storage.Models.Database
{
    /// <summary>
    /// Represents the type of record for EF database.
    /// Contains info about saved repo.
    /// </summary>
    public class Record
    {
        /// <summary>
        /// Autoincremental parameter to be primary key in database
        /// </summary>
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        /// <summary>
        /// Username which owns the saved repository.
        /// </summary>
        [Required]
        public string User { get; set; }
        
        /// <summary>
        /// Unique identifier for saved repository
        /// Used for requests to repository microservice
        /// </summary>
        [Required]
        public string SaveId { get; set; }
        
        /// <summary>
        /// Stores date and time when record was added to database.
        /// Can be specified by user or automatically set to current time.
        /// </summary>
        [Required]
        public DateTime LastModified { get; set; }
        
        /// <summary>
        /// Optional field than could be used for storing additional information.
        /// </summary>
        public string Info { get; set; }
    }
}