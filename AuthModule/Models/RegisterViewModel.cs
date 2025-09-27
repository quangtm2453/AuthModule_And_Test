using System.ComponentModel.DataAnnotations;

namespace AuthModule.Models
{
    public class RegisterViewModel
    {
        [Required] 
        public string Username { get; set; } = "";

        [Required]
        [EmailAddress] 
        public string Email { get; set; } = "";

        [Required]
        [DataType(DataType.Password)] 
        public string Password { get; set; } = "";

        public string Role { get; set; } = "User";
    }
}
