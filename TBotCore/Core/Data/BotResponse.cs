using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Db;

namespace TBotCore.Core.Data
{
    /// <summary>
    /// Response from dialog, operation and etc.
    /// </summary>
    public class BotResponse
    {
        public readonly ResponseType Type;
        public readonly object Data;
        public readonly IUser User;
        public readonly Dialogs.Dialog Dialog;

        public BotResponse(object data, ResponseType type, IUser user)
        {
            if (type == ResponseType.Exception)
                throw new ArgumentException("Response type can't be 'Exception'! Use BotResponseException instead", "type");

            Data = data;
            Type = type;
            User = user;
            Dialog = null;
        }

        public BotResponse(object data, ResponseType type, IUser user, Dialogs.Dialog dialog) :
            this(data, type, user)
        {
            Dialog = dialog;
        }

        /// <summary>
        /// Create empty response
        /// </summary>
        public BotResponse(IUser user)
        {
            Data = null;
            Type = ResponseType.Null;
            User = user;
            Dialog = null;
        }

        /// <summary>
        /// Return type of any bot operation
        /// </summary>
        public enum ResponseType
        {
            Null = 0x0,             // not defined
            Text,                   // just some text to show
            Data,                   // serialized data to parse
            Dialog,                 // redirects to dialog
            Exception,              // return exception message
        }

        /// <summary>
        /// Checks if struct type not defined
        /// </summary>
        public bool IsNull() { return Type == ResponseType.Null; }
        /// <summary>
        /// Checks if struct type not defined
        /// </summary>
        public bool IsException() { return Type == ResponseType.Exception; }
    }
}
