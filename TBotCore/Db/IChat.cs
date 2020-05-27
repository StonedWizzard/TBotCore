using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBotCore.Db
{
    public interface IChat
    {
        int Id { get; set; }
        long UserId { get; set; }
        string UserName { get; set; }
    }
}
