using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBotCore.Db
{
    /// <summary>
    /// Represents telegram db user.
    /// This interface should be umplemented with db user model
    /// in application for compability.
    /// </summary>
    public interface IUser
    {
        int Id { get; set; }
        int UserId { get; set; }
        string UserName { get; set; }

        string FirstName { get; set; }
        string LastName { get; set; }

        string Phone { get; set; }
        string Mail { get; set; }

        string Login { get; set; }
        string Password { get; set; }
        IUserRole UserRole { get; set; }
        IUserPreferences UserPreferences { get; set; }

        // service info
        DateTime LastVisit { get; set; }
        int MessagesSend { get; set; }
        /// <summary>
        /// Defines is user compleat registration (true) 
        /// or need additional steps (false)
        /// </summary>
        bool IsRegistered { get; set; }

        /// <summary>
        /// Return current user role from Db
        /// </summary>
        IUserRole GetUserRole();

        /// <summary>
        /// Return current user preferences from Db.
        /// </summary>
        IUserPreferences GetUserPreferences();
    }
}
