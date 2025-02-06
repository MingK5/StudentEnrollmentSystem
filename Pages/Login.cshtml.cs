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
        public string StudentId { get; set; } = string.Empty; // Prevents null warnings

        [BindProperty]
        public string Password { get; set; } = string.Empty; // Prevents null warnings

        public string ErrorMessage { get; set; } = string.Empty; // Prevents null warnings

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(StudentId) || string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Student ID and Password are required.";
                return Page();
            }

            var student = _context.Students.FirstOrDefault(s => s.StudentId == StudentId);

            // Ensure student exists and check hashed password
            if (student == null || !BCrypt.Net.BCrypt.Verify(Password, student.Password))
            {
                ErrorMessage = "Invalid Student ID or Password.";
                return Page();
            }

            // **Retrieve program from the database**
            string studentProgram = student.Program ?? "Unknown";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, student.StudentName),
                new Claim(ClaimTypes.NameIdentifier, student.StudentId),
                new Claim("Program", student.Program)  // **Include Program in Claims**
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
