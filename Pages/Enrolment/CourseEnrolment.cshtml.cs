using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using System.Security.Claims;

namespace StudentEnrollmentSystem.Pages.Enrolment
{
    public class CourseEnrolmentModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public Student Student { get; set; } = new Student();

        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string Program { get; set; } = string.Empty;

        public CourseEnrolmentModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = HttpContext.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                Console.WriteLine("[ERROR] User is not authenticated. Redirecting to login.");
                return RedirectToPage("/Login");
            }

            // Retrieve student info from claims
            StudentId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            StudentName = user.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
            Program = user.FindFirst("Program")?.Value ?? "Unknown";

            Console.WriteLine($"[DEBUG] Retrieved StudentId from claims: '{StudentId}'");

            if (string.IsNullOrEmpty(StudentId))
            {
                Console.WriteLine("[ERROR] StudentId is NULL. Login session might be broken.");
                return RedirectToPage("/Login");
            }

            // Fetch student record
            Student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == StudentId);

            if (Student == null)
            {
                Console.WriteLine("[ERROR] Student not found in database!");
                return NotFound();
            }

            Console.WriteLine("[DEBUG] Successfully loaded student profile.");
            return Page();
        }
    }
}

