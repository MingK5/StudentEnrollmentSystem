using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Models;

namespace StudentEnrollmentSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Enrol> Enrolments { get; set; } = null!;
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<StudentAccount> StudentAccounts { get; set; } = null!;
        public DbSet<Feedback> Feedback { get; set; } = null!;
        public DbSet<StudentUnavailability> StudentUnavailability { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<StudentAccount>().ToTable("StudentAccount");
            modelBuilder.Entity<Evaluation>().ToTable("Evaluation");
            modelBuilder.Entity<Course>().ToTable("Courses");
            modelBuilder.Entity<Enrol>().ToTable("Enrol");
            modelBuilder.Entity<Feedback>().ToTable("Feedback");
            modelBuilder.Entity<StudentUnavailability>().ToTable("StudentUnavailability");
        }
    }
}
