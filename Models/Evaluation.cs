using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentEnrollmentSystem.Models
{
    [Table("Evaluation")]
    public class Evaluation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("evaluationId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EvaluationId { get; set; }

        [Column("organizationRate")]
        [Range(1, 5)]
        public int OrganizationRate { get; set; }

        [Column("clarityRate")]
        [Range(1, 5)]
        public int ClarityRate { get; set; }

        [Column("materialRate")]
        [Range(1, 5)]
        public int MaterialRate { get; set; }

        [Column("comment")]
        public string? Comment { get; set; }

        [Column("enrolId")]
        public int EnrolId { get; set; }
    }
}
