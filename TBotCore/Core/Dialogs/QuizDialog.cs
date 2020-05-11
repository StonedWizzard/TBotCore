using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rd = TBotCore.Config.RawData;

namespace TBotCore.Core.Dialogs
{
    /// <summary>
    /// Show content of dialog and await user response by buttons or keyboard.
    /// Process response.
    /// </summary>
    class QuizDialog : Dialog, IUserResponseAwaiter
    {
        

        public QuizDialog(Rd.Dialog dialog, Dialog owner) : base(dialog, owner) 
        {
            Operation = null;
        }
    }
}
