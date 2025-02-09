using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem.Pages.API
{
    public class LogoutUserModel : PageModel
    {
        public async Task<IActionResult> OnPostAsync()
        {
            await HttpContext.SignOutAsync();
            return new JsonResult(new { success = true });
        }
    }
}
