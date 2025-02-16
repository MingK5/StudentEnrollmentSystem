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
        private readonly IConfiguration _configuration;

        [BindProperty]
        public Student Student { get; set; } = new Student();

        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string Program { get; set; } = string.Empty;
        public string Session { get; set; } = string.Empty;

        public List<Course> AvailableCourses { get; set; } = new();

        [BindProperty]
        public List<string> SelectedCourseIds { get; set; } = new();

        public CourseEnrolmentModel(ApplicationDbContext context, IConfiguration configuration)
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

            if (existingEnrollment != null)
            {
                TempData["ErrorMessage"] = "You are already enrolled in this session!";
                return RedirectToPage("/Main"); 
            }

            AvailableCourses = await _context.Courses
                .Select(c => new Course
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    Lecturer = c.Lecturer,
                    CourseFee = c.CourseFee,
                    Day = c.Day,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime
                }).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            if (string.IsNullOrWhiteSpace(StudentId))
            {
                StudentId = Request.Form["StudentId"].ToString().Trim();
            }
            int newEnrollmentId = await GenerateNewEnrolId();
            int newTransactionId = await GenerateNewTransactionId();
            string newDocumentNo = await GenerateNewDocumentNo();
            decimal totalAmt = 0;

            foreach (var courseId in SelectedCourseIds)
            {
                var enrolment = new Enrol
                {
                    EnrolId = newEnrollmentId,
                    StudentId = StudentId,
                    CourseId = courseId,
                    Session = _configuration["Session"],
                    DatePerformed = DateTime.UtcNow,
                    Action = "Enrol",
                    Reason = null,
                    Status = "Approved"
                };

                _context.Enrolments.Add(enrolment);
                newEnrollmentId += 1;
            }

            var selectedCourses = await _context.Courses
            .Where(c => SelectedCourseIds.Contains(c.CourseId))
            .ToListAsync();

            if (selectedCourses.Any())
            {
                totalAmt = selectedCourses.Sum(c => c.CourseFee);
            }

            var transaction = new StudentAccount
            {
                TransactionId = newTransactionId,
                TransactionDate = DateTime.UtcNow,
                StudentId = StudentId,
                Process = "Enrol",
                Particulars = "Tuition Fee",
                DocumentNo = newDocumentNo,
                Session = _configuration["Session"],
                Status = "Approved",
                Message = "Transaction is recorded.",
                Amount = totalAmt,
            };


            _context.StudentAccounts.Add(transaction);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Enrollment successful!";
            return RedirectToPage("/Main");
        }

        private async Task<int> GenerateNewEnrolId()
        {
            var lastTransaction = await _context.Enrolments
                .OrderByDescending(t => t.EnrolId)
                .FirstOrDefaultAsync();

            return lastTransaction != null ? lastTransaction.EnrolId + 1 : 1;
        }

        private async Task<int> GenerateNewTransactionId()
        {
            var lastTransaction = await _context.StudentAccounts
                .OrderByDescending(t => t.TransactionId)
                .FirstOrDefaultAsync();

            int newId = 1001; 

            if (lastTransaction != null)
            {
                newId = lastTransaction.TransactionId + 1;
            }

            return newId;
        }

        private async Task<string> GenerateNewDocumentNo()
        {
            var lastDoc = await _context.StudentAccounts
                .OrderByDescending(t => t.DocumentNo)
                .FirstOrDefaultAsync();

            int newNumber = 1;

            if (lastDoc != null && lastDoc.DocumentNo.StartsWith("DOC"))
            {
                int.TryParse(lastDoc.DocumentNo.Substring(3), out newNumber);
                newNumber++;
            }

            return $"DOC{newNumber:D8}"; 
        }
    }
}

