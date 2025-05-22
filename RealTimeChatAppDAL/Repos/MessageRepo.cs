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
    public class MessageRepo : BaseRepo<Message>
    {
    }
}
