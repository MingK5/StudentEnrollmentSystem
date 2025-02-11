using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using System.Security.Claims;

namespace StudentEnrollmentSystem.Pages.AddDrop
{
    public class CourseAddDropModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public Student Student { get; set; } = new Student();

        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string Program { get; set; } = string.Empty;
        public string Session { get; set; } = string.Empty;

        public List<EnrolledCourseViewModel> AvailableCourses { get; set; } = new List<EnrolledCourseViewModel>();
        public List<EnrolledCourseViewModel> ActiveCourses { get; set; } = new List<EnrolledCourseViewModel>();

        [BindProperty]
        public List<string> SelectedCourseIds { get; set; } = new List<string>();

        [BindProperty]
        public List<string> SelectionReason { get; set; } = new List<string>();
        [BindProperty]
        public List<string> SelectionAction { get; set; } = new List<string>();

        public CourseAddDropModel(ApplicationDbContext context, IConfiguration configuration)
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

            AvailableCourses = _context.Courses
                .Where(course => !_context.Enrolments
                    .Any(enrol => enrol.StudentId == StudentId && enrol.CourseId == course.CourseId))
                .Select(course => new EnrolledCourseViewModel
                {
                    CourseId = course.CourseId,
                    CourseName = course.CourseName,
                    Credit = course.Credit,
                    StartTime = course.StartTime,
                    EndTime = course.EndTime,
                    Day = course.Day
                })
                .ToList();

            var lastCourses = await _context.Enrolments
                .Where(e => e.StudentId == StudentId && e.Session == Session)
                .GroupBy(e => e.CourseId)
                .Select(g => g.OrderByDescending(e => e.DatePerformed).First())
                .ToListAsync(); // Ensure this query executes first

            AvailableCourses = _context.Courses
                .AsEnumerable() // Switch to client-side processing
                .Where(course =>
                    !lastCourses.Any(e => e.CourseId == course.CourseId) // Never enrolled
                    || lastCourses.Any(e => e.CourseId == course.CourseId && e.Action == "Drop") // Last action was "Drop"
                )
                .Select(course => new EnrolledCourseViewModel
                {
                    CourseId = course.CourseId,
                    CourseName = course.CourseName,
                    Credit = course.Credit,
                    StartTime = course.StartTime,
                    EndTime = course.EndTime,
                    Day = course.Day
                })
                .ToList();

            ActiveCourses = lastCourses
                .Where(e => e.StudentId == StudentId && (e.Action =="Enrol" || e.Action == "Add")) // Corrected StudentId type
                .Join(
                    _context.Courses,
                    enrol => enrol.CourseId,
                    course => course.CourseId,
                    (enrol, course) => new EnrolledCourseViewModel
                    {
                        CourseId = course.CourseId,
                        CourseName = course.CourseName,
                        Credit = course.Credit,
                        StartTime = course.StartTime,
                        EndTime = course.EndTime,
                        Day = course.Day
                    })
                .ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {            
            if (string.IsNullOrWhiteSpace(StudentId))
            {
                StudentId = Request.Form["StudentId"].ToString().Trim();
            }
            int newEnrollmentId = await GenerateNewEnrolId();
            decimal debitAmt = 0;
            decimal creditAmt = 0;

            for (int i = 0; i < SelectedCourseIds.Count; i++)
            {
                var enrolment = new Enrol
                {
                    EnrolId = newEnrollmentId,
                    StudentId = StudentId,
                    CourseId = SelectedCourseIds[i],
                    Session = _configuration["Session"],
                    DatePerformed = DateTime.UtcNow,
                    Action = SelectionAction[i],
                    Reason = SelectionReason[i],
                    Status = "Approved"
                };

                _context.Enrolments.Add(enrolment);
                newEnrollmentId += 1;
            }


            var courseIdsToAdd = SelectedCourseIds
                .Where((id, index) => SelectionAction[index].Contains("Add"))
                .ToList();

            var addCourses = await _context.Courses
                .Where(c => courseIdsToAdd.Contains(c.CourseId))
                .ToListAsync();

            var courseIdsToDrop = SelectedCourseIds
                .Where((id, index) => SelectionAction[index].Contains("Add"))
                .ToList();

            var dropCourses = await _context.Courses
                .Where(c => courseIdsToAdd.Contains(c.CourseId))
                .ToListAsync();

            if (addCourses.Any())
            {
                int newTransactionId = await GenerateNewTransactionId(true);
                string newDocumentNo = await GenerateNewDocumentNo(true);
                debitAmt = addCourses.Sum(c => c.CourseFee);

                var transaction = new StudentAccount
                {
                    TransactionId = newTransactionId,
                    TransactionDate = DateTime.UtcNow,
                    StudentId = StudentId,
                    Process = "Add",
                    Particulars = "Tuition Fee",
                    DocumentNo = newDocumentNo,
                    Session = _configuration["Session"],
                    Status = "Approved",
                    Message = "Transaction is recorded.",
                    Amount = debitAmt,
                };

                _context.StudentAccounts.Add(transaction);
            }

            if (dropCourses.Any())
            {
                int newTransactionId = await GenerateNewTransactionId(!addCourses.Any());
                string newDocumentNo = await GenerateNewDocumentNo(!addCourses.Any());
                creditAmt = addCourses.Sum(c => c.CourseFee);

                var transaction = new StudentAccount
                {
                    TransactionId = newTransactionId,
                    TransactionDate = DateTime.UtcNow,
                    StudentId = StudentId,
                    Process = "Drop",
                    Particulars = "Drop Courses",
                    DocumentNo = newDocumentNo,
                    Session = _configuration["Session"],
                    Status = "Approved",
                    Message = "Transaction is recorded.",
                    Amount = -debitAmt,
                };

                _context.StudentAccounts.Add(transaction);
            }

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

        private async Task<int> GenerateNewTransactionId(bool a)
        {
            var lastTransaction = await _context.StudentAccounts
                .OrderByDescending(t => t.TransactionId)
                .FirstOrDefaultAsync();

            int newId = 1001;

            if (lastTransaction != null)
            {
                newId = a ? lastTransaction.TransactionId + 1 : lastTransaction.TransactionId + 2;
            }

            return newId;
        }

        private async Task<string> GenerateNewDocumentNo(bool a)
        {
            var lastDoc = await _context.StudentAccounts
                .OrderByDescending(t => t.DocumentNo)
                .FirstOrDefaultAsync();

            int newNumber = 1;

            if (lastDoc != null && lastDoc.DocumentNo.StartsWith("DOC"))
            {
                if (int.TryParse(lastDoc.DocumentNo.Substring(3), out int lastNumber))
                {
                    newNumber = lastNumber + (a ? 1 : 2);
                };
            }

            return $"DOC{newNumber:D8}";
        }
    }
}

