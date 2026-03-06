using Airport_Managment_SYS.Utilities;
using Stripe;
using Microsoft.Extensions.Logging;


namespace Airport_Managment_SYS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var connectionString =builder.Configuration.GetConnectionString("DefaultConnection")
                                    ?? throw new InvalidOperationException("Connection string"
                                    + "'DefaultConnection' not found.");

            //AppConfiguration.Config(builder.Services, connectionString);

            builder.Services.Config(connectionString);

            // Configure Stripe (must be done before app.Build())
            builder.Services.Configure<StripeSittings>(builder.Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            var app = builder.Build();

            // Run DB initializer (migrations + seeding) at startup
            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                    // Run synchronously on startup; this will apply migrations and seed data
                    initializer.InitializeAsync().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    // If initialization fails, log and continue to allow diagnostics
                    var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
                    logger?.LogError(ex, "Database initializer failed");
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
