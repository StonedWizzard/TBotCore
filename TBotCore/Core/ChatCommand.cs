using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Db;

namespace TBotCore.Core
{
    /// <summary>
    /// Represent single chat command
    /// </summary>
    class ChatCommand : IRequestValidate
    {
        public virtual bool ValidateUser(IUserRole role)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Defines where the command can be called
        /// </summary>
        [Flags]
        public enum CommandAccessability
        {
            PublicChats = 0x2,
            UserChat = 0x4,
            Anywhere = PublicChats & UserChat,
        }
    }
}
