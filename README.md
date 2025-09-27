# AuthModule

Project **Authentication Module** bằng ASP.NET Core MVC, kèm **xUnit test project**.


## 1. Yêu cầu

- .NET SDK 7.0+  
- MySQL (5.7+ hoặc 8.0+)  
- Visual Studio 2022 hoặc VS Code  


## 2. Cài đặt và cấu hình

### 2.1 Restore packages

```bash
dotnet restore
2.2 Cấu hình MySQL
Mở file appsettings.json trong project MVC và chỉnh connection string:

"ConnectionStrings": {
  "DefaultConnection": "server=localhost;port=3306;database=AuthModuleDb;user=root;password=YourPassword;"
}

3. Migration & Database
3.1 Tạo migration
    dotnet ef migrations add InitialCreate
3.2 Áp dụng migration

    dotnet ef database update

4. Chạy project MVC

    dotnet run --project AuthModule

Chức năng: đăng ký, đăng nhập, logout, session và cookie RememberMe.

5. Chạy xUnit Tests

dotnet test AuthModule.Tests
Chạy tất cả test trong project test.

