using RealTimeChatAppDAL.Models.Base;
using System;
using System.Collections.Generic;

namespace RealTimeChatAppDAL.Models;

public partial class GroupMember : EntityBase
{
    public int UserId { get; set; }

    public int RoomId { get; set; }

    public virtual ChatRoom Room { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
