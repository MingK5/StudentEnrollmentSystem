using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentEnrollmentSystem.Models
{
    [Table("Student")]  // Ensures EF maps to "Student" table in SQL Server
    public class Student
    {
        [Key]
        [Column("studentId")]  // Matches the exact column name in SSMS
        public string StudentId { get; set; } = string.Empty;

        [Required]
        [Column("studentName")]  // Matches "studentName" in SSMS
        public string StudentName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Column("email")]  // Matches "email" in SSMS
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("password")]  // Matches "password" in SSMS
        public string Password { get; set; } = string.Empty;
    }
}
