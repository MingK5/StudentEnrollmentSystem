using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Models;
using System.Security.Claims;
using StudentEnrollmentSystem.Data;

namespace StudentEnrollmentSystem.Pages.AddDrop
{
    public class AddDropHistoryModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public Student Student { get; set; } = new Student();

        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string Program { get; set; } = string.Empty;
        public string Session { get; set; } = string.Empty;

        public List<Course> AvailableCourses { get; set; } = new();

        public List<EnrolledCourseViewModel> EnrolledCourses { get; set; } = new List<EnrolledCourseViewModel>();

        public AddDropHistoryModel(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = HttpContext.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                Console.WriteLine("[ERROR] User is not authenticated. Redirecting to login.");
                return RedirectToPage("/Login");
            }

            StudentId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            StudentName = user.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
            Program = user.FindFirst("Program")?.Value ?? "Unknown";
            Session = _configuration["Session"] ?? string.Empty;

            Student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == StudentId);

            var existingEnrollment = await _context.Enrolments.FirstOrDefaultAsync(e => e.StudentId == StudentId && e.Session == Session);

            if (existingEnrollment == null)
            {
                TempData["ErrorMessage"] = "Please enrol the courses first.";
                return RedirectToPage("/Main"); // Redirect to main page
            }

            EnrolledCourses = _context.Enrolments
                .Where(e => e.StudentId == StudentId && e.Action != "Enrol")
                .Join(
                    _context.Courses,
                    enrol => enrol.CourseId,
                    course => course.CourseId,
                    (enrol, course) => new EnrolledCourseViewModel
                    {
                        CourseId = course.CourseId,
                        CourseName = course.CourseName,
                        Action = enrol.Action,
                        Reason = enrol.Reason,
                        DatePerformed = enrol.DatePerformed
                    })
                .OrderBy(e => e.DatePerformed) 
                .ToList();

            return Page();
        }
    }
}
