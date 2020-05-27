using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Db;

namespace TBotCore.Core
{
    /// <summary>
    /// Contains and provide execution of manual chat commands.
    /// Also stores some support methods
    /// </summary>
    public class ChatCommandsProvider
    {
        /// <summary>
        /// Returns true and command in case if string correct, 
        /// user registered and has rights to call it
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public (bool, ChatCommand) GetCommand(string data, IUser user)
        {
            return (true, null);
        }
    }
}
