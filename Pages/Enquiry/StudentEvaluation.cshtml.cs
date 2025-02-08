using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem.Pages.Enquiry
{
    public class StudentEvaluationModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public StudentEvaluationModel(ApplicationDbContext context)
        {
            _context = context;
            CourseList = new List<SelectListItem>();
            Evaluation = new Evaluation();
        }

        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string Program { get; set; } = string.Empty;
        public List<SelectListItem> CourseList { get; set; }

        [BindProperty]
        public string SelectedCourseId { get; set; } = string.Empty;

        [BindProperty]
        public Evaluation Evaluation { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = HttpContext.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return RedirectToPage("/Login");
            }

            StudentName = user.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Unknown";
            StudentId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            Program = user.FindFirst("Program")?.Value ?? "Unknown";

            if (_context.Enrolments != null && _context.Courses != null && _context.Evaluations != null)
            {
                var enrolledCourses = await _context.Enrolments
                    .Where(e => e.StudentId == StudentId)
                    .Select(e => new
                    {
                        e.EnrolId,
                        e.CourseId,
                        e.DatePerformed
                    })
                    .ToListAsync();

                var evaluatedEnrollments = await _context.Evaluations
                    .Select(ev => ev.EnrolId)
                    .ToListAsync();

                CourseList = enrolledCourses
                    .Where(e => e.CourseId != null && !evaluatedEnrollments.Contains(e.EnrolId))
                    .Select(e => new SelectListItem
                    {
                        Value = e.CourseId,
                        Text = _context.Courses
                            .Where(c => c.CourseId == e.CourseId)
                            .Select(c => c.CourseName)
                            .FirstOrDefault() ?? "Unknown Course"
                    })
                    .ToList();
            }

            return Page();
        }

        public async Task<JsonResult> OnGetGetCourseDetailsAsync(string courseId)
        {
            var courseData = await _context.Courses
                .Where(c => c.CourseId == courseId)
                .FirstOrDefaultAsync();

            if (courseData == null)
            {
                return new JsonResult(null);
            }

            var course = new
            {
                courseData.CourseId,
                courseData.CourseName,
                courseData.Credit,
                courseData.Lecturer,
                CourseFee = courseData.CourseFee.ToString("F2"),  // Convert decimal to string with 2 decimal places
                StartTime = courseData.StartTime.HasValue
                    ? $"{courseData.StartTime.Value.Hours:D2}:{courseData.StartTime.Value.Minutes:D2}"
                    : "N/A",
                EndTime = courseData.EndTime.HasValue
                    ? $"{courseData.EndTime.Value.Hours:D2}:{courseData.EndTime.Value.Minutes:D2}"
                    : "N/A",
                courseData.Day
            };

            return new JsonResult(course);

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, message = "Invalid form data." });
            }

            var user = HttpContext.User;
            StudentId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            if (string.IsNullOrEmpty(StudentId))
            {
                return new JsonResult(new { success = false, message = "User authentication failed. Please log in again." });
            }

            var enrolment = await _context.Enrolments
                .Where(e => e.StudentId == StudentId && e.CourseId == SelectedCourseId)
                .FirstOrDefaultAsync();

            if (enrolment == null)
            {
                return new JsonResult(new { success = false, message = "You are not enrolled in this course." });
            }

            // Ensure evaluation is linked to the correct enrolment
            Evaluation.EnrolId = enrolment.EnrolId;

            try
            {
                _context.Evaluations.Add(Evaluation);
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true, message = "Evaluation submitted successfully." });
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return new JsonResult(new { success = false, message = "An error occurred while saving your evaluation. Please try again." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Unexpected error: {ex.Message}" });
            }
        }

    }
}