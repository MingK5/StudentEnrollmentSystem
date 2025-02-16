using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentEnrollmentSystem.Models
{
    public class StudentUnavailability
    {
        [Key]
        public int StudentUnavailabilityId { get; set; } 

        [ForeignKey("Student")]
        public string StudentId { get; set; } = string.Empty; 

        public string Day { get; set; } = string.Empty;

        public TimeSpan? StartTime { get; set; } 

        public TimeSpan? EndTime { get; set; }

        // Navigation property for Student
        public Student? Student { get; set; }
    }
}
