using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem.Pages.Account
{
    public class UpdateProfileModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public Student Student { get; set; } = new Student();

        public string StudentName { get; private set; } = string.Empty;
        public string StudentId { get; private set; } = string.Empty;
        public string Program { get; private set; } = string.Empty;

        public UpdateProfileModel(ApplicationDbContext context)
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("[ERROR] Model state is invalid!");
                string errorMessages = "<strong>Please correct the following errors:</strong><ul>";

                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"[VALIDATION ERROR] {state.Key}: {error.ErrorMessage}");
                        errorMessages += $"<li>{error.ErrorMessage}</li>";
                    }
                }
                errorMessages += "</ul>";

                // Store error messages in TempData to be shown via Bootstrap alert
                TempData["ErrorMessage"] = errorMessages;
                return Page(); // Stop submission if validation fails
            }

            Console.WriteLine("[DEBUG] No validation errors. Processing form...");

            var user = HttpContext.User;
            string studentId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            if (string.IsNullOrEmpty(studentId))
            {
                Console.WriteLine("[ERROR] StudentId is NULL in POST request.");
                return RedirectToPage("/Login");
            }

            var existingStudent = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
            if (existingStudent == null)
            {
                Console.WriteLine($"[ERROR] Student with ID {studentId} not found in database!");
                return NotFound();
            }

            // Retain key student information
            existingStudent.PrimaryEmail = Student.PrimaryEmail;
            existingStudent.AlternativeEmail = Student.AlternativeEmail;
            existingStudent.PhoneNo = Student.PhoneNo;
            existingStudent.MobileNo = Student.MobileNo;
            existingStudent.EmergencyContactHp = Student.EmergencyContactHp;

            // Save changes
            _context.Students.Update(existingStudent);
            await _context.SaveChangesAsync();

            Console.WriteLine("[SUCCESS] Profile updated successfully!");
            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToPage();
        }
    }
}
