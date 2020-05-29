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
    public interface IUserLogMessage
    {
        public int Id { get; set; }
        /// <summary>
        /// Foreign key in users collection
        /// </summary>
        public long UserId { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
    }
}
