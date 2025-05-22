using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RealTimeChatAppDAL.Models;
using RealTimeChatAppDAL.Repos;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;

namespace RealTimeChatApp.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly UserRepo _userRepo = new UserRepo();

        public RegisterModel()
        {
            
        }

        [BindProperty]
        public RegisterInput? Input { get; set; }

        public class RegisterInput
        {
            [Required]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (_userRepo.IsUserExist(Input.Email))
            {
                ModelState.AddModelError("", "There is already a user with this email! Try another one.");
                return Page();
            }

            _userRepo.Add(new User
            {
                UserName = Input.UserName,
                Email = Input.Email,
                PasswordHash = HashPassword(Input.Password)
            });

            var user = _userRepo.GetUserByUsername(Input.UserName);

            if (user != null )
            {
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
                ModelState.AddModelError("", "Login or password not match");
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
