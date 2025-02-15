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

namespace StudentEnrollmentSystem.Pages.Statement
{
    public class StudentStatementModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public StudentStatementModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<StudentAccount> Transactions { get; set; } = new();

        public async Task<IActionResult> OnGetFilterTransactionsAsync(string dateFrom, string dateTo)
        {
            var user = HttpContext.User;

            var studentId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            if (string.IsNullOrEmpty(dateFrom) || string.IsNullOrEmpty(dateTo))
            {
                return new JsonResult(new List<StudentAccount>());
            }

            DateTime startDate = DateTime.Parse(dateFrom);
            DateTime endDate = DateTime.Parse(dateTo).AddDays(1).AddSeconds(-1); // Extend to end of the day

            var transactions = await _context.StudentAccounts
                .Where(s => s.StudentId== studentId && s.Status == "Approved" && s.TransactionDate >= startDate && s.TransactionDate <= endDate)
                .OrderBy(s => s.TransactionDate)
                .ToListAsync();

            return new JsonResult(transactions);
        }
    }
}
