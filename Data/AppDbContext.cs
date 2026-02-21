using Airport_Managment.Models;
using Microsoft.EntityFrameworkCore;

namespace Airport_Managment.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Trip> Trips => Set<Trip>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(user => user.FullName).HasMaxLength(100).IsRequired();
            entity.Property(user => user.Email).HasMaxLength(120).IsRequired();
        });

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.Property(trip => trip.Route).HasMaxLength(150).IsRequired();
            entity.Property(trip => trip.SeatPrice).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.Property(reservation => reservation.TotalPrice).HasColumnType("decimal(18,2)");

            entity.HasOne(reservation => reservation.User)
                .WithMany(user => user.Reservations)
                .HasForeignKey(reservation => reservation.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(reservation => reservation.Trip)
                .WithMany(trip => trip.Reservations)
                .HasForeignKey(reservation => reservation.TripId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
