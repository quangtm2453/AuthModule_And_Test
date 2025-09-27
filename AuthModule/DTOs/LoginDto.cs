namespace AuthModule.DTOs
{
    public class LoginDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }

        public LoginDto(string username, string password, bool rememberMe)
        {
            this.Username = username;
            this.Password = password;
            this.RememberMe = rememberMe;
        }
        public LoginDto()
        {
            
        }

    }
}
