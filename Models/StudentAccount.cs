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
        public string Process { get; set; } = string.Empty; // e.g., "Enroll", "Add", "Drop", "Payment"

        [Column("particulars")]
        public string? Particulars { get; set; } // Description of transaction

        [Column("documentNo")]
        public string? DocumentNo { get; set; } // Invoice, Receipt, or Credit Note Number

        [Column("session")]
        public string? Session { get; set; } // Semester Session (e.g., JAN2025)

        [Column("status")]
        public string? Status { get; set; } // e.g., "Approved", "Pending"

        [Column("message")]
        public string? Message { get; set; } // e.g., "Transaction recorded."

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; } // Amount associated with transaction
    }
}
