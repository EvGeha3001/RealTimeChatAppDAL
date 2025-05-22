using RealTimeChatAppDAL.Models.Base;
using System;
using System.Collections.Generic;

namespace RealTimeChatAppDAL.Models;

public partial class Message : EntityBase
{
    public int UserId { get; set; }

    public int RoomId { get; set; }

    public string? Content { get; set; } 

    public byte[]? ImageData { get; set; }

    public string? ImageMimeType { get; set; }

    public byte[]? VoiceData { get; set; }

    public string? VoiceContentType { get; set; }

    public DateTime? SentAt { get; set; }

    public virtual ChatRoom Room { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
