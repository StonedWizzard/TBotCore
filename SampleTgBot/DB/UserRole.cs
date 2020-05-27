using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Db;

namespace SampleTgBot.DB
{
    class UserRole : IUserRole
    {
        public int Id { get; set; }

        public int UserId { get; set; }
    }
}
