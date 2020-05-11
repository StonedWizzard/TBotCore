using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rd = TBotCore.Config.RawData;

namespace TBotCore.Core.Dialogs
{
    /// <summary>
    /// This class mostly like most used regular dialog node.
    /// Contains list of next dialogs sorted and within system buttons.
    /// Attention - this dialogs list can be cutted to give space in request for system buttons!
    /// </summary>
    class DialogContainer : Dialog, IDialogsContainer
    {
        /// <summary>
        /// Reference to contained dialogs
        /// </summary>
        public List<Dialog> Dialogs { get; protected set; }

        public DialogContainer(Rd.Dialog dialog, Dialog owner) : base(dialog, owner) { }

        /// <summary>
        /// Set subDialogs references
        /// </summary>
        public void SetSubDialogs(List<Dialog> dialogs) { Dialogs = dialogs; }

        /// <summary>
        /// Returns sorted subdialogs
        /// </summary>
        public virtual List<Dialog> GetSubDialogs()
        {
            return Dialogs.OrderBy(x => x.DisplayPriority).ToList();
        }

        /// <summary>
        /// Returns all subdialogs (required for editor mostly)
        /// </summary>
        public List<Dialog> GetAllSubDialogs()
        {
            return Dialogs.OrderBy(x => x.DisplayPriority).ToList();
        }
    }
}
