using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rd = TBotCore.Config.RawData;

namespace TBotCore.Core.Dialogs
{
    /// <summary>
    /// This kind of dialogs show stored dialogs one by one depending on their sort value
    /// Also activate UserInputContextController wich allow collect responses from user
    /// When last dialog answered - initialize processing of collected data
    /// </summary>
    public class SerialDialog : Dialog, IUserResponseAwaiter
    {
        public SerialDialog(Rd.Dialog dialog, Dialog owner) : base(dialog, owner) { }
    }
}

