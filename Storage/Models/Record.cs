using System;
using System.ComponentModel.DataAnnotations;

namespace Storage.Models
{
    public class Record
    {
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

        public override bool Equals(object obj)
        {
            return obj is Record record && SaveId.Equals(record.SaveId);
        }
    }
}