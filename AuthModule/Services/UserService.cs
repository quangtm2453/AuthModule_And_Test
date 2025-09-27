using AuthModule.DTOs;
using AuthModule.Entities;
using AuthModule.Repositories;
using Microsoft.AspNetCore.Identity;

namespace AuthModule.Services
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepo;
        private readonly IPasswordHasher<User> _hasher;

        public UserService(IUserRepository userRepo, IPasswordHasher<User> hasher)
        {
            _userRepo = userRepo;
            _hasher = hasher;
        }

        public async Task<UserDto> RegisterAsync(RegisterDto dto)
        {
            var user = new User(dto.Username, dto.Email, "", dto.Role);
            user.PasswordHash = _hasher.HashPassword(user, dto.Password);
            await _userRepo.AddAsync(user);

            return new UserDto { Id = user.Id, Username = user.Username, Email = user.Email, Role = user.Role };
        }
        


        public async Task<UserDto?> Validate(LoginDto dto)
        {
            var user = await _userRepo.GetByUsernameAsync(dto.Username);
            if (user == null)
                return null;

            var res = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (res == PasswordVerificationResult.Success)
            {
                return new UserDto { Id = user.Id, Username = user.Username, Email = user.Email, Role = user.Role };
            }
            return null;
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) 
                return null;
            return new UserDto { Id = user.Id, Username = user.Username, Email = user.Email, Role = user.Role };
        }
    }
}
