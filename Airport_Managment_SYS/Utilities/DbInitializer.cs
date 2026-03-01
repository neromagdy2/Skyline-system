using System;
using System.Linq;
using System.Threading.Tasks;
using Airport_Managment_SYS.DataAccess;
using Airport_Managment_SYS.Models;
using Airport_Managment_SYS.Utilities;
using Airport_Managment_SYS.Repositories;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

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
        private readonly IRepository<Nationalities> _nationalitiesRepository;
        private readonly IRepository<Airport> _airportRepository;
        private readonly IRepository<Airplane> _airplaneRepository;
        private readonly IRepository<Seat> _seatRepository;
        private readonly IRepository<Trip> _tripRepository;
        private readonly IRepository<TripSeat> _tripSeatRepository;
        private readonly IRepository<SeatClass> _seatClassRepository;

        public DbInitializer(
            ApplicationDbcontext dbContext,
            ILogger<DbInitializer> logger,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IRepository<Country> countryRepository,
            IRepository<GovernerateState> governerateRepository,
            IRepository<Nationalities> nationalitiesRepository,
            IRepository<Airport> airportRepository,
            IRepository<Airplane> airplaneRepository,
            IRepository<Seat> seatRepository,
            IRepository<Trip> tripRepository,
            IRepository<TripSeat> tripSeatRepository,
            IRepository<SeatClass> seatClassRepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
            _countryRepository = countryRepository;
            _governerateRepository = governerateRepository;
            _nationalitiesRepository = nationalitiesRepository;
            _airportRepository = airportRepository;
            _airplaneRepository = airplaneRepository;
            _seatRepository = seatRepository;
            _tripRepository = tripRepository;
            _tripSeatRepository = tripSeatRepository;
            _seatClassRepository = seatClassRepository;
        }

        public async Task InitializeAsync()
        {
            bool migrationsApplied = true;

            try
            {
                if (_dbContext.Database.GetPendingMigrations().Any())
                {
                    try
                    {
                        _dbContext.Database.Migrate();
                    }
                    catch (SqlException sqlEx)
                    {
                        // If migration partially fails due to existing objects, log and skip seeding
                        _logger.LogWarning(sqlEx, "Migration failed or partial: {Message}", sqlEx.Message);
                        migrationsApplied = false;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Migration failed: {Message}", ex.Message);
                        migrationsApplied = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check/apply migrations: {Message}", ex.Message);
                migrationsApplied = false;
            }

            if (!migrationsApplied)
            {
                _logger.LogWarning("Migrations did not complete successfully; continuing with seeding anyway.");
                // we deliberately do not return so that seeding can still attempt to run
            }

            try
            {
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
                        NationalitiesId = null
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
                        NationalitiesId = null
                    };
                    await _userManager.CreateAsync(admin, "Admin@123");
                    await _userManager.AddToRoleAsync(admin, StaticVariables.ADMIN);
                }

                // Seed nationalities if not present
                var existingNationalities = (await _nationalitiesRepository.GetAsync()).ToList();
                if (!existingNationalities.Any())
                {
                    var nationalities = new List<Nationalities>
                    {
                        new Nationalities { Name = "Egypt", Code = "EG" },
                        new Nationalities { Name = "United States", Code = "US" },
                        new Nationalities { Name = "United Kingdom", Code = "UK" },
                        new Nationalities { Name = "France", Code = "FR" },
                        new Nationalities { Name = "Germany", Code = "DE" },
                        new Nationalities { Name = "Italy", Code = "IT" },
                        new Nationalities { Name = "Spain", Code = "ES" },
                        new Nationalities { Name = "Canada", Code = "CA" },
                        new Nationalities { Name = "Australia", Code = "AU" },
                        new Nationalities { Name = "Japan", Code = "JP" },
                        new Nationalities { Name = "China", Code = "CN" },
                        new Nationalities { Name = "India", Code = "IN" },
                        new Nationalities { Name = "Brazil", Code = "BR" },
                        new Nationalities { Name = "Saudi Arabia", Code = "SA" },
                        new Nationalities { Name = "United Arab Emirates", Code = "AE" },
                        new Nationalities { Name = "Qatar", Code = "QA" },
                        new Nationalities { Name = "Kuwait", Code = "KW" },
                        new Nationalities { Name = "Jordan", Code = "JO" },
                        new Nationalities { Name = "Lebanon", Code = "LB" },
                        new Nationalities { Name = "Turkey", Code = "TR" },
                        new Nationalities { Name = "South Africa", Code = "ZA" },
                        new Nationalities { Name = "Nigeria", Code = "NG" },
                        new Nationalities { Name = "Kenya", Code = "KE" },
                        new Nationalities { Name = "Morocco", Code = "MA" },
                        new Nationalities { Name = "Algeria", Code = "DZ" },
                        new Nationalities { Name = "Tunisia", Code = "TN" },
                        new Nationalities { Name = "Libya", Code = "LY" },
                        new Nationalities { Name = "Sudan", Code = "SD" },
                        new Nationalities { Name = "Iraq", Code = "IQ" },
                        new Nationalities { Name = "Iran", Code = "IR" },
                        new Nationalities { Name = "Pakistan", Code = "PK" },
                        new Nationalities { Name = "Bangladesh", Code = "BD" },
                        new Nationalities { Name = "Indonesia", Code = "ID" },
                        new Nationalities { Name = "Malaysia", Code = "MY" },
                        new Nationalities { Name = "Singapore", Code = "SG" },
                        new Nationalities { Name = "Thailand", Code = "TH" },
                        new Nationalities { Name = "Philippines", Code = "PH" },
                        new Nationalities { Name = "Argentina", Code = "AR" },
                        new Nationalities { Name = "Chile", Code = "CL" },
                        new Nationalities { Name = "Colombia", Code = "CO" },
                        new Nationalities { Name = "Peru", Code = "PE" },
                        new Nationalities { Name = "Venezuela", Code = "VE" },
                        new Nationalities { Name = "Greece", Code = "GR" },
                        new Nationalities { Name = "Portugal", Code = "PT" },
                        new Nationalities { Name = "Netherlands", Code = "NL" },
                        new Nationalities { Name = "Belgium", Code = "BE" },
                        new Nationalities { Name = "Switzerland", Code = "CH" },
                        new Nationalities { Name = "Austria", Code = "AT" },
                        new Nationalities { Name = "Sweden", Code = "SE" },
                        new Nationalities { Name = "Norway", Code = "NO" },
                        new Nationalities { Name = "Denmark", Code = "DK" },
                        new Nationalities { Name = "Finland", Code = "FI" },
                        new Nationalities { Name = "Poland", Code = "PL" },
                        new Nationalities { Name = "Czech Republic", Code = "CZ" },
                        new Nationalities { Name = "Hungary", Code = "HU" },
                        new Nationalities { Name = "Romania", Code = "RO" },
                        new Nationalities { Name = "Bulgaria", Code = "BG" },
                        new Nationalities { Name = "Croatia", Code = "HR" },
                        new Nationalities { Name = "Serbia", Code = "RS" },
                        new Nationalities { Name = "Ukraine", Code = "UA" },
                        new Nationalities { Name = "Belarus", Code = "BY" },
                        new Nationalities { Name = "New Zealand", Code = "NZ" },
                        new Nationalities { Name = "Israel", Code = "IL" }
                    };

                    foreach (var nationality in nationalities)
                    {
                        await _nationalitiesRepository.AddAsync(nationality);
                        await _nationalitiesRepository.CommitAsync();
                    }
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

                // ensure at least two airports exist so trips can be created
                var existingAirports = (await _airportRepository.GetAsync()).ToList();
                if (existingAirports.Count < 2)
                {
                    _logger.LogInformation("Not enough airports found ({Count}), creating placeholders.", existingAirports.Count);
                    var gov = (await _governerateRepository.GetAsync()).FirstOrDefault();
                    if (gov != null)
                    {
                        await _airportRepository.AddAsync(new Airport { Name = "Placeholder A", GovernerateStateId = gov.Id });
                        await _airportRepository.AddAsync(new Airport { Name = "Placeholder B", GovernerateStateId = gov.Id });
                        await _airportRepository.CommitAsync();
                    }
                    existingAirports = (await _airportRepository.GetAsync()).ToList();
                }

                // always attempt to seed planes, seats, trips (checks just prevent duplicates)
                _logger.LogInformation("Seeding planes and trips...");
                // ensure test planes exist
                var planeA = (await _airplaneRepository.GetAsync(p => p.Name == "TestPlaneA")).FirstOrDefault();
                var planeB = (await _airplaneRepository.GetAsync(p => p.Name == "TestPlaneB")).FirstOrDefault();
                if (planeA == null || planeB == null)
                {
                    if (planeA == null) planeA = new Airplane { Name = "TestPlaneA", Model = "Model-A" };
                    if (planeB == null) planeB = new Airplane { Name = "TestPlaneB", Model = "Model-B" };
                    if (planeA.Id == 0) await _airplaneRepository.AddAsync(planeA);
                    if (planeB.Id == 0) await _airplaneRepository.AddAsync(planeB);
                    await _airplaneRepository.CommitAsync();
                }

                // ensure at least one seat class exists
                if (!(await _seatClassRepository.GetAsync()).Any())
                {
                    await _seatClassRepository.AddAsync(new SeatClass { Name = "Economy" });
                    await _seatClassRepository.CommitAsync();
                }
                var econ = (await _seatClassRepository.GetAsync()).First();

                // ensure plane seats exist for our test planes
                var seatsForA = (await _seatRepository.GetAsync(s => s.AirplaneId == planeA.Id)).ToList();
                if (!seatsForA.Any())
                {
                    var seats = new List<Seat>
                    {
                        new Seat { SeatNumber = 1, Available = true, seatClassId = econ.Id, AirplaneId = planeA.Id },
                        new Seat { SeatNumber = 2, Available = true, seatClassId = econ.Id, AirplaneId = planeA.Id }
                    };
                    foreach (var s in seats) await _seatRepository.AddAsync(s);
                    await _seatRepository.CommitAsync();
                }
                var seatsForB = (await _seatRepository.GetAsync(s => s.AirplaneId == planeB.Id)).ToList();
                if (!seatsForB.Any())
                {
                    var seats = new List<Seat>
                    {
                        new Seat { SeatNumber = 1, Available = true, seatClassId = econ.Id, AirplaneId = planeB.Id },
                        new Seat { SeatNumber = 2, Available = true, seatClassId = econ.Id, AirplaneId = planeB.Id }
                    };
                    foreach (var s in seats) await _seatRepository.AddAsync(s);
                    await _seatRepository.CommitAsync();
                }

                // ensure test trips exist
                var existingTrips = (await _tripRepository.GetAsync(t => t.Price == 199.99f || t.Price == 299.99f)).ToList();
                if (existingTrips.Count < 2)
                {
                    var airports = (await _airportRepository.GetAsync()).ToList();
                    if (airports.Count >= 2)
                    {
                        // recalc plane references in case they were newly created
                        planeA = (await _airplaneRepository.GetAsync(p => p.Name == "TestPlaneA")).First();
                        planeB = (await _airplaneRepository.GetAsync(p => p.Name == "TestPlaneB")).First();

                        var trip1 = new Trip
                        {
                            Price = 199.99f,
                            DateTime = new DateTime(2026, 3, 1, 9, 0, 0),
                            ArrivalDateTime = new DateTime(2026, 3, 1, 12, 0, 0),
                            AirplaneId = planeA.Id,
                            Airport_FromId = airports[1].Id,
                            Airport_ToId = airports[0].Id,
                            IsDeleted = false,
                            TripSeats = new List<TripSeat>()
                        };
                        var trip2 = new Trip
                        {
                            Price = 299.99f,
                            DateTime = new DateTime(2026, 3, 2, 15, 30, 0),
                            ArrivalDateTime = new DateTime(2026, 3, 2, 18, 30, 0),
                            AirplaneId = planeB.Id,
                            Airport_FromId = airports[0].Id,
                            Airport_ToId = airports[1].Id,
                            IsDeleted = false,
                            TripSeats = new List<TripSeat>()
                        };
                        var seatsPlaneA = (await _seatRepository.GetAsync(s => s.AirplaneId == planeA.Id)).ToList();
                        foreach (var s in seatsPlaneA) trip1.TripSeats.Add(new TripSeat { SeatId = s.Id, IsBooked = false });
                        var seatsPlaneB = (await _seatRepository.GetAsync(s => s.AirplaneId == planeB.Id)).ToList();
                        foreach (var s in seatsPlaneB) trip2.TripSeats.Add(new TripSeat { SeatId = s.Id, IsBooked = false });

                        await _tripRepository.AddAsync(trip1);
                        await _tripRepository.AddAsync(trip2);
                        await _tripRepository.CommitAsync();
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
