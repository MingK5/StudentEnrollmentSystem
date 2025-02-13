using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;

namespace StudentEnrollmentSystem.Pages.Statement
{
    public class RegistrationSummaryModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public RegistrationSummaryModel(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string Program { get; set; } = string.Empty;
        public List<EnrolledCourseViewModel> EnrolledCourses { get; set; } = new List<EnrolledCourseViewModel>();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = HttpContext.User;

            // Redirect to Login if user is not authenticated
            if (user == null || !user.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Login");
            }

            // Fetch user details from Claims
            StudentName = user.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
            StudentId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            Program = user.FindFirst("Program")?.Value ?? "Unknown";
            var Session = _configuration["Session"] ?? string.Empty;

            var lastCourses = await _context.Enrolments
                .Where(e => e.StudentId == StudentId && e.Session == Session)
                .Include(e => e.Course)
                .GroupBy(e => e.CourseId)
                .Select(g => g.OrderByDescending(e => e.DatePerformed).First())
                .ToListAsync();

            EnrolledCourses = lastCourses
                .Where(e => e.StudentId == StudentId && (e.Action == "Enrolled" || e.Action == "Add")) 
                .Join(
                    _context.Courses,
                    enrol => enrol.CourseId,
                    course => course.CourseId,
                    (enrol, course) => new EnrolledCourseViewModel
                    {
                        CourseId = course.CourseId,
                        CourseName = course.CourseName,
                        Lecturer = course.Lecturer,
                        Credit = course.Credit,
                        StartTime = course.StartTime,
                        EndTime = course.EndTime,
                        Day = course.Day
                    })
                .ToList();

            return Page();
        }
    }
}
