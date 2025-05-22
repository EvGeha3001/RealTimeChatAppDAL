using RealTimeChatAppDAL.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealTimeChatAppDAL.Models;

[Table("Users")]
public partial class User : EntityBase
{
    public string? FirstName { get; set; }

    public string? DisplayName { get; set; }

    public string UserName { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public string? SecondName { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
