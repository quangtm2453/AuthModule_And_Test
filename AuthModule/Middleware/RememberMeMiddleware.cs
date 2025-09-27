using AuthModule.Services;
using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;

namespace AuthModule.Middleware
{
    public class RememberMeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDataProtector _protector;

        public RememberMeMiddleware(RequestDelegate next, IDataProtectionProvider provider)
        {
            _next = next;
            _protector = provider.CreateProtector("RememberMeCookieProtection_v1");
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            if (context.Session.GetInt32("UserId") == null)
            {
                if (context.Request.Cookies.TryGetValue("remember_me", out var cookie))
                {
                    try
                    {
                        var json = _protector.Unprotect(cookie);
                        var payload = JsonSerializer.Deserialize<RememberPayload>(json);
                        if (payload != null && payload.ExpiresAt > DateTime.UtcNow)
                        {
                            // validate user exists
                            var user = await userService.GetByIdAsync(payload.UserId);
                            if (user != null)
                            {
                                context.Session.SetInt32("UserId", payload.UserId);
                            }
                        }
                    }
                    catch
                    {
                        // invalid/tampered cookie: ignore (optionally delete cookie)
                    }
                }
            }

            await _next(context);
        }

        private class RememberPayload
        {
            public int UserId { get; set; }
            public DateTime ExpiresAt { get; set; }
        }
    }
}
