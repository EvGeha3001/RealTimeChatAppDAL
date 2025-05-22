using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RealTimeChatAppDAL.EF;
using RealTimeChatAppDAL.Models;
using RealTimeChatAppDAL.Repos;
using RealTimeChatAppDAL.Repos.Base;
using System.Reflection;
using System.Security.Claims;

namespace RealTimeChatApp.Pages
{
    public class ChatsModel : PageModel
    {
        private readonly UserRepo _userRepo = new UserRepo();

        public ChatsModel()
        {

        }

        public List<ChatRoom> ChatRooms { get; set; } = new();

        public async Task OnGetAsync()
        {
            var user = _userRepo.GetUserByUsername(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (user != null)
            {
                ChatRooms = await _userRepo.GetChats(user);
            }
        }
    }
}
