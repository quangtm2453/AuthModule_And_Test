using AuthModule.Controllers;
using AuthModule.DTOs;
using AuthModule.Entities;
using AuthModule.Models;
using AuthModule.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Text.Json;
using Xunit;

namespace AuthModule.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IDataProtector> _protectorMock;
        private readonly Mock<IDataProtectionProvider> _providerMock;
        private readonly Mock<IWebHostEnvironment> _envMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _protectorMock = new Mock<IDataProtector>();
            _providerMock = new Mock<IDataProtectionProvider>();
            _envMock = new Mock<IWebHostEnvironment>();

            _providerMock.Setup(p => p.CreateProtector(It.IsAny<string>()))
                         .Returns(_protectorMock.Object);

            _controller = new AuthController(_userServiceMock.Object, _providerMock.Object, _envMock.Object);

            // Thiết lập HttpContext cho session và cookies
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new DummySession(); // Tạo session dummy
            _controller.ControllerContext.HttpContext = httpContext;
        }

        [Fact]
        public void Login_Get_ReturnsView()
        {
            var result = _controller.Login() as ViewResult;

            Assert.NotNull(result);
            Assert.IsType<LoginViewModel>(result.Model);
        }

        [Fact]
        public async Task Login_Post_InvalidModel_ReturnsViewWithModel()
        {
            _controller.ModelState.AddModelError("Username", "Required");
            var model = new LoginViewModel();

            var result = await _controller.Login(model) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(model, result.Model);
        }

        [Fact]
        public async Task Login_Post_InvalidUser_AddsModelError()
        {
            var model = new LoginViewModel { Username = "user", Password = "pass" };

            // Mock trả về null UserDto
            _userServiceMock.Setup(s => s.Validate(It.IsAny<LoginDto>()))
                            .ReturnsAsync((UserDto)null);

            var result = await _controller.Login(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(_controller.ModelState.IsValid);
        }


        [Fact]
        public async Task Login_Post_ValidUser_SetsSessionAndRedirects()
        {
            var model = new LoginViewModel { Username = "admin", Password = "pass" };

            // Mock trả về UserDto hợp lệ
            var userDto = new UserDto { Id = 1, Role = "Admin" };
            _userServiceMock.Setup(s => s.Validate(It.IsAny<LoginDto>()))
                            .ReturnsAsync(userDto);

            _protectorMock.Setup(p => p.Protect(It.IsAny<string>())).Returns("protected");

            var result = await _controller.Login(model) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Admin", result.ControllerName);
            Assert.Equal("Home", result.ActionName);
            Assert.Equal(1, _controller.HttpContext.Session.GetInt32("UserId"));
        }

        [Fact]
        public void Logout_ClearsSessionAndCookies()
        {
            _controller.HttpContext.Session.SetInt32("UserId", 1);
            _controller.HttpContext.Response.Cookies.Append("remember_me", "value");

            var result = _controller.Logout() as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Login", result.ActionName);
            Assert.Null(_controller.HttpContext.Session.GetInt32("UserId"));
            // Không thể kiểm tra cookie trực tiếp vì DefaultHttpContext Cookies chỉ hỗ trợ append
        }
    }

    // Dummy session để test
    public class DummySession : ISession
    {
        private readonly Dictionary<string, byte[]> _storage = new();

        public IEnumerable<string> Keys => _storage.Keys;
        public string Id => "dummy";
        public bool IsAvailable => true;

        public void Clear() => _storage.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _storage.Remove(key);
        public void Set(string key, byte[] value) => _storage[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _storage.TryGetValue(key, out value);
    }
}
