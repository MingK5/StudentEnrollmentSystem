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
        [Column("password")]  // Matches "password" in SSMS
        public string Password { get; set; } = string.Empty;

        [EmailAddress]
        [Column("primaryEmail")]
        public string? PrimaryEmail { get; set; }  // Nullable

        [EmailAddress]
        [Column("alternativeEmail")]
        public string? AlternativeEmail { get; set; }  // Nullable

        [Column("program")]
        public string? Program { get; set; } = "Unknown";  // Nullable with default

        // Home Address Fields (Nullable)
        [Column("homeAddress")]
        public string? HomeAddress { get; set; }

        [Column("homePostcode")]
        public string? HomePostcode { get; set; }

        [Column("homeCity")]
        public string? HomeCity { get; set; }

        [Column("homeState")]
        public string? HomeState { get; set; }

        [Column("homeCountry")]
        public string? HomeCountry { get; set; }

        // Mailing Address Fields (Nullable)
        [Column("mailAddress")]
        public string? MailAddress { get; set; }

        [Column("mailPostcode")]
        public string? MailPostcode { get; set; }

        [Column("mailCity")]
        public string? MailCity { get; set; }

        [Column("mailState")]
        public string? MailState { get; set; }

        [Column("mailCountry")]
        public string? MailCountry { get; set; }

        // Emergency Contact Fields (Nullable)
        [Column("emergencyContactPerson")]
        public string? EmergencyContactPerson { get; set; }

        [Column("emergencyContactRelationship")]
        public string? EmergencyContactRelationship { get; set; }

        [Column("emergencyContactHp")]
        public string? EmergencyContactHp { get; set; }
    }
}
