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
    class UserPreferences : IUserPreferences
    {
        // set language by default here
        public UserPreferences() { Language = "en"; }

        public UserPreferences(long userId) : this()
        {
            UserId = userId;
        }


        [Key]
        public int Id { get; set; }

        [Index(IsUnique = true)]
        public long UserId { get; set; }

        public string Language { get; set; }
    }
}
