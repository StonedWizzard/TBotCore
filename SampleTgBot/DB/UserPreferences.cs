using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Db;

namespace SampleTgBot.DB
{
    class UserPreferences : IUserPreferences
    {
        public int Id { get; set; }

        public string Language { get; set; }
        public int UserId { get; set; }
    }
}
