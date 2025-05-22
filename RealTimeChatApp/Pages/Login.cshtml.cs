using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RealTimeChatAppDAL.Models;
using RealTimeChatAppDAL.Repos;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;

namespace RealTimeChatApp.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UserRepo _userRepo = new UserRepo();

        public LoginModel()
        {

        }

        [BindProperty]
        public LoginInput Input { get; set; }

        public class LoginInput
        {

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = _userRepo.GetUserByEmail(Input.Email);

            if (user != null)
            {
                if (HashPassword(Input.Password) != user.PasswordHash)
                {
                    ModelState.AddModelError("", "Login or password don't match");
                    return Page();
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", principal, new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(14)
                });

                return RedirectToPage("/Index");
            }
            else
            {
                ModelState.AddModelError("", "Login or password don't match");
                return Page();
            }
        }
        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
