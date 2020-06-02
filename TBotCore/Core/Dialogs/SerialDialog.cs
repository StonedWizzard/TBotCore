using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Core.Data;
using TBotCore.Db;
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

        /// <summary>
        /// Return next dialog in serial list
        /// </summary>
        public virtual Dialog Next(Dialog currentDialog)
        {
            // if no any dialogs - return this instance
            if (Dialogs.Count == 0) return this;
            else
            {
                int indx = Dialogs.IndexOf(currentDialog) + 1;
                return Dialogs.Count < indx ? Dialogs[indx] : this;
            }
        }

        public override List<Dialog> GetSubDialogs(Dialog currentDialog = null)
        {
            List<Dialog> result = new List<Dialog>();



            return result;
        }
    }
}

