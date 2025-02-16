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

        public int GenerateNewEnquiryId()
        {
            var lastFeedback = _context.Feedback.OrderByDescending(f => f.EnquiryId).FirstOrDefault();

            if (lastFeedback == null)
            {
                return 1;
            }
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
                _context.Feedback.Add(newFeedback);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Your message has been successfully sent!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error while saving the feedback message: {ex.Message}";

                if (ex.InnerException != null)
                {
                    TempData["ErrorMessage"] += $" Inner Exception: {ex.InnerException.Message}";
                }

                return RedirectToPage();
            }
        }

    }
}
