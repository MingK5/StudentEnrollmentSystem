using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using System.Security.Claims;
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

        // Generate a new enquiryId
        public int GenerateNewEnquiryId()
        {
            // Query the Feedback table to get the last enquiryId
            var lastFeedback = _context.Feedback.OrderByDescending(f => f.EnquiryId).FirstOrDefault();

            // If there is no feedback, start with 1
            if (lastFeedback == null)
            {
                return 1;
            }

            // Otherwise, increment the last enquiryId by 1
            return lastFeedback.EnquiryId + 1;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = HttpContext.User;

            var studentId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            if (string.IsNullOrWhiteSpace(Category) || string.IsNullOrWhiteSpace(Subject) || string.IsNullOrWhiteSpace(Message))
            {
                TempData["ErrorMessage"] = "All fields must be filled out.";
                return RedirectToPage();
            }

            // Create a new Feedback object
            var newFeedback = new Feedback
            {
                EnquiryId = GenerateNewEnquiryId(), 
                StudentId = studentId,
                Category = Category,
                Subject = Subject,
                Message = Message,
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
                // Log the inner exception for more details
                TempData["ErrorMessage"] = $"Error while saving the feedback message: {ex.Message}";

                // Optionally, you can add the inner exception message as well
                if (ex.InnerException != null)
                {
                    TempData["ErrorMessage"] += $" Inner Exception: {ex.InnerException.Message}";
                }

                return RedirectToPage();
            }
        }

    }
}
