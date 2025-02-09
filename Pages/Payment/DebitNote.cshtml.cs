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

        public async Task<IActionResult> OnGetAsync(string documentNo)
        {
            if (string.IsNullOrWhiteSpace(documentNo))
            {
                return NotFound();
            }

            // Fetch the transaction from StudentAccount (only for Enrol process)
            Transaction = await _context.StudentAccounts
                .FirstOrDefaultAsync(t => t.DocumentNo == documentNo && t.Process == "Add");

            if (Transaction == null)
            {
                return NotFound();
            }

            // Fetch Student Name from the Students table
            var student = await _context.Students
                .Where(s => s.StudentId == Transaction.StudentId)
                .Select(s => s.StudentName)
                .FirstOrDefaultAsync();

            StudentName = student ?? "Unknown";

            return Page();
        }
    }
}
