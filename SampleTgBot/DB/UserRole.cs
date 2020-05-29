using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Db;

namespace SampleTgBot.DB
{
    class UserRole : IUserRole
    {
        public UserRole() { }

        public UserRole(long userId) : this() { UserId = userId; }


        [Key]
        public int Id { get; set; }

        // only one role per user
        [Index(IsUnique = true)]
        public long UserId { get; set; }
        public int Role { get; set; }
        public string AccessTags { get; set; }

        public List<string> GetAccessTags(char separator = ';')
        {
            return AccessTags?.Trim()
                .Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}
