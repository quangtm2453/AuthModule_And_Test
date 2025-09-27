using System.Threading.Tasks;
using Xunit;
using Moq;
using AuthModule.Services;
using AuthModule.Repositories;
using AuthModule.Entities;
using AuthModule.DTOs;
using Microsoft.AspNetCore.Identity;

namespace AuthModule.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly IPasswordHasher<User> _hasher;
        private readonly UserService _service;

        public UserServiceTests()
        {
            // Tạo mock repository
            _userRepoMock = new Mock<IUserRepository>();

            // Dùng password hasher thật
            _hasher = new PasswordHasher<User>();

            // Inject vào UserService
            _service = new UserService(_userRepoMock.Object, _hasher);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnUserDto()
        {
            // Arrange
            var dto = new RegisterDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Password123",
                Role = "User"
            };

            // Setup mock: AddAsync chỉ cần trả về Task.CompletedTask
            _userRepoMock
                .Setup(r => r.AddAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.RegisterAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Username, result.Username);
            Assert.Equal(dto.Email, result.Email);
            Assert.Equal(dto.Role, result.Role);

            // Kiểm tra repository AddAsync được gọi đúng 1 lần
            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Validate_ShouldReturnUserDto_WhenPasswordIsCorrect()
        {
            // Arrange
            var user = new User("testuser", "test@test.com", "", "User");
            user.PasswordHash = _hasher.HashPassword(user, "123456");

            _userRepoMock.Setup(r => r.GetByUsernameAsync("testuser"))
                         .ReturnsAsync(user);

            var dto = new LoginDto { Username = "testuser", Password = "123456" };

            // Act
            var result = await _service.Validate(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public async Task Validate_ShouldReturnNull_WhenPasswordIsWrong()
        {
            // Arrange
            var user = new User("testuser", "test@test.com", "", "User");
            user.PasswordHash = _hasher.HashPassword(user, "123456");

            _userRepoMock.Setup(r => r.GetByUsernameAsync("testuser"))
                         .ReturnsAsync(user);

            var dto = new LoginDto { Username = "testuser", Password = "wrongpass" };

            // Act
            var result = await _service.Validate(dto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenUserNotFound()
        {
            // Arrange
            _userRepoMock.Setup(r => r.GetByIdAsync(99))
                         .ReturnsAsync((User)null!);

            // Act
            var result = await _service.GetByIdAsync(99);

            // Assert
            Assert.Null(result);
        }
    }
}
