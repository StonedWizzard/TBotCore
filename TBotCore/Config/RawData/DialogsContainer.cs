using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Core;
using CD = TBotCore.Core.Dialogs;

namespace TBotCore.Config.RawData
{
    /// <summary>
    /// Serializeble container with dialogs and other supporting stuff
    /// (configs, support buttons, etc...)
    /// Also class represent root dialog
    /// </summary>
    [Serializable]
    public class DialogsContainer : Dialog
    {
        /// <summary>
        /// List of generic support buttons
        /// Root dialog can't own any of them.
        /// </summary>
        public List<Button> SupportButtons;

        public Dialog RegistrationDialog;

        public DialogsContainer() : base()
        {
            Id = "Root";
            Name = "txt_rootDialogName";
            Message = new List<string>();
            Data = null;
            Type = typeof(CD.RootDialog).ToString();
            
            SupportButtons = new List<Button>();
            RegistrationDialog = GetRegistrationDialog();
        }

        /// <summary>
        /// Converts work dialogs to serializeble data
        /// </summary>
        public DialogsContainer(DialogsProvider dp) : this()
        {
            var root = dp.RootDialog;
            Dialogs = DialogsReader(root);

            // fill general buttons
            foreach (var btn in dp.GetButtons())
                SupportButtons.Add(new Button(btn));

            // initialize own buttons field
            Buttons = new List<string>();
            foreach (var btn in root.SupportButtons)
                Buttons.Add(btn.Id);

            // provide serialization of registration dialog
            RegistrationDialog = new Dialog(dp.RegistrationDialog);
            RegistrationDialog.Dialogs = DialogsReader(dp.RegistrationDialog);
        }

        /// <summary>
        /// Use recursion to extract subdialogs
        /// </summary>
        private List<Dialog> DialogsReader(CD.Dialog entryPoint)
        {
            if (entryPoint == null)
                return new List<Dialog>();

            List<Dialog> result = new List<Dialog>();

            foreach (var dia in entryPoint.Dialogs)
            {
                Dialog parent = new Dialog(dia);
                parent.Dialogs = DialogsReader(dia);
                result.Add(parent);
            }
            return result;
        }

        private Dialog GetRegistrationDialog()
        {
            return new Dialog()
            {
                Id = "RegistrationDialog",
                Name = "txt_registrationDialogName",
                Type = typeof(CD.RegistrationDialog).ToString(),
                Message = new List<string> { "txt_registrationDia_content1" },
                Data = "",
                Operation = null,
            };
        }
    }
}
