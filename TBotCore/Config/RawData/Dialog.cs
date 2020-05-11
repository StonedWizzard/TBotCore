using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CD = TBotCore.Core.Dialogs;

namespace TBotCore.Config.RawData
{
    /// <summary>
    /// Serializeble dialog
    /// </summary>
    [Serializable]
    class Dialog : Button
    {
        /// <summary>
        /// Dialog message
        /// </summary>
        public List<string> Message;
        /// <summary>
        /// Name of operation for dialog to execute
        /// </summary>
        public string Operation;

        /// <summary>
        /// List of stored dialogs
        /// Attention - deserialized type should implement IDialogsContainer
        /// </summary>
        public List<Dialog> Dialogs;

        public Dialog() : base()
        {
            Dialogs = new List<Dialog>();
            Message = new List<string>();
            Operation = null;
        }

        public Dialog(CD.Dialog dialog) : base(dialog)
        {
            Dialogs = new List<Dialog>();
            Message = 
                dialog is CD.MultiPageDialog ? (dialog as CD.MultiPageDialog).Content : new List<string>() { dialog.Content };

            Operation = dialog.Operation != null ? dialog.Operation.ToString() : null;
        }
    }
}
