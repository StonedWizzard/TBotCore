using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Core.Dialogs;
using TBotCore.Db;

namespace TBotCore.Core.Data
{
    class BotExceptionResponse : BotResponse
    {
        /// <summary>
        /// Exception message to put into console
        /// </summary>
        public readonly string Message;
        /// <summary>
        /// Set to show or not exception to user in chat.
        /// Exception message to user is 'Data' field
        /// </summary>
        public readonly bool ShowNotification;

        public BotExceptionResponse(string msg, object data, IUser user, bool show = true) :
            base(data, ResponseType.Exception, user)
        {
            Message = msg;
            ShowNotification = show;
        }

        public BotExceptionResponse(string msg, object data, IUser user, Dialog dialog, bool show = true)
            : base(data, ResponseType.Exception, user, dialog)
        {
            Message = msg;
            ShowNotification = show;
        }
    }
}
