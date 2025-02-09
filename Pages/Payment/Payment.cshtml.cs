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
    public class PaymentModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PaymentModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string IdentificationNo { get; set; } = "N/A";
        public string Program { get; set; } = string.Empty;
        public string Session { get; set; } = string.Empty;
        public decimal NetAmount { get; set; } = 0;
        public List<StudentAccount> PendingTransactions { get; set; } = new List<StudentAccount>();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = HttpContext.User;

            // Redirect if not authenticated
            if (user == null || !user.Identity.IsAuthenticated)
            {
                Console.WriteLine("DEBUG: User not authenticated, redirecting to Login.");
                return RedirectToPage("/Login");
            }

            // Fetch user details from claims
            StudentId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            StudentName = user.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
            Program = user.FindFirst("Program")?.Value ?? "Unknown";

            if (string.IsNullOrEmpty(StudentId))
            {
                Console.WriteLine("DEBUG: StudentId is empty, redirecting to Login.");
                return RedirectToPage("/Login");
            }

            // Fetch student information (Identification No)
            var student = await _context.Students
                .Where(s => EF.Functions.Collate(s.StudentId.Trim(), "SQL_Latin1_General_CP1_CI_AS") == StudentId.Trim())
                .Select(s => new { s.StudentName, s.Program, s.IdentificationNo })
                .FirstOrDefaultAsync();

            if (student != null)
            {
                StudentName = student.StudentName ?? "Unknown";
                Program = student.Program ?? "Unknown";
                IdentificationNo = student.IdentificationNo ?? "N/A";
            }

            Console.WriteLine($"DEBUG: Fetching transactions for StudentId: {StudentId}");

            // Fetch all transactions for the student, excluding failed ones
            var allTransactions = await _context.StudentAccounts
                .Where(t => t.StudentId == StudentId && t.Status != "Failed")  // Exclude failed transactions
                .OrderBy(t => t.TransactionDate)
                .ToListAsync();

            if (!allTransactions.Any())
            {
                Console.WriteLine("DEBUG: No transactions found.");
            }
            else
            {
                Console.WriteLine($"DEBUG: Found {allTransactions.Count} transactions.");
                foreach (var transaction in allTransactions)
                {
                    Console.WriteLine($"DEBUG: TransactionId: {transaction.TransactionId}, Date: {transaction.TransactionDate}, Process: {transaction.Process}, Amount: {transaction.Amount}");
                }
            }

            // Identify the latest "Payment" transaction
            var lastPayment = allTransactions.LastOrDefault(t => t.Process == "Payment");

            if (lastPayment != null)
            {
                Console.WriteLine($"DEBUG: Last Payment found - TransactionId: {lastPayment.TransactionId}, Date: {lastPayment.TransactionDate}");
            }
            else
            {
                Console.WriteLine("DEBUG: No previous payment found.");
            }

            // Filter transactions happening AFTER the latest payment, also ensuring they are not failed
            PendingTransactions = lastPayment != null
                ? allTransactions.Where(t => t.TransactionDate > lastPayment.TransactionDate && t.Process != "Payment").ToList()
                : allTransactions.Where(t => t.Process != "Payment").ToList();

            Console.WriteLine($"DEBUG: Pending transactions count: {PendingTransactions.Count}");

            // Compute net amount payable
            NetAmount = PendingTransactions.Sum(t => t.Amount);

            // **Extract the latest session from any transaction (not just Enrol)**
            var latestTransaction = allTransactions.LastOrDefault();
            Session = latestTransaction?.Session ?? "Unknown";

            Console.WriteLine($"DEBUG: NetAmount: {NetAmount}, Latest Session: {Session}");

            return Page();
        }

        public async Task<IActionResult> OnPostPayAsync()
        {
            Console.WriteLine("OnPostPayAsync() triggered");

            if (string.IsNullOrWhiteSpace(StudentId))
            {
                StudentId = Request.Form["StudentId"].ToString().Trim();
                Console.WriteLine($"DEBUG: Retrieved StudentId from form: '{StudentId}'");
            }

            if (string.IsNullOrWhiteSpace(StudentId))
            {
                TempData["ErrorMessage"] = "Error: StudentId is missing.";
                return RedirectToPage();
            }

            var studentExists = await _context.Students
                .AnyAsync(s => EF.Functions.Collate(s.StudentId.Trim(), "SQL_Latin1_General_CP1_CI_AS") == StudentId);

            if (!studentExists)
            {
                TempData["ErrorMessage"] = "Error: Student record does not exist.";
                Console.WriteLine("Error: Student record does not exist");
                return RedirectToPage();
            }

            if (!decimal.TryParse(Request.Form["NetAmount"], out decimal amountDue) || amountDue <= 0)
            {
                TempData["ErrorMessage"] = "No outstanding amount to pay.";
                return RedirectToPage();
            }

            // Fetch the latest ENROL transaction for the session number
            var latestEnrolTransaction = await _context.StudentAccounts
                .Where(t => t.StudentId == StudentId && t.Process == "Enrol")
                .OrderByDescending(t => t.TransactionDate)
                .FirstOrDefaultAsync();

            string sessionNumber = latestEnrolTransaction?.Session ?? "Unknown";

            // Generate new transaction and document numbers
            int newTransactionId = await GenerateNewTransactionId();
            string newDocumentNo = await GenerateNewDocumentNo();

            // Generate new transaction
            var newTransaction = new StudentAccount
            {
                TransactionId = newTransactionId,
                StudentId = StudentId,
                Process = "Payment",
                Particulars = "Tuition Fee Payment",
                DocumentNo = newDocumentNo,
                TransactionDate = DateTime.Now,
                Session = sessionNumber, // Assign session from latest Enrol
                Status = "Approved",
                Message = "Payment has been successfully recorded.",
                Amount = -amountDue // Negative to cancel out the balance
            };

            try
            {
                Console.WriteLine($"Saving transaction {newTransactionId} to database...");
                _context.StudentAccounts.Add(newTransaction);
                await _context.SaveChangesAsync();
                Console.WriteLine("Transaction saved successfully!");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Database error: {ex.Message}";
                Console.WriteLine($"Database error: {ex.Message}");
                return RedirectToPage();
            }

            return RedirectToPage("/Payment/PaymentSuccess");
        }

        public IActionResult OnPostCancel()
        {
            return RedirectToPage("/Main");
        }

        private async Task<int> GenerateNewTransactionId()
        {
            var lastTransaction = await _context.StudentAccounts
                .OrderByDescending(t => t.TransactionId)
                .FirstOrDefaultAsync();

            int newId = 1001; // Default starting value

            if (lastTransaction != null)
            {
                newId = lastTransaction.TransactionId + 1; // Increment latest transaction ID
            }

            return newId;
        }

        private async Task<string> GenerateNewDocumentNo()
        {
            var lastDoc = await _context.StudentAccounts
                .OrderByDescending(t => t.DocumentNo)
                .FirstOrDefaultAsync();

            int newNumber = 1;

            if (lastDoc != null && lastDoc.DocumentNo.StartsWith("DOC"))
            {
                int.TryParse(lastDoc.DocumentNo.Substring(3), out newNumber);
                newNumber++;
            }

            return $"DOC{newNumber:D8}";  // Formats as DOC00000001, DOC00000002, etc.
        }
    }
}
