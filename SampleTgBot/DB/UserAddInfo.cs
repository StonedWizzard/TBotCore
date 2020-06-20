using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Db;

namespace SampleTgBot.DB
{
    class UserAddInfo : IUserAddInfo
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public string NickName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
        public bool? Gender { get; set; }

        public void SetFields(Dictionary<string, object> args)
        {
            throw new NotImplementedException();
        }
    }
}
