using Airport_Managment_SYS.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;


namespace MoviesApp.Utilities
{
    public class DbInitializer : IDbInitializer
    {


        private readonly ApplicationDbcontext dbContext;
        private readonly ILogger logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        public DbInitializer(ApplicationDbcontext dbContext, ILogger logger, RoleManager<IdentityRole> roleManager)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            _roleManager = roleManager;
        }

        public void Initialize()
        {

            try
            {
                if (dbContext.Database.GetPendingMigrations().Any())
                {

                    dbContext.Database.Migrate();

                }
                if (!_roleManager.Roles.Any())
                {

                    _roleManager.CreateAsync(new(StaticVariables.SUPER_ADMIN)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(StaticVariables.ADMIN)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(StaticVariables.USER)).GetAwaiter().GetResult();



                }
                _userManager.CreateAsync(new ApplicationUser()
                {
                    UserName = "nerooo",
                    
                    EmailConfirmed = true,
                    PhoneNumber = "01055959599"
                }, "Nero14@").GetAwaiter().GetResult();
                var user = _userManager.FindByNameAsync("nerooo").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user,StaticVariables.SUPER_ADMIN).GetAwaiter().GetResult();
            }


            catch (Exception ex)
            {
                logger.LogError(ex.Message);

            }

        }
    }
}
