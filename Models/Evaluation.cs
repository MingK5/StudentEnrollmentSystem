using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentEnrollmentSystem.Models
{
    [Table("Evaluation")]
    public class Evaluation
    {
        [Key]
        [Column("evaluationId")]
        public int EvaluationId { get; set; }

        [Column("studentId")]
        public string StudentId { get; set; } = string.Empty;

        [Column("courseId")]
        public string CourseId { get; set; } = string.Empty;

        [Column("organizationRate")]
        [Range(1, 5)]
        public int OrganizationRate { get; set; }  

        [Column("clarityRate")]
        [Range(1, 5)]
        public int ClarityRate { get; set; }  

        [Column("materialRate")]
        [Range(1, 5)]
        public int MaterialRate { get; set; }  

        [Column("comments")]
        public string? Comments { get; set; }
    }
}
