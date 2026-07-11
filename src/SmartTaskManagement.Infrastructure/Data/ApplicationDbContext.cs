using Microsoft.EntityFrameworkCore;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Domain.Enums;

namespace SmartTaskManagement.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Role).IsRequired().HasConversion<int>();

                // Seed admin user
                entity.HasData(new User
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Email = "admin@smarttask.com",
                    Username = "admin",
                    FirstName = "System",
                    LastName = "Admin",
                    PasswordHash = "AQAAAAIAAYagAAAAEMy1zqjMv7g9Xh6sHv2nwS0jN1CkOxLzRqT3vW5bY2aG8fHg9JkLmNoPqRsTuVwXyZ", // Hashed "Admin@123"
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                });
            });

            // Project Configuration
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.StartDate).IsRequired();

                entity.HasOne(e => e.CreatedByUser)
                    .WithMany(e => e.Projects)
                    .HasForeignKey(e => e.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Task Configuration
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Status).IsRequired().HasConversion<int>();
                entity.Property(e => e.Priority).IsRequired().HasConversion<int>();
                entity.Property(e => e.DueDate).IsRequired();

                entity.HasOne(e => e.Project)
                    .WithMany(e => e.Tasks)
                    .HasForeignKey(e => e.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.AssignedToUser)
                    .WithMany(e => e.AssignedTasks)
                    .HasForeignKey(e => e.AssignedToUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Indexes for performance
                entity.HasIndex(e => e.ProjectId);
                entity.HasIndex(e => e.AssignedToUserId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Priority);
                entity.HasIndex(e => e.DueDate);
            });
        }
    }
}