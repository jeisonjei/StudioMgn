using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Radzen;
using StudioMgn.Areas.Identity;
using StudioMgn.Data;
using StudioMgn.Services;
using StudioMgn.Areas.Identity.Data;

namespace StudioMgn
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddLocalization();
            var connectionString = builder.Configuration.GetConnectionString("SqliteConnection") ?? throw new InvalidOperationException("Connection string 'SqliteConnection' not found.");
            //var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            //var connectionString = "Data source=data\\studioMgn.db";
            builder.Services.AddDbContextFactory<ApplicationDbContext>(options=>
                options.UseSqlite(connectionString));
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString),ServiceLifetime.Transient);
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddDefaultIdentity<IdentityUser>(options => {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password = new PasswordOptions
                {
                    RequireDigit = false,
                    RequiredLength = 4,
                    RequiredUniqueChars=0,
                    RequireLowercase=false,
                    RequireNonAlphanumeric=false,
                    RequireUppercase=false
                };
            })
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<AppointmentsService>();
            builder.Services.AddSingleton<EmailService>();
            builder.Services.AddScoped<DbInitializer>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            using (var db=app.Services.GetRequiredService<ApplicationDbContext>())
            {
                db.Database.Migrate();
            }
            using (var scope=app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbInitializer = services.GetRequiredService<DbInitializer>();
                dbInitializer.CreateAdminUserAsync().Wait();
            }
            app.Run();
        }
    }
}