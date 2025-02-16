using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentEnrollmentSystem.Models
{
    [Table("StudentAccount")]
    public class StudentAccount
    {
        [Key]
        [Column("transactionId")]
        public int TransactionId { get; set; }

        [Required]
        [Column("transactionDate")]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Required]
        [Column("studentId")]
        public string StudentId { get; set; } = string.Empty;

        [Required]
        [Column("process")]
        public string Process { get; set; } = string.Empty; 

        [Column("particulars")]
        public string? Particulars { get; set; } 

        [Column("documentNo")]
        public string? DocumentNo { get; set; } 

        [Column("session")]
        public string? Session { get; set; } 

        [Column("status")]
        public string? Status { get; set; } 

        [Column("message")]
        public string? Message { get; set; } 

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; } 
    }
}
