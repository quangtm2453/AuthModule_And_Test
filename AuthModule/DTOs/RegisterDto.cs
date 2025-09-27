namespace AuthModule.DTOs
{
    public class RegisterDto
    {
     

        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "User";

        public RegisterDto(string username, string email, string password, string role)
        {
            Username = username;
            Email = email;
            Password = password;
            Role = role;
        }
        public RegisterDto()
        {
            
        }


    }
}
