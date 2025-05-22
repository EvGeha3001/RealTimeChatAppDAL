using Microsoft.EntityFrameworkCore;
using RealTimeChatAppDAL.Models;
using RealTimeChatAppDAL.Repos.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatAppDAL.Repos
{
    [NotMapped]
    public class UserRepo : BaseRepo<User>
    {
        public bool IsUserExist(string email)
        {
            var user = Context.Users.FirstOrDefault(u => u.Email == email);
            return user != null;
        }
        public User GetUserByUsername(string username) 
            => Context.Users.FirstOrDefault(u => u.UserName == username);

        public User GetUserById(int id) 
            => Context.Users.FirstOrDefault(u => u.Id == id);

        public User GetUserByEmail(string email) 
            => Context.Users.FirstOrDefault(u => u.Email == email);

        public async Task<List<ChatRoom>> GetChats(User user)
        {
            var chats = await Context.GroupMembers
                .Where(g => g.UserId == user.Id)
                .Include(g => g.User)
                .Select(g => g.Room)
                .ToListAsync();
            return chats;
        }
    }
}
