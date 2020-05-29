using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBotCore.Db
{
    /// <summary>
    /// Account settings for user
    /// </summary>
    public interface IUserPreferences
    {
        public int Id { get; set; }

        public long UserId { get; set; }
        string Language { get; set; }
    }
}
