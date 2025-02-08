using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudentEnrollmentSystem.Pages.Payment
{
    public class PaymentSuccessModel : PageModel
    {
        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string Program { get; set; } = string.Empty;

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

