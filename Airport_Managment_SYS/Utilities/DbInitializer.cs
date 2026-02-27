using System;
using System.Linq;
using System.Threading.Tasks;
using Airport_Managment_SYS.DataAccess;
using Airport_Managment_SYS.Models;
using Airport_Managment_SYS.Utilities;
using Airport_Managment_SYS.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Airport_Managment_SYS.Utilities
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbcontext _dbContext;
        private readonly ILogger<DbInitializer> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<GovernerateState> _governerateRepository;
        private readonly IRepository<Airport> _airportRepository;

        public DbInitializer(
            ApplicationDbcontext dbContext,
            ILogger<DbInitializer> logger,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IRepository<Country> countryRepository,
            IRepository<GovernerateState> governerateRepository,
            IRepository<Airport> airportRepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
            _countryRepository = countryRepository;
            _governerateRepository = governerateRepository;
            _airportRepository = airportRepository;
        }

    public async Task InitializeAsync()
    {
            try
            {
                if (_dbContext.Database.GetPendingMigrations().Any())
                {
                    _dbContext.Database.Migrate();
                }

                // Create roles
                if (!_roleManager.Roles.Any())
                {
                    await _roleManager.CreateAsync(new IdentityRole(StaticVariables.SUPER_ADMIN));
                    await _roleManager.CreateAsync(new IdentityRole(StaticVariables.ADMIN));
                    await _roleManager.CreateAsync(new IdentityRole(StaticVariables.USER));
                }

                // Create main users and assign roles if they don't exist
                if (await _userManager.FindByNameAsync("superadmin") is null)
                {
                    var super = new ApplicationUser
                    {
                        UserName = "superadmin",
                        Email = "superadmin@example.com",
                        EmailConfirmed = true,
                        PhoneNumber = "+201055959599",
                        Nationality = null!
                    };
                    await _userManager.CreateAsync(super, "Super@123");
                    await _userManager.AddToRoleAsync(super, StaticVariables.SUPER_ADMIN);
                }

                if (await _userManager.FindByNameAsync("admin") is null)
                {
                    var admin = new ApplicationUser
                    {
                        UserName = "admin",
                        Email = "admin@example.com",
                        EmailConfirmed = true,
                        PhoneNumber = "+201000000000",
                        NationalitiesId = 1
                    };
                    await _userManager.CreateAsync(admin, "Admin@123");
                    await _userManager.AddToRoleAsync(admin, StaticVariables.ADMIN);
                }

                // Seed countries, governorates and airports if not present
                var existingCountries = (await _countryRepository.GetAsync()).ToList();
                if (!existingCountries.Any())
                {
                    // Curated list of popular countries with a few governorates/cities and airports
                    var seed = new List<(Country country, (string gov, string[] airports)[] govs)>
                    {
                        (new Country { Name = "Egypt", MobileCode = "+20" }, new[] {
                            ("Cairo", new[] { "Cairo International Airport" }),
                            ("Alexandria", new[] { "Borg El Arab Airport" })
                        }),
                        (new Country { Name = "United States", MobileCode = "+1" }, new[] {
                            ("New York", new[] { "John F. Kennedy International Airport" }),
                            ("Los Angeles", new[] { "Los Angeles International Airport" })
                        }),
                        (new Country { Name = "United Kingdom", MobileCode = "+44" }, new[] {
                            ("London", new[] { "Heathrow Airport" }),
                            ("Manchester", new[] { "Manchester Airport" })
                        }),
                        (new Country { Name = "United Arab Emirates", MobileCode = "+971" }, new[] {
                            ("Dubai", new[] { "Dubai International Airport" }),
                            ("Abu Dhabi", new[] { "Abu Dhabi International Airport" })
                        }),
                        (new Country { Name = "Saudi Arabia", MobileCode = "+966" }, new[] {
                            ("Riyadh", new[] { "King Khalid International Airport" }),
                            ("Jeddah", new[] { "King Abdulaziz International Airport" })
                        }),
                        (new Country { Name = "Canada", MobileCode = "+1" }, new[] {
                            ("Toronto", new[] { "Toronto Pearson International Airport" }),
                            ("Vancouver", new[] { "Vancouver International Airport" })
                        }),
                        (new Country { Name = "Germany", MobileCode = "+49" }, new[] {
                            ("Frankfurt", new[] { "Frankfurt Airport" }),
                            ("Munich", new[] { "Munich Airport" })
                        }),
                        (new Country { Name = "France", MobileCode = "+33" }, new[] {
                            ("Paris", new[] { "Charles de Gaulle Airport" }),
                            ("Nice", new[] { "Nice Cote d'Azur Airport" })
                        }),
                        (new Country { Name = "India", MobileCode = "+91" }, new[] {
                            ("Delhi", new[] { "Indira Gandhi International Airport" }),
                            ("Mumbai", new[] { "Chhatrapati Shivaji Maharaj International Airport" })
                        }),
                        (new Country { Name = "China", MobileCode = "+86" }, new[] {
                            ("Beijing", new[] { "Beijing Capital International Airport" }),
                            ("Shanghai", new[] { "Shanghai Pudong International Airport" })
                        }),
                        (new Country { Name = "Australia", MobileCode = "+61" }, new[] {
                            ("Sydney", new[] { "Sydney Kingsford Smith Airport" }),
                            ("Melbourne", new[] { "Melbourne Airport" })
                        }),
                    };

                    // Add countries
                    foreach (var entry in seed)
                    {
                        await _countryRepository.AddAsync(entry.country);
                    }
                    await _countryRepository.CommitAsync();

                    // Add governorates and airports per country
                    foreach (var entry in seed)
                    {
                        // Ensure we have the tracked country with Id
                        var country = (await _countryRepository.GetAsync(c => c.Name == entry.country.Name)).FirstOrDefault();
                        if (country == null) continue;

                        foreach (var g in entry.govs)
                        {
                            var gov = new GovernerateState { Name = g.gov, CountryId = country.Id };
                            await _governerateRepository.AddAsync(gov);
                            await _governerateRepository.CommitAsync();

                            // fetch the saved governorate to get Id
                            var savedGov = (await _governerateRepository.GetAsync(x => x.Name == g.gov && x.CountryId == country.Id)).FirstOrDefault();
                            if (savedGov == null) continue;

                            foreach (var apName in g.airports)
                            {
                                var ap = new Airport { Name = apName, GovernerateStateId = savedGov.Id };
                                await _airportRepository.AddAsync(ap);
                            }
                            await _airportRepository.CommitAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing the database");
            }
        }
    }
}
