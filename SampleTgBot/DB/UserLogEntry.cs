using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Db;

namespace SampleTgBot.DB
{
    class UserLogEntry : IUserLogMessage
    {
        public UserLogEntry() { Time = DateTime.Now; }

        public UserLogEntry(long userId, string msg) : this()
        {
            UserId = userId;
            Message = msg;
        }

        public int Id { get; set; }
        public long UserId { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
    }
}
