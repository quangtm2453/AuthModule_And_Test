using AuthModule.Data;
using AuthModule.Repositories;
using AuthModule.Services;
using AuthModule.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(conn, ServerVersion.AutoDetect(conn)));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordHasher<AuthModule.Entities.User>, PasswordHasher<AuthModule.Entities.User>>();

builder.Services.AddDataProtection();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// --- Optional: apply migrations on startup (dev convenience) ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // will apply any pending migrations; will create DB if doesn't exist and migrations exist
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseMiddleware<RememberMeMiddleware>();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
