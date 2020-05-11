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
    interface IUserPreferences
    {
        string Language { get; set; }
    }
}
