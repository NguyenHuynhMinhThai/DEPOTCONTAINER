using DEPOTCONTAINER.Data;
using DEPOTCONTAINER.Repositories;
using DEPOTCONTAINER.Repositories.Interfaces;
using DEPOTCONTAINER.Services;
using DEPOTCONTAINER.Services.Interfaces;
using DEPOTCONTAINER.Validators;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ============ Configuration ============
// Đọc connection string từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "server=localhost;port=3307;database=depotdb;user=root;password=root123";

// ============ DbContext ============
builder.Services.AddDbContext<DepotDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// ============ Dependency Injection - Repository ============
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ============ Dependency Injection - Service ============
// Đăng ký các service với interface (SOLID: Dependency Inversion)
builder.Services.AddScoped<IContainerService, ContainerService>();
builder.Services.AddScoped<IBlockService, BlockService>();
builder.Services.AddScoped<ILineOperatorService, LineOperatorService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IContainerMovementService, ContainerMovementService>();
builder.Services.AddScoped<IReleaseOrderService, ReleaseOrderService>();

// ============ Dependency Injection - Singleton ============
// Singleton pattern: chỉ tạo 1 instance cho toàn bộ vòng đời ứng dụng.
// Dùng factory delegate để trỏ vào Instance property (double-check locking)
// — vừa giữ đúng Singleton Pattern, vừa tương thích với DI của .NET 10
// (DI không thể gọi được private constructor của 2 class này).
builder.Services.AddSingleton<DEPOTCONTAINER.Singletons.DepotConfigManager>(_ => DEPOTCONTAINER.Singletons.DepotConfigManager.Instance);
builder.Services.AddSingleton<DEPOTCONTAINER.Singletons.InMemoryCache>(_ => DEPOTCONTAINER.Singletons.InMemoryCache.Instance);

// ============ MVC + Razor Pages + API ============
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ============ Swagger ============
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "DEPOT Container API",
        Version = "v1",
        Description = "Hệ thống quản lý Depot Container - API endpoints"
    });
});

var app = builder.Build();

// ============ Middleware Pipeline ============
// Thứ tự middleware rất quan trọng trong ASP.NET Core
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Custom Middleware: log mọi request
app.Use(async (context, next) =>
{
    var start = DateTime.UtcNow;
    await next();
    var elapsed = DateTime.UtcNow - start;
    Console.WriteLine($"[Request] {context.Request.Method} {context.Request.Path} - {elapsed.TotalMilliseconds:F0}ms");
});

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

// Tắt HTTPS redirect trong Docker (chỉ dùng HTTP port 8080)
// Khi chạy local bằng `dotnet run`, vẫn dùng HTTPS bình thường
if (!app.Environment.IsDevelopment() || Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
{
    // Trong container - bỏ qua HTTPS redirect
    app.Logger.LogInformation("[Startup] Chạy trong Docker container - bỏ qua HTTPS redirect");
}
else
{
    app.UseHttpsRedirection();
}

// Default route cho Razor Pages
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ============ Auto migrate database khi start ============
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DepotDbContext>();
        context.Database.EnsureCreated();

        // Seed dữ liệu mẫu
        DbSeeder.Seed(context);

        Console.WriteLine("[Startup] Database ready.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Lỗi khi khởi tạo database.");
    }
}

app.Run();