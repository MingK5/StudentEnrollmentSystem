using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentEnrollmentSystem.Data;

namespace StudentEnrollmentSystem.Pages.Account
{
    public class ChangePasswordModel : PageModel
    {
        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string Program { get; set; } = string.Empty;

        private readonly ApplicationDbContext _context;

        public ChangePasswordModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string ExistingPassword { get; set; } = string.Empty;

        [BindProperty]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        public string ConfirmNewPassword { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(ExistingPassword) || string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmNewPassword))
            {
                ErrorMessage = "All fields are required.";
                return Page();
            }

            if (NewPassword.Length < 8 || NewPassword.Length > 20)
            {
                ErrorMessage = "Password must be between 8 to 20 characters.";
                return Page();
            }

            if (!Regex.IsMatch(NewPassword, @"^(?=.*[A-Za-z])(?=.*\d).+$"))
            {
                ErrorMessage = "Password must contain at least one letter and one digit.";
                return Page();
            }

            if (NewPassword != ConfirmNewPassword)
            {
                ErrorMessage = "New passwords do not match.";
                return Page();
            }

            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(studentId))
            {
                return RedirectToPage("/Login");
            }

            var student = _context.Students.FirstOrDefault(s => s.StudentId == studentId);

            if (student == null)
            {
                ErrorMessage = "User not found.";
                return Page();
            }

            if (!BCrypt.Net.BCrypt.Verify(ExistingPassword, student.Password))
            {
                ErrorMessage = "Existing password is incorrect.";
                return Page();
            }

            student.Password = BCrypt.Net.BCrypt.HashPassword(NewPassword);
            _context.Students.Update(student);
            await _context.SaveChangesAsync();

            SuccessMessage = "Password updated successfully!";
            return Page();
        }

        public IActionResult OnGet()
        {
            var user = HttpContext.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Login");
            }

            StudentName = user.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Unknown";
            StudentId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            Program = user.FindFirst("Program")?.Value ?? "Unknown";

            return Page();
        }
    }
}
