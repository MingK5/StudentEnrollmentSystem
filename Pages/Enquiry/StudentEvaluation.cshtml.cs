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
            CourseList = new List<SelectListItem>(); // Initialize CourseList
        }

        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string Program { get; set; } = string.Empty;

        public List<SelectListItem> CourseList { get; set; }
        [BindProperty]
        public string SelectedCourseId { get; set; } = string.Empty;

        public Course SelectedCourse { get; set; }
        [BindProperty]
        public Evaluation Evaluation { get; set; } = new();

        public IActionResult OnGet()
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
                CourseList = _context.Enrolments
                    .Where(e => e.StudentId == StudentId &&
                                !_context.Evaluations.Any(ev => ev.StudentId == StudentId && ev.CourseId == e.CourseId))
                    .Select(e => new SelectListItem
                    {
                        Value = e.CourseId,
                        Text = _context.Courses.Where(c => c.CourseId == e.CourseId)
                                    .Select(c => c.CourseName)
                                    .FirstOrDefault() ?? "Unknown"
                    }).ToList();

            }

            return Page();
        }

        public async Task<JsonResult> OnGetGetCourseDetails(string courseId)
        {
            var course = await _context.Courses
                .Where(c => c.CourseId == courseId)
                .Select(c => new { c.CourseId, c.CourseName, c.Credit, c.Lecturer, c.StartTime, c.EndTime, c.Day })
                .FirstOrDefaultAsync();

            return new JsonResult(course);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Evaluation.StudentId = StudentId;
            Evaluation.CourseId = SelectedCourseId;

            _context.Evaluations.Add(Evaluation);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Enquiry/StudentEvaluation");
        }
    }
}
