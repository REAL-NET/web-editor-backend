using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Storage.Models.Database
{
    /// <summary>
    /// Represents the type of record for EF database.
    /// Contains info about saved repo.
    /// </summary>
    public class DbRecord : Record
    {
        /// <summary>
        /// Autoincremental parameter to be primary key in database
        /// </summary>
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
    }
}