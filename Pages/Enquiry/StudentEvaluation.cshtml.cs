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

            string studentId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";

            var enrolledCourses = await _context.Enrolments
                .Where(e => e.StudentId == studentId)
                .Select(e => new { e.EnrolId, e.CourseId, e.DatePerformed })
                .ToListAsync();

            var evaluatedEnrollments = await _context.Evaluations
                .Where(ev => enrolledCourses.Select(e => e.EnrolId).Contains(ev.EnrolId))
                .Select(ev => ev.EnrolId)
                .ToListAsync();

            CourseList = enrolledCourses
                .Where(e => !evaluatedEnrollments.Contains(e.EnrolId))
                .Select(e => new SelectListItem { Value = e.CourseId, Text = e.CourseId })
                .ToList();

            return Page();
        }

        public async Task<JsonResult> OnGetGetCourseDetailsAsync(string courseId)
        {
            var courseData = await _context.Courses
                .Where(c => c.CourseId == courseId &&
                    !_context.Evaluations.Any(e => e.EnrolId ==
                        _context.Enrolments
                        .Where(en => en.CourseId == courseId && en.StudentId == StudentId)
                        .Select(en => en.EnrolId)
                        .FirstOrDefault()))
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
            if (!ModelState.IsValid) return Page();

            var enrolment = await _context.Enrolments
                .FirstOrDefaultAsync(e => e.CourseId == SelectedCourseId);

            if (enrolment != null)
            {
                Evaluation.EnrolId = enrolment.EnrolId;
                _context.Evaluations.Add(Evaluation);
                await _context.SaveChangesAsync();

                return new JsonResult(new { success = true });
            }

            return new JsonResult(new { success = false });
        }
    }
}

