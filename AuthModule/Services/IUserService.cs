using AuthModule.DTOs;
using AuthModule.Entities;

namespace AuthModule.Services
{
    public interface IUserService
    {
        Task<UserDto> RegisterAsync(RegisterDto dto);
        Task<UserDto?> Validate(LoginDto dto);
        Task<UserDto?> GetByIdAsync(int id);
    }
}
