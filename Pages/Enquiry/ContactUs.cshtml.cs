using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem.Pages.Enquiry
{
    public class ContactUsModel : PageModel
    {
        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string Program { get; set; } = string.Empty;


        [BindProperty]
        [Required(ErrorMessage = "Please select a category.")]
        public string Category { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Subject is required.")]
        [StringLength(100, ErrorMessage = "Subject must be less than 100 characters.")]
        public string Subject { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Message is required.")]
        [StringLength(500, ErrorMessage = "Message must be less than 500 characters.")]
        public string Message { get; set; } = string.Empty;

        public string SuccessMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Simulate sending data (In real implementation, save to database or send an email)
                await Task.Delay(500); // Simulating async operation

                SuccessMessage = "Your message has been sent successfully!";
            }
            catch
            {
                ErrorMessage = "An error occurred while sending your message. Please try again.";
                return Page();
            }

            // Ensure a return statement exists for all execution paths
            return Page();
        }


        public IActionResult OnGet()
        {
            var user = HttpContext.User;

            // Redirect to Login if user is not authenticated
            if (user == null || !user.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Login");
            }

            // Fetch user details from Claims
            StudentName = user.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Unknown";
            StudentId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            Program = user.FindFirst("Program")?.Value ?? "Unknown";

            return Page();
        }
    }
}

