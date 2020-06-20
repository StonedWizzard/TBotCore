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
    class User : IUser
    {
        public User()
        {
            RegisteredDate = DateTime.Now;
            LastVisit = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }
        [Index(IsUnique = true)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public IUserRole UserRole { get; set; }
        public IUserPreferences UserPreferences { get; set; }
        public IUserAddInfo UserInfo { get; set; }

        public DateTime LastVisit { get; set; }
        [Required]
        public DateTime RegisteredDate { get; set; }
        public int MessagesSend { get; set; }
        public bool IsRegistered { get; set; }

        public void Update(IUser user)
        {
            this.IsRegistered = user.IsRegistered;
            this.LastVisit = user.LastVisit;
            this.Login = user.Login;
            this.MessagesSend = user.MessagesSend;
            this.Password = user.Password;
            this.UserName = user.UserName;
            this.UserPreferences = user.UserPreferences;
            this.UserRole = user.UserRole;
        }

        // create new user instance
        public static User GetNewUser(IUser user)
        {
            User result = new User();

            result.UserId = user.UserId;
            result.UserName = user.UserName;
            result.IsRegistered = false;

            result.UserPreferences = new UserPreferences(result.UserId);
            result.UserRole = new UserRole(result.UserId);

            return result;
        }
    }
}
