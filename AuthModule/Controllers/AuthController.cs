using AuthModule.DTOs;
using AuthModule.Models;
using AuthModule.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AuthModule.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly IDataProtector _protector;
        private readonly IWebHostEnvironment _env;

        public AuthController(IUserService userService, IDataProtectionProvider provider, IWebHostEnvironment env)
        {
            _userService = userService;
            _protector = provider.CreateProtector("RememberMeCookieProtection_v1");
            _env = env;
        }

        [HttpGet]
        public IActionResult Login() {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = new LoginDto { Username = model.Username, Password = model.Password, RememberMe = model.RememberMe };
            var user = await _userService.Validate(dto);
            if (user == null)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
                return View(model);
            }

            // Lưu session
            HttpContext.Session.SetInt32("UserId", user.Id);

            // Nếu RememberMe thì tạo cookie 
            if (model.RememberMe)
            {
                var payload = new { UserId = user.Id, ExpiresAt = DateTime.UtcNow.AddDays(14) };
                var json = JsonSerializer.Serialize(payload);
                var protectedValue = _protector.Protect(json);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = !_env.IsDevelopment(),
                    Expires = DateTimeOffset.UtcNow.AddDays(14),
                    SameSite = SameSiteMode.Lax
                };
                Response.Cookies.Append("remember_me", protectedValue, cookieOptions);
            }

            // redirect theo role
            if (user.Role == "Admin") return RedirectToAction("Admin", "Home");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register() {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = new RegisterDto { Username = model.Username, Email = model.Email, Password = model.Password, Role = model.Role };
            var created = await _userService.RegisterAsync(dto);

            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("remember_me");
            return RedirectToAction("Login");
        }
    }
}
