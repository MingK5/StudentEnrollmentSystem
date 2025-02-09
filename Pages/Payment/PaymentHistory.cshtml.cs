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
    public class PaymentHistoryModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PaymentHistoryModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string Program { get; set; } = string.Empty;
        public DateTime FromDate { get; set; } = DateTime.Today.AddMonths(-3);
        public DateTime ToDate { get; set; } = DateTime.Today;
        public List<StudentAccount> PaymentTransactions { get; set; } = new List<StudentAccount>();

        public async Task<IActionResult> OnGetAsync(DateTime? from, DateTime? to, string studentId)
        {
            var user = HttpContext.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Login");
            }

            StudentId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            StudentName = user.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
            Program = user.FindFirst("Program")?.Value ?? "Unknown";

            FromDate = from ?? DateTime.Today.AddMonths(-3);
            ToDate = to ?? DateTime.Today;

            PaymentTransactions = await _context.StudentAccounts
                .Where(t => t.StudentId == StudentId && t.Process == "Payment" && t.TransactionDate >= FromDate && t.TransactionDate <= ToDate)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return Page();
        }
    }
}
