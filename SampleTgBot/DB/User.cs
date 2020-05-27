using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TBotCore.Db;
namespace SampleTgBot.DB
{
    class User : IUser
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public IUserRole UserRole { get; set; }
        public IUserPreferences UserPreferences { get; set; }

        public DateTime LastVisit { get; set; }
        public int MessagesSend { get; set; }
        public bool IsRegistered { get; set; }

        public IUserPreferences GetUserPreferences()
        {
            using AppDbContext dbContext = new AppDbContext();
            return dbContext.UserPreferences.FirstOrDefault(x => x.UserId == this.UserId);
        }

        public IUserRole GetUserRole()
        {
            using AppDbContext dbContext = new AppDbContext();
            return dbContext.UserRoles.FirstOrDefault(x => x.UserId == this.UserId);
        }

        public void Update(IUser user)
        {
            this.FirstName = user.FirstName;
            this.IsRegistered = user.IsRegistered;
            this.LastName = user.LastName;
            this.LastVisit = user.LastVisit;
            this.Login = user.Login;
            this.Mail = user.Mail;
            this.MessagesSend = user.MessagesSend;
            this.Password = user.Password;
            this.Phone = user.Phone;
            this.UserName = user.UserName;
            this.UserPreferences = user.UserPreferences;
            this.UserRole = user.UserRole;
        }

        // create new user instance
        public static User GetNewUser(IUser user)
        {
            User result = new User();

            result.UserId = user.UserId;
            result.LastName = user.LastName;
            result.FirstName = user.FirstName;
            result.UserName = user.UserName;
            result.IsRegistered = false;

            result.LastVisit = DateTime.Now;

            return result;
        }
    }
}
