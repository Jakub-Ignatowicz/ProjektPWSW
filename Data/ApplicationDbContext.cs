using Microsoft.EntityFrameworkCore;

namespace Projekt.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Tracker> Trackers { get; set; }
        public DbSet<PhisicalActivityTracker> PhysicalActivityTrackers { get; set; }
        public DbSet<SleepTracker> SleepTrackers { get; set; }
        public DbSet<WaterIntakeTracker> WaterIntakeTrackers { get; set; }
        public DbSet<ActivityType> ActivityTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tracker>()
                .HasDiscriminator<string>("TrackerType")
                .HasValue<PhisicalActivityTracker>("PhisicalActivityTracker")
                .HasValue<SleepTracker>("SleepTracker")
                .HasValue<WaterIntakeTracker>("WaterIntakeTracker");

            modelBuilder.Entity<User>()
                .HasMany(u => u.Trackers)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserID);

            modelBuilder.Entity<ActivityType>()
                .HasMany(a => a.PhysicalActivities)
                .WithOne(p => p.ActivityType)
                .HasForeignKey(p => p.ActivityTypeId);

            modelBuilder.Entity<ActivityType>()
                .Property(a => a.Type)
                .HasConversion<string>();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}
