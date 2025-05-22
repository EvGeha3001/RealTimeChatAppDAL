using RealTimeChatAppDAL.Models.Base;
using System;
using System.Collections.Generic;

namespace RealTimeChatAppDAL.Models;

public partial class ChatRoom : EntityBase
{
    public string Name { get; set; } = null!;

    public bool IsGroup { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
