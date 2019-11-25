using System.ComponentModel.DataAnnotations;

namespace Auth.Models
{
    public class AuthInfo
    {
        public string Login { get; set; } 
        public string Password { get; set; }
    }
}