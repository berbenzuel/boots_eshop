using System.Security.Claims;
using BootEshop.Models;
using BootEshop.Services;
using Database;
using Database.Entities;
using DatabaseManager.Services.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BootEshop
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Configuration.AddYamlFile("appsettings.yaml", optional: false, reloadOnChange: true);

            // Database config
            var dbCfg = builder.Configuration.GetSection("appConfig:database").Get<DatabaseConfig>()
                        ?? throw new ArgumentNullException("appsettings.yaml", "Database configuration not found");

            builder.Services.AddDbContext<EshopContext>(o => { o.UseMySQL(dbCfg.ConnectionString); });

            // Identity setup with roles
            builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.SignIn.RequireConfirmedAccount = false;
                })
                .AddRoles<IdentityRole<Guid>>() // crucial for role claims
                .AddEntityFrameworkStores<EshopContext>()
                .AddDefaultTokenProviders();



            // App config & custom services
            builder.Services.Configure<AppConfig>(
                builder.Configuration.GetSection("appConfig"));
            builder.Services.AddEshopServices();

            // Cookie configuration
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });
            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;

                // important for dev on http
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });
            
            var app = builder.Build();

            
            // Error handling
            if (!app.Environment.IsDevelopment())
                app.UseExceptionHandler("/Home/Error");

            app.UseStaticFiles();
            
            app.UseRouting();
            app.UseSession();
            // Authentication & Authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();

            // Seed roles and admin user
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                if (!await roleManager.RoleExistsAsync("Admin"))
                    await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
                if (!await roleManager.RoleExistsAsync("User"))
                    await roleManager.CreateAsync(new IdentityRole<Guid>("User"));

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var adminEmail = "admin@admin";
                var adminPassword = "123456Ab";

                var admin = await userManager.FindByEmailAsync(adminEmail);
                if (admin == null)
                {
                    admin = new User
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(admin, adminPassword);
                    if (result.Succeeded)
                        await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
            
            // Map MVC controllers
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}