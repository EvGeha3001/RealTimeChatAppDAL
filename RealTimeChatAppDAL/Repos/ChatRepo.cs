using Microsoft.AspNetCore.Http;
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
    public class ChatRepo : BaseRepo<ChatRoom>
    {
        public int AddChat(string name, bool isGroup, User sender, User receiver)
        {
            var chatRoom = new ChatRoom
            {
                Name = name,
                IsGroup = isGroup,
                CreatedAt = DateTime.Now,
                GroupMembers = new List<GroupMember>
                {
                    new GroupMember { UserId = sender.Id },
                    new GroupMember { UserId = receiver.Id }
                }
            };

            Context.ChatRooms.Add(chatRoom);
            return Context.SaveChanges();
        }
        public ChatRoom GetChatRoom(int id) 
            => Context.ChatRooms.Include(c => c.GroupMembers).FirstOrDefault(x => x.Id == id);

        public async Task<List<Message>> GetMessagesByRoomId(int id)
        {
            var messages = await Context.Messages
                .Where(m => m.RoomId == id)
                .OrderBy(m => m.SentAt)
                .Include(m => m.User)
                .ToListAsync();
            return messages;
        }
        public async Task<int> SendMessage(int userId, int roomId, string content)
        {
            var message = new Message
            {
                UserId = userId,
                RoomId = roomId,
                Content = content,
                SentAt = DateTime.Now,
            };            
            Context.Messages.Add(message);
            return await Context.SaveChangesAsync();
        }
        public async Task<int> SendMessage(Message message)
        {
            if (message != null)
            {
                Context.Messages.Add(message);
            }
            return await Context.SaveChangesAsync();
        }
        public async Task<int> SendMessage(int userId, int roomId, string content, byte[]? imageData = null, string? mimeType = null)
        {
            var message = new Message
            {
                RoomId = roomId,
                UserId = userId,
                Content = content,
                SentAt = DateTime.Now,
                ImageData = imageData,
                ImageMimeType = mimeType
            };

            Context.Messages.Add(message);
            return await Context.SaveChangesAsync();
        }
        public async Task<int> AddUserToRoom(int userId, int chatId)
        {
            var chatRoom = GetChatRoom(chatId);
            chatRoom.GroupMembers.Add(new GroupMember { UserId = userId });
            return await Context.SaveChangesAsync();
        }
        public async Task<int> DeleteMessage(int messageId)
        {
            var message = Context.Messages.FirstOrDefault(m => m.Id == messageId);
            Context.Messages.Remove(message);
            return await Context.SaveChangesAsync();
        }
        
    }
}
