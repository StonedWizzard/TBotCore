using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Db;

namespace SampleTgBot.DB
{
    class Chat : IChat
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
    }
}
