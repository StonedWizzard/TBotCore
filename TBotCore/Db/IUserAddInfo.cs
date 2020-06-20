using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBotCore.Db
{
    /// <summary>
    /// Additional user info entity
    /// </summary>
    public interface IUserAddInfo
    {
        int Id { get; set; }
        long UserId { get; set; }

        string NickName { get; set; }

        string FirstName { get; set; }
        string LastName { get; set; }

        string Phone { get; set; }
        string Mail { get; set; }

        bool? Gender { get; set; }

        /// <summary>
        /// Read dictionary (wich get from user input context)
        /// and setup fields values
        /// </summary>
        public void SetFields(Dictionary<string, object> args);
    }
}
