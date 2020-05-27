using System;
using System.Collections.Generic;
using System.Linq;
using CD = TBotCore.Core.Dialogs;

namespace TBotCore.Config.RawData
{
    /// <summary>
    /// Serializeble dialog
    /// </summary>
    [Serializable]
    public class Dialog : Button
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
        /// </summary>
        public List<Dialog> Dialogs;
        /// <summary>
        /// List of supportive buttons. stored only Id
        /// </summary>
        public List<string> Buttons;

        public Dialog() : base()
        {
            Buttons = new List<string>();
            Dialogs = new List<Dialog>();
            Message = new List<string>();
            Operation = null;
        }

        public Dialog(CD.Dialog dialog) : base(dialog)
        {
            Dialogs = new List<Dialog>();
            Message = 
                dialog is CD.PaginatedDialog ? (dialog as CD.PaginatedDialog).Content.ToList() : new List<string>() { dialog.Content };

            Buttons = new List<string>();
            foreach (var btn in dialog.SupportButtons)
                Buttons.Add(btn.Id);

            Operation = dialog.Operation?.ToString();
        }
    }
}
