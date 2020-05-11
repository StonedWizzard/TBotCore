using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBotCore.Db
{
    /// <summary>
    /// Interface represent single user log entry in Db userLogs table
    /// </summary>
    interface IUserLogMessage
    {
        /// <summary>
        /// Foreign key in users collection
        /// </summary>
        public int UserDbId { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
    }
}
