using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem.Pages.Payment
{
    public class InvoiceModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public InvoiceModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public StudentAccount Transaction { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string IdentificationNo { get; set; } = string.Empty;
        public string MailingAddress { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(string documentNo)
        {
            if (string.IsNullOrWhiteSpace(documentNo))
            {
                return NotFound();
            }

            // Fetch the transaction from StudentAccount (only for Enrol process)
            Transaction = await _context.StudentAccounts
                .FirstOrDefaultAsync(t => t.DocumentNo == documentNo && t.Process == "Enrol");

            if (Transaction == null)
            {
                return NotFound();
            }

            // Fetch the student's details from the Student table
            var student = await _context.Students
                .Where(s => s.StudentId == Transaction.StudentId)
                .Select(s => new
                {
                    s.StudentName,
                    s.IdentificationNo,
                    s.MailAddress,
                    s.MailPostcode,
                    s.MailCity,
                    s.MailState,
                    s.MailCountry
                })
                .FirstOrDefaultAsync();

            if (student != null)
            {
                StudentName = student.StudentName ?? "Unknown";
                IdentificationNo = student.IdentificationNo ?? "Unknown";

                // Concatenate full mailing address
                MailingAddress = $"{student.MailAddress}, {student.MailPostcode}, {student.MailCity}, {student.MailState}, {student.MailCountry}";
            }

            return Page();
        }
    }
}
