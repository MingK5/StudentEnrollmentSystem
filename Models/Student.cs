using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentEnrollmentSystem.Models
{
    [Table("Student")] 
    public class Student
    {
        [Key]
        [Column("studentId")]
        public string StudentId { get; set; } = string.Empty;

        [Column("identificationNo")]
        public string? IdentificationNo { get; set; }

        [Required]
        [Column("password")]
        public string Password { get; set; } = string.Empty;

        [Column("emergencyContactPerson")]
        public string? EmergencyContactPerson { get; set; }

        [Column("emergencyContactRelationship")]
        public string? EmergencyContactRelationship { get; set; }

        [RegularExpression(@"^\d{10,15}$", ErrorMessage = "Enter a valid emergency contact number with country code (e.g., 60164915163)")]
        [Column("emergencyContactHp")]
        public string? EmergencyContactHp { get; set; }

        [Column("bankName")]
        public string? BankName { get; set; }

        [Column("bankAccount")]
        public string? BankAccount { get; set; }

        [Column("bankHolderName")]
        public string? BankHolderName { get; set; }

        [Column("studyMode")]
        public string? StudyMode { get; set; }

        [Column("school")]
        public string? School { get; set; } 

        [Column("level")]
        public string? Level { get; set; }

        [Column("program")]
        public string? Program { get; set; } = "Unknown";

        [Required]
        [Column("studentName")]
        public string StudentName { get; set; } = string.Empty;

        [Column("homeAddress")]
        public string? HomeAddress { get; set; }

        [Column("homePostcode")]
        public string? HomePostcode { get; set; }

        [Column("homeCity")]
        public string? HomeCity { get; set; }

        [Column("homeState")]
        public string? HomeState { get; set; }

        [Column("homeCountry")]
        public string? HomeCountry { get; set; }

        [Column("mailAddress")]
        public string? MailAddress { get; set; }

        [Column("mailPostcode")]
        public string? MailPostcode { get; set; }

        [Column("mailCity")]
        public string? MailCity { get; set; }

        [Column("mailState")]
        public string? MailState { get; set; }

        [Column("mailCountry")]
        public string? MailCountry { get; set; }

        [EmailAddress]
        [Column("primaryEmail")]
        public string? PrimaryEmail { get; set; }

        [EmailAddress]
        [Column("alternativeEmail")]
        public string? AlternativeEmail { get; set; }

        [RegularExpression(@"^\d{10,15}$", ErrorMessage = "Enter a valid phone number with country code (e.g., 60164915163)")]
        [Column("phoneNo")]
        public string? PhoneNo { get; set; }

        [RegularExpression(@"^\d{10,15}$", ErrorMessage = "Enter a valid mobile number with country code (e.g., 60164915163)")]
        [Column("mobileNo")]
        public string? MobileNo { get; set; }
        public ICollection<StudentUnavailability>? StudentUnavailabilities { get; set; }
    }
}
