using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentEnrollmentSystem.Models
{
    [Table("Feedback")]
    public class Feedback
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("enquiryId")]
        public int EnquiryId { get; set; }

        [Column("studentId")]
        public string? StudentId { get; set; }

        [Column("category")]
        public string? Category { get; set; }

        [Column("subject")]
        public string? Subject { get; set; }

        [Column("message")]
        public string? Message { get; set; }

        public DateTime DateSubmitted { get; set; }
    }
}
