using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity;

namespace Airport_Managment_SYS.DataAccess
{
    public class ApplicationDbcontext : IdentityDbContext
    {
        public ApplicationDbcontext(DbContextOptions<ApplicationDbcontext> options)
        : base(options) { }

        DbSet<Payment> Payments { get; set; }
        DbSet<Trip> Trips { get; set; }
        DbSet<ApplicationUser> ApplicationUsers { get; set; }
        DbSet<Airport> airports { get; set; }   
        DbSet<Airplane> Airplanes { get; set; }
        DbSet<Country > Countrys { get; set; }
        DbSet<GovernerateState> GoverneratesStates { get; set; }
        DbSet<Reservation> Reservations { get; set; }
        DbSet<SeatClass> SeatClasses { get; set; }
        DbSet<Seat > Seats { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
         



    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

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

            // Disable cascade on SeatId
            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Seat)
                .WithMany()
                .HasForeignKey(t => t.SeatId)
                .OnDelete(DeleteBehavior.NoAction);

            // If the error persists, do the same for AirplaneId or Airports
            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Airplane)
                .WithMany()
                .HasForeignKey(t => t.AirplaneId)
                .OnDelete(DeleteBehavior.NoAction);
        }



    }
}
