# AuthModule

Project **Authentication Module** bằng ASP.NET Core MVC, kèm **xUnit test project**.  
Hỗ trợ chức năng đăng ký, đăng nhập, logout và RememberMe cookie.

---

## 1. Yêu cầu

- .NET SDK 7.0+  
- MySQL (5.7+ hoặc 8.0+)  
- Visual Studio 2022 hoặc VS Code  

---

## 2. Cài đặt

1. Restore packages:

```bash
dotnet restore

```
2. Cấu hình MySQL trong appsettings.json:
```bash
"ConnectionStrings": {
  "DefaultConnection": "server=localhost;port=3306;database=AuthModuleDb;user=root;password=YourPassword;"
}
```
3. Migration & Database
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```
4. Chạy project MVC
```bash
dotnet run --project AuthModule
```
5. Chạy xUnit Tests
```bash
dotnet test AuthModule.Tests
