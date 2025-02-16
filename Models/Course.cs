using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentEnrollmentSystem.Models
{
    public class Course
    {
        [Key]
        [Column("courseId")] 
        public string CourseId { get; set; } = string.Empty;

        [Required]
        [Column("courseName")]
        public string CourseName { get; set; } = string.Empty;

        [Column("credit")]
        public int Credit { get; set; }

        [Column("lecturer")]
        public string Lecturer { get; set; } = string.Empty;

        [Column("courseFee")]
        public decimal CourseFee { get; set; }

        [Column("startTime")]
        public TimeSpan? StartTime { get; set; }

        [Column("endTime")]
        public TimeSpan? EndTime { get; set; }

        [Column("day")]
        public string Day { get; set; } = string.Empty;

    }
}
