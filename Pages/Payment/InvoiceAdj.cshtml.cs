using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem.Pages.Payment
{
    public class InvoiceAdjModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public InvoiceAdjModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string Program { get; set; } = string.Empty;
        public List<string> AvailableSessions { get; set; } = new List<string>();
        public string SelectedSession { get; set; } = string.Empty;
        public List<StatementTransaction> StatementTransactions { get; set; } = new List<StatementTransaction>();

        public async Task<IActionResult> OnGetAsync(string session)
        {
            var user = HttpContext.User;

            // Redirect if user is not authenticated
            if (user == null || !user.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Login");
            }

            // Fetch user details from claims
            StudentId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            StudentName = user.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
            Program = user.FindFirst("Program")?.Value ?? "Unknown";

            if (string.IsNullOrEmpty(StudentId))
            {
                return RedirectToPage("/Login");
            }

            // Fetch available sessions for the student
            AvailableSessions = await _context.StudentAccounts
                .Where(t => t.StudentId == StudentId)
                .Select(t => t.Session)
                .Distinct()
                .OrderByDescending(s => s)
                .ToListAsync();

            // Default to latest session if none is selected
            SelectedSession = session ?? AvailableSessions.FirstOrDefault() ?? "Unknown";

            if (SelectedSession == "Unknown")
            {
                return Page();
            }

            // Fetch only Approved transactions related to the selected session
            StatementTransactions = await _context.StudentAccounts
                .Where(t => t.StudentId == StudentId &&
                            t.Session == SelectedSession &&
                            (t.Process == "Enrol" || t.Process == "Add" || t.Process == "Drop") && // Exclude "Payment"
                            t.Status == "Approved") // Only Approved transactions
                .Select(t => new StatementTransaction
                {
                    DocumentType = t.Process == "Enrol" ? "Invoice" :
                                   t.Process == "Add" ? "Debit Note" :
                                   t.Process == "Drop" ? "Credit Note" : "Unknown",
                    Process = t.Process,
                    Particulars = t.Particulars,
                    DocumentNo = t.DocumentNo,
                    TransactionDate = t.TransactionDate,
                    Amount = t.Amount
                })
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return Page();
        }

        public class StatementTransaction
        {
            public string DocumentType { get; set; } = string.Empty;
            public string Process { get; set; } = string.Empty;
            public string Particulars { get; set; } = string.Empty;
            public string DocumentNo { get; set; } = string.Empty;
            public DateTime TransactionDate { get; set; }
            public decimal Amount { get; set; }
        }
    }
}
