using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RealTimeChatAppDAL.EF;
using RealTimeChatAppDAL.Models;
using RealTimeChatAppDAL.Repos;
using System.Security.Claims;

public class AddChatModel : PageModel
{
    private readonly ChatRepo _chatRepo = new ChatRepo();
    private readonly UserRepo _userRepo = new UserRepo();

    public AddChatModel()
    {
        
    }

    [BindProperty]
    public string Name { get; set; }

    [BindProperty]
    public bool IsGroup { get; set; }

    [BindProperty]
    public string UserName { get; set; }

    public void OnGet()
    {
        
    }

    public IActionResult OnPost()
    {
        var sender = _userRepo.GetUserByUsername(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        if (!ModelState.IsValid || sender == null)
        {
            return Page();
        }

        var receiver = _userRepo.GetUserByUsername(UserName);
        if (receiver == null)
        {
            return Page();
        }

        _chatRepo.AddChat(Name, IsGroup, sender, receiver);

        return RedirectToPage("/Index");      
    }
}    
    

