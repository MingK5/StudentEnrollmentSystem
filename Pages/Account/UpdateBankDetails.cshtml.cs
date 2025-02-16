using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace StudentEnrollmentSystem.Pages.Account
{
    public class UpdateBankDetailsModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public UpdateBankDetailsModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public string StudentId { get; set; } = string.Empty;

        [BindProperty]
        public string StudentName { get; set; } = string.Empty;

        [BindProperty]
        public string Program { get; set; } = string.Empty;

        [BindProperty]
        public string BankName { get; set; } = string.Empty;

        [BindProperty]
        public string BankAccount { get; set; } = string.Empty;

        [BindProperty]
        public string BankHolderName { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            var user = HttpContext.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Login");
            }

            StudentName = user.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
            StudentId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            Program = user.FindFirst("Program")?.Value ?? "Unknown";

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT bankName, bankAccount, bankHolderName FROM dbo.Student WHERE studentId = @StudentId";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentId", StudentId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            BankName = reader["bankName"]?.ToString() ?? "";
                            BankAccount = reader["bankAccount"]?.ToString() ?? "";
                            BankHolderName = reader["bankHolderName"]?.ToString() ?? "";
                        }
                    }
                }
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(BankName))
            {
                ViewData["BankErrorMessage"] = "Please select a valid bank.";
                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE dbo.Student SET bankName=@BankName, bankAccount=@BankAccount, bankHolderName=@BankHolderName WHERE studentId=@StudentId";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentId", StudentId);
                    cmd.Parameters.AddWithValue("@BankName", BankName);
                    cmd.Parameters.AddWithValue("@BankAccount", BankAccount);
                    cmd.Parameters.AddWithValue("@BankHolderName", BankHolderName);

                    cmd.ExecuteNonQuery();
                }
            }

            TempData["SuccessMessage"] = "Bank details updated successfully!";
            return RedirectToPage("/Account/UpdateBankDetails");
        }
    }
}
