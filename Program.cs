using Microsoft.EntityFrameworkCore;
using PW.Models;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// HttpClient
// ========================================

builder.Services.AddHttpClient();

// ========================================
// MVC
// ========================================

builder.Services
    .AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// ========================================
// SQL Server
// ========================================

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connection =
        builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseMySql(
        connection,
        ServerVersion.AutoDetect(connection));
});

// ========================================
// Session
// ========================================

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);

    options.Cookie.HttpOnly = true;

    options.Cookie.IsEssential = true;

    options.Cookie.Name = "MentalHealthAI.Session";
});

var app = builder.Build();

// ========================================
// Configure HTTP Pipeline
// ========================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// ========================================
// Session
// ========================================

app.UseSession();

// ========================================
// Authorization
// ========================================

app.UseAuthorization();

// ========================================
// Areas
// ========================================

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

// ========================================
// Default Route
// ========================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();