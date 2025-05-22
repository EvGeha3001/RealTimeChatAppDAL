using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RealTimeChatAppDAL.EF;
using RealTimeChatAppDAL.Models;
using RealTimeChatAppDAL.Repos;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace RealTimeChatApp.Pages
{
    public class ChatModel : PageModel
    {
        private readonly ChatRepo _chatRepo;
        private readonly UserRepo _userRepo;
        private readonly MessageRepo _messageRepo;
        private readonly IHubContext<ChatHub> _chatHub;

        public ChatModel(ChatRepo chatRepo, UserRepo userRepo, MessageRepo messageRepo,IHubContext<ChatHub> chatHub)
        {
            _chatRepo = chatRepo;
            _userRepo = userRepo;
            _chatHub = chatHub;
            _messageRepo = messageRepo;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public IFormFile? UploadImage { get; set; }

        [BindProperty]
        public IFormFile? VoiceMessage { get; set; }

        [BindProperty]
        public int MessageId { get; set; }

        [BindProperty]
        public string? UsernameToAdd { get; set; }

        [BindProperty]
        public string? NewMessage { get; set; }

        public string? AddUserResult { get; set; }

        public User? CurrentUser { get; set; }

        public ChatRoom? ChatRoom { get; set; }

        public List<Message> Messages { get; set; } = new();

        public List<string> ChatUserNames()
        {
            var users = ChatRoom.GroupMembers
                .Where(g => g.RoomId == Id)
                .Select(g => _userRepo.GetUserById(g.UserId).UserName).ToList();
            return users;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            if (!await LoadContextDataAsync())
                return NotFound();

            Messages = await _chatRepo.GetMessagesByRoomId(ChatRoom.Id);

            return Page();
        }
        public async Task<IActionResult> OnPostSendMessageAsync(int id)
        {
            if (!await LoadContextDataAsync())
                return NotFound();

            if (string.IsNullOrWhiteSpace(NewMessage) && UploadImage == null && VoiceMessage == null)
                return RedirectToPage(new { id = Id });


            byte[]? imageData = null;
            string? mimeType = null;

            if (UploadImage != null && UploadImage.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await UploadImage.CopyToAsync(ms);
                    imageData = ms.ToArray();
                    mimeType = UploadImage.ContentType;
                }
            }

            byte[]? audioBytes = null;
            string? contentType = null;

            if (VoiceMessage != null && VoiceMessage.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await VoiceMessage.CopyToAsync(ms);
                    audioBytes = ms.ToArray();
                    contentType = VoiceMessage.ContentType;
                }              
            }

            var message = new Message
            {
                RoomId = ChatRoom.Id,
                UserId = CurrentUser.Id,
                Content = NewMessage,
                SentAt = DateTime.Now,
                ImageData = imageData,
                ImageMimeType = mimeType,
                VoiceContentType = contentType,
                VoiceData = audioBytes,
            };

            await _chatRepo.SendMessage(message);

            var base64Image = imageData != null ? Convert.ToBase64String(imageData) : null;
            var base64Audio = audioBytes != null ? Convert.ToBase64String(audioBytes) : null;

            await _chatHub.Clients.Group($"room_{ChatRoom.Id}")
                .SendAsync("ReceiveMessage", CurrentUser.UserName, NewMessage, message.Id, base64Image, base64Audio, message.SentAt?.ToString("g") ?? "");

            return RedirectToPage(new { id = Id });
        }
        public async Task<IActionResult> OnPostEditMessageAsync(int id)
        {
            if (!await LoadContextDataAsync())
                return NotFound();

            var message = _messageRepo.GetOne(MessageId);

            if (message != null)
            {
                message.Content = NewMessage;
                _messageRepo.Save(message);
                await _chatHub.Clients.Group($"room_{ChatRoom.Id}")
                    .SendAsync("MessageEdited", MessageId, NewMessage);
            }
            return RedirectToPage(new { id = Id });
        }
        public async Task<IActionResult> OnPostDeleteMessageAsync(int messageId)
        {
            if (!await LoadContextDataAsync())
                return NotFound();

            await _chatRepo.DeleteMessage(messageId);
            await _chatHub.Clients.Group($"room_{ChatRoom.Id}")
                .SendAsync("OnDeleteMessage", messageId);

            return RedirectToPage(new { id = Id });
        }
        public async Task<IActionResult> OnPostAddUserAsync()
        {
            ChatRoom = _chatRepo.GetChatRoom(Id);
            if (ChatRoom == null)
                return NotFound();

            var userToAdd = _userRepo.GetUserByUsername(UsernameToAdd);
            if (userToAdd == null)
            {
                AddUserResult = "User not found";
                return await OnGetAsync(); 
            }

            var alreadyInChat = ChatRoom.GroupMembers.Any(gm => gm.UserId == userToAdd.Id);
            if (alreadyInChat)
            {
                AddUserResult = "User is already in chat";
                return await OnGetAsync();
            }

            await _chatRepo.AddUserToRoom(userToAdd.Id, ChatRoom.Id);
            AddUserResult = "User added";

            return await OnGetAsync(); 
        }       
        private async Task<bool> LoadContextDataAsync()
        {
            ChatRoom = _chatRepo.GetChatRoom(Id);
            CurrentUser = _userRepo.GetUserByUsername(
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return ChatRoom != null && CurrentUser != null;
        }


    }
    public class ChatHub : Hub
    {
        private readonly MessageRepo _messageRepo;
        private readonly ChatRepo _chatRepo;
        private readonly UserRepo _userRepo;
        public ChatHub(ChatRepo chatRepo, MessageRepo messageRepo, UserRepo userRepo) 
        { 
            _chatRepo = chatRepo;
            _messageRepo = messageRepo;
            _userRepo = userRepo;
        }
        public async Task JoinRoom(int roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"room_{roomId}");
        }

        public async Task SendMessage(int roomId, string userName, string messageContent, string base64Image, string contentType)
        {
            try
            {
                var user = _userRepo.GetUserByUsername(userName);
                if (user == null)
                    return;

                byte[] imageData = null;
                if (!string.IsNullOrEmpty(base64Image))
                {
                    imageData = Convert.FromBase64String(base64Image);
                }


                var message = new Message
                {
                    RoomId = roomId,
                    UserId = user.Id,
                    Content = messageContent,
                    SentAt = DateTime.Now,
                    ImageData = imageData,
                    ImageMimeType = contentType
                };
                await _chatRepo.SendMessage(message);


                await Clients.Group($"room_{roomId}")
                    .SendAsync("ReceiveMessage", user.UserName, messageContent, message.Id, base64Image, message.SentAt?.ToString("g") ?? "");
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendMessage error: " + ex.Message);
                throw;
            }
            
        }
        public async Task DeleteMessage(int messageId, int roomId)
        {
            await _chatRepo.DeleteMessage(messageId);

            await Clients.Group($"room_{roomId}").SendAsync("OnDeleteMessage", messageId);
            
        }
        public async Task EditMessage(int messageId, string newContent)
        {
            var message = _messageRepo.GetOne(messageId);
            if (message != null)
            {
                message.Content = newContent;
                _messageRepo.Save(message);
                await Clients.All.SendAsync("MessageEdited", messageId, newContent);
            }                 
        }
    }
}