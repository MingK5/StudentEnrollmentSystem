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

                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"[VALIDATION ERROR] {state.Key}: {error.ErrorMessage}");
                    }
                }

                return Page();
            }

            var user = HttpContext.User;
            string studentId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            Console.WriteLine($"[DEBUG] Form submitted with StudentId: '{studentId}'");

            if (string.IsNullOrEmpty(studentId))
            {
                Console.WriteLine("[ERROR] StudentId is NULL in POST request. Form is broken.");
                return RedirectToPage("/Login");
            }

            var existingStudent = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (existingStudent == null)
            {
                Console.WriteLine($"[ERROR] Student with ID {studentId} not found in database!");
                return NotFound();
            }

            // Ensure StudentId, StudentName, and Password are not overwritten with NULL
            Student.StudentId = existingStudent.StudentId;
            Student.StudentName = existingStudent.StudentName; // Retain name
            Student.Password = existingStudent.Password; // Retain password

            Console.WriteLine($"[DEBUG] Retaining Student Name: {Student.StudentName}");
            Console.WriteLine($"[DEBUG] Retaining Password Hash");

            // Update database fields
            existingStudent.HomeAddress = Student.HomeAddress;
            existingStudent.HomePostcode = Student.HomePostcode;
            existingStudent.HomeCity = Student.HomeCity;
            existingStudent.HomeState = Student.HomeState;
            existingStudent.HomeCountry = Student.HomeCountry;

            existingStudent.MailAddress = Student.MailAddress;
            existingStudent.MailPostcode = Student.MailPostcode;
            existingStudent.MailCity = Student.MailCity;
            existingStudent.MailState = Student.MailState;
            existingStudent.MailCountry = Student.MailCountry;

            existingStudent.PrimaryEmail = Student.PrimaryEmail;
            existingStudent.AlternativeEmail = Student.AlternativeEmail;
            existingStudent.EmergencyContactRelationship = Student.EmergencyContactRelationship;
            existingStudent.EmergencyContactPerson = Student.EmergencyContactPerson;
            existingStudent.EmergencyContactHp = Student.EmergencyContactHp;

            // Save changes in database
            _context.Students.Update(existingStudent);
            await _context.SaveChangesAsync();

            Console.WriteLine("[SUCCESS] Profile updated successfully!");

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToPage();
        }

    }
}
