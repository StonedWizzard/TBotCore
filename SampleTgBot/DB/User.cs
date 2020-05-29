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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public int? UserRoleId { get; set; }
        [ForeignKey("UserRoleId")]
        public IUserRole UserRole { get; set; }

        public int? UserPreferencesId { get; set; }
        [ForeignKey("UserPreferencesId")]
        public IUserPreferences UserPreferences { get; set; }

        public DateTime LastVisit { get; set; }
        [Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime RegisteredDate { get; set; }
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

            result.UserPreferences = new UserPreferences(result.UserId);
            result.UserRole = new UserRole(result.UserId);

            return result;
        }
    }
}
