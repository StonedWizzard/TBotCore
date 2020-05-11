using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CD = TBotCore.Core.Dialogs;

namespace TBotCore.Config.RawData
{
    /// <summary>
    /// Serializeble container with dialogs and other supporting stuff
    /// (configs, support buttons, etc...)
    /// Also class represent root dialog
    /// </summary>
    [Serializable]
    class DialogsContainer : Dialog
    {
        /// <summary>
        /// List of generic support buttons
        /// Root dialog can't own any of them.
        /// </summary>
        public List<Button> SupportButtons;

        public DialogsContainer() : base()
        {
            Id = "Root";
            Name = "txt_rootDialogName";
            Message = new List<string>();
            Data = null;
            Type = typeof(Core.Dialogs.RootDialog).ToString();

            SupportButtons = new List<Button>();
        }

        /// <summary>
        /// Converts work dialogs to serializeble data
        /// </summary>
        public DialogsContainer(Core.Dialogs.RootDialog root) : base()
        {
            Dialogs = DialogsReader(root);
        }

        /// <summary>
        /// Use recursion to extract subdialogs
        /// </summary>
        private List<Dialog> DialogsReader(CD.Dialog entryPoint)
        {
            if (entryPoint == null)
                return new List<Dialog>();

            List<Dialog> result = new List<Dialog>();

            var container = entryPoint as CD.IDialogsContainer;
            foreach (var dia in container.Dialogs)
            {
                Dialog parent = new Dialog(dia);
                if (dia is CD.IDialogsContainer)
                {
                    parent.Dialogs = DialogsReader(dia);
                    result.Add(parent);
                }
            }
            return result;
        }
    }
}
