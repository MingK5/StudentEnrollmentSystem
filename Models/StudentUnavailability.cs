using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentEnrollmentSystem.Models
{
    public class StudentUnavailability
    {
        [Key]
        public int StudentUnavailabilityId { get; set; } // Primary Key

        [ForeignKey("Student")]
        public string StudentId { get; set; } = string.Empty; // Correct Foreign Key referencing Student

        public string Day { get; set; } = string.Empty; // Stores day of unavailability

        public TimeSpan? StartTime { get; set; } // Start time of unavailability

        public TimeSpan? EndTime { get; set; } // End time of unavailability

        // Navigation property for Student
        public Student? Student { get; set; }
    }
}
