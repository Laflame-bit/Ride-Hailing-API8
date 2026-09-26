using Microsoft.EntityFrameworkCore;
using RideHailingAPI.Domain.Entities;

namespace RideHailingAPI.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<DriverProfile> DriverProfiles { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Ride> Rides { get; set; }
    public DbSet<RideStatusHistory> RideStatusHistories { get; set; }
    public DbSet<EmailOtp> EmailOtps { get; set; }
    public DbSet<PhoneOtp> PhoneOtps { get; set; }
    public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.PhoneNumber)
            .IsUnique();

        modelBuilder.Entity<Vehicle>()
            .HasIndex(v => v.PlateNumber)
            .IsUnique();

        modelBuilder.Entity<Ride>()
            .HasIndex(r => r.RideReference)
            .IsUnique();

        modelBuilder.Entity<DriverProfile>()
            .HasOne(d => d.User)
            .WithOne()
            .HasForeignKey<DriverProfile>(d => d.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DriverProfile>()
            .HasOne(d => d.Vehicle)
            .WithOne(v => v.DriverProfile)
            .HasForeignKey<Vehicle>(v => v.DriverProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ride>()
            .HasOne(r => r.Passenger)
            .WithMany()
            .HasForeignKey(r => r.PassengerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ride>()
            .HasOne(r => r.Driver)
            .WithMany()
            .HasForeignKey(r => r.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RideStatusHistory>()
            .HasOne(h => h.Ride)
            .WithMany()
            .HasForeignKey(h => h.RideId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RideStatusHistory>()
            .HasOne(h => h.ChangedByUser)
            .WithMany()
            .HasForeignKey(h => h.ChangedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmailOtp>()
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PhoneOtp>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PasswordResetOtp>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AuditLog>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ride>()
            .Property(r => r.Status)
            .HasConversion<string>();

        modelBuilder.Entity<RideStatusHistory>()
            .Property(h => h.Status)
            .HasConversion<string>();

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();
    }
}