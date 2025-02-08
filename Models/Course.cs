using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentEnrollmentSystem.Models
{
    public class Course
    {
        [Key]
        [Column("courseId")]  // Matches exact column name in SSMS
        public string CourseId { get; set; } = string.Empty;

        [Required]
        [Column("courseName")]
        public string CourseName { get; set; } = string.Empty;

        [Column("credit")]
        public int Credit { get; set; }

        [Column("lecturer")]
        public string Lecturer { get; set; } = string.Empty;

        [Column("courseFee")]
        public string CourseFee { get; set; } = string.Empty;

        [Column("startTime")]
        public string StartTime { get; set; } = string.Empty;

        [Column("endTime")]
        public string EndTime { get; set; } = string.Empty;

        [Column("day")]
        public string Day { get; set; } = string.Empty;
    }
}
