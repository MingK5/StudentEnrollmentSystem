using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentEnrollmentSystem.Models
{
    [Table("Enrol")]
    public class Enrol
    {
        [Key]
        [Column("enrolId")]
        public int EnrolId { get; set; }

        [Column("session")]
        public string Session { get; set; } = string.Empty;

        [Column("studentId")]
        public string StudentId { get; set; } = string.Empty;

        [Column("courseId")]
        public string CourseId { get; set; } = string.Empty;

        [Column("reason")]
        public string? Reason { get; set; }

        [Column("action")]
        public string Action { get; set; } = string.Empty;

        [Column("status")]
        public string Status { get; set; } = string.Empty;

        [Column("datePerformed")]
        public DateTime? DatePerformed { get; set; }
        public Student? Student { get; set; }
        public Course? Course { get; set; }
    }
}
