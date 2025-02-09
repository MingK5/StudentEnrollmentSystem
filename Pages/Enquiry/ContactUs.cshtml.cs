using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem.Pages.ContactUs
{
    public class ContactUsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        // Properties to bind form input
        [BindProperty]
        public string Category { get; set; }
        [BindProperty]
        public string Subject { get; set; }
        [BindProperty]
        public string Message { get; set; }

        public ContactUsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(Category) || string.IsNullOrWhiteSpace(Subject) || string.IsNullOrWhiteSpace(Message))
            {
                TempData["ErrorMessage"] = "All fields must be filled out.";
                return RedirectToPage();
            }

            // Create a new Feedback object
            var newFeedback = new Feedback
            {
                Category = Category,
                Subject = Subject,
                Message = Message,
                DateSubmitted = DateTime.Now
            };

            try
            {
                // Save the feedback message to the database
                _context.Feedback.Add(newFeedback);
                await _context.SaveChangesAsync();

                // Success: Redirect with a success message
                TempData["SuccessMessage"] = "Your message has been successfully sent!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                // Error: Handle any database errors
                TempData["ErrorMessage"] = $"Error while saving the feedback message: {ex.Message}";
                return RedirectToPage();
            }
        }
    }
}
