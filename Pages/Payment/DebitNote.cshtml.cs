using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem.Pages.Payment
{
    public class DebitNoteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DebitNoteModel(ApplicationDbContext context)
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

            Transaction = await _context.StudentAccounts
                .FirstOrDefaultAsync(t => t.DocumentNo == documentNo && t.Process == "Add");

            if (Transaction == null)
            {
                return NotFound();
            }

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

                MailingAddress = $"{student.MailAddress}, {student.MailPostcode}, {student.MailCity}, {student.MailState}, {student.MailCountry}";
            }

            return Page();
        }
    }
}
