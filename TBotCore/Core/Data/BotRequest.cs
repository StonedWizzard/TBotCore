using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Db;

namespace TBotCore.Core.Data
{
    /// <summary>
    /// User input, button click or other request from user.
    /// </summary>
    struct BotRequest
    {
        private static readonly char[] DataSeparator = new char[] { ':' };

        public readonly RequestType Type;
        public readonly string Data;
        public readonly IUser User;
        public readonly Telegram.Bot.Types.Message Msg;

        public BotRequest(string data, RequestType type, IUser user)
        {
            Type = type;
            Data = data;
            User = user;
            Msg = null;
        }

        public BotRequest(string data, RequestType type, IUser user, Telegram.Bot.Types.Message msg) :
            this(data, type, user)
        {
            Msg = msg;
        }

        /// <summary>
        /// Converts Data string to operation apropriate args
        /// </summary>
        public (string, string) ToOperationArgument()
        {
            if(Data.Contains(":") && Data.Count(x => x == ':') == 1)
            {
                var subStrings = Data.Split(DataSeparator, StringSplitOptions.RemoveEmptyEntries);
                string key, value;
                try
                {
                    key = subStrings[0];
                    value = subStrings[1];
                    return (key, value);
                }
                catch { return (null, null); }
            }
            return (null, null);
        }

        /// <summary>
        /// Type of input request
        /// </summary>
        public enum RequestType
        {
            Null = 0x0,
            TextMessage,
            Command,
            CallbackData,
        }

        /// <summary>
        /// Checks if struct type not defined
        /// </summary>
        public bool IsNull() { return Type == RequestType.Null; }
    }
}
