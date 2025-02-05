using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using System.Security.Claims;

namespace StudentEnrollmentSystem.Pages
{
    public class MainModel : PageModel
    {
        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string Program { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            var user = HttpContext.User;

            // **Redirect to Login if user is not authenticated**
            if (user == null || !user.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Login");
            }

            // Fetch user details from Claims
            StudentName = user.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
            StudentId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            Program = user.FindFirst("Program")?.Value ?? "Unknown";

            return Page();
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            // **Sign out the user**
            await HttpContext.SignOutAsync();

            // **Clear session**
            HttpContext.Session.Clear();

            // **Prevent caching to block Back button after logout**
            Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "-1"; // Ensure page expires immediately

            return RedirectToPage("/Login");
        }
    }
}
