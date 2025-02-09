using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem.Pages.Payment
{
    public class ReceiptModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ReceiptModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public StudentAccount Transaction { get; set; }
        public string StudentName { get; set; } = string.Empty;  // Fetch student name separately

        public async Task<IActionResult> OnGetAsync(string documentNo)
        {
            if (string.IsNullOrWhiteSpace(documentNo))
            {
                return NotFound();
            }

            // Fetch the transaction details
            Transaction = await _context.StudentAccounts
                .FirstOrDefaultAsync(t => t.DocumentNo == documentNo);

            if (Transaction == null)
            {
                return NotFound();
            }

            // Fetch the student's name from the Student table
            var student = await _context.Students
                .Where(s => s.StudentId == Transaction.StudentId)
                .Select(s => new { s.StudentName })
                .FirstOrDefaultAsync();

            StudentName = student?.StudentName ?? "Unknown";

            return Page();
        }
    }
}
