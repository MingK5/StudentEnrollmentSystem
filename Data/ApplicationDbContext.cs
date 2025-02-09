using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Models;

namespace StudentEnrollmentSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Define the tables (DbSets)
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Enrol> Enrolments { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }  
        public DbSet<Course> Courses { get; set; }
        
        public DbSet<StudentAccount> StudentAccounts { get; set; } = null!; // Add StudentAccount table

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Explicitly map tables
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<StudentAccount>().ToTable("StudentAccount");
            modelBuilder.Entity<Evaluation>().ToTable("Evaluation");
        }
    }
}
