using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBotCore.Core
{
    /// <summary>
    /// Provides validation method for commands, operations and dialogs
    /// </summary>
    interface IRequestValidate
    {
        /// <summary>
        /// Check if user role allow access to this member
        /// </summary>
        bool ValidateUser(Db.IUserRole role);
    }
}
