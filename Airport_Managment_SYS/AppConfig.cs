using Airport_Managment_SYS.DataAccess;
using Airport_Managment_SYS.Models;
using Airport_Managment_SYS.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace Airport_Managment_SYS
{
    public static class AppConfiguration
    {

        public static void Config(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationDbcontext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireNonAlphanumeric = false;
                options.SignIn.RequireConfirmedEmail = true;
            })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbcontext>();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });
            services.AddTransient<IEmailSender, EmailSender>();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });



            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IDbInitializer, DbInitializer>();

        }
    }
}
