using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using BCrypt.Net;

namespace StudentEnrollmentSystem.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public LoginModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string StudentId { get; set; } = string.Empty; 

        [BindProperty]
        public string Password { get; set; } = string.Empty; 

        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(StudentId) || string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Student ID and Password are required.";
                return Page();
            }

            var student = _context.Students.FirstOrDefault(s => s.StudentId == StudentId);

            if (student == null)
            {
                ErrorMessage = "Invalid Student ID or Password.";
                return Page();
            }

            if (!BCrypt.Net.BCrypt.Verify(Password, student.Password))
            {
                ErrorMessage = "Invalid Student ID or Password.";
                return Page();
            }

            string studentName = student.StudentName ?? "Unknown";
            string studentProgram = student.Program ?? "Unknown";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, studentName),
                new Claim(ClaimTypes.NameIdentifier, student.StudentId),
                new Claim("Program", studentProgram) 
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToPage("/Main");
        }
    }
}
