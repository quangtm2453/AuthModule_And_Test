using System.ComponentModel.DataAnnotations;

namespace AuthModule.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        public string Role { get; set; } = "User";

        public User() { }

        public User(string username, string email, string passwordHash, string role = "User")
        {
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
        }
    }
}
