using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity;

namespace Airport_Managment_SYS.DataAccess
{
    public class ApplicationDbcontext : IdentityDbContext
    {
        public ApplicationDbcontext(DbContextOptions<ApplicationDbcontext> options)
        : base(options) { }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Airport> airports { get; set; }   
        public DbSet<Airplane> Airplanes { get; set; }
        public DbSet<Country > Countrys { get; set; }
        public DbSet<GovernerateState> GoverneratesStates { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<SeatClass> SeatClasses { get; set; }
        public DbSet<Seat > Seats { get; set; }
        public DbSet<PlaneSeats > planeSeats { get; set; }
        public DbSet<Nationalities> Nationalities { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
         



    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            // Configure ApplicationUser-Nationalities relationship
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.National)
                .WithMany()
                .HasForeignKey(u => u.NationalitiesId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure global query filter for soft delete
            modelBuilder.Entity<Payment>().HasQueryFilter(p => !p.IsDeleted);

            // Configure Origin Airport
            modelBuilder.Entity<Reservation>().HasKey(r => new { r.TripId , r.ApplicationUserId });

            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Airport_From)
                .WithMany()
                .HasForeignKey(t => t.Airport_FromId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Airport_To)
                .WithMany()
                .HasForeignKey(t => t.Airport_ToId)
                .OnDelete(DeleteBehavior.Restrict);


            // If the error persists, do the same for AirplaneId or Airports
            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Airplane)
                .WithMany()
                .HasForeignKey(t => t.AirplaneId)
                .OnDelete(DeleteBehavior.NoAction);
        }



    }
}
