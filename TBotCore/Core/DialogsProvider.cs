using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TBotCore.Core.Dialogs;
using TBotCore.Debug;
using TBotCore.Core.Data;
using TBotCore.Db;

namespace TBotCore.Core
{
    /// <summary>
    /// Contains and controlls dialogs
    /// </summary>
    sealed class DialogsProvider : IDebugUnit
    {
        /// <summary>
        /// Dictionary contains references to dialogs in list. Simplified access to them by id.
        /// Original dialogs struct keeps in list.
        /// </summary>
        private Dictionary<string, Dialog> DialogsTable;
        private List<Dialog> Dialogs;

        /// <summary>
        /// Dictionary with support buttons, like back/forward, etc...
        /// </summary>
        private Dictionary<string, Button> SupportButtons;

        private RootDialog _RootDialog;
        /// <summary>
        /// Shortcut ref to root dialog
        /// </summary>
        public RootDialog RootDialog { get => _RootDialog; }

        private QuizSeriesDialog _RegistrationDialog;
        /// <summary>
        /// Shortcut reference for registration dialog
        /// </summary>
        public QuizSeriesDialog RegistrationDialog { get => _RegistrationDialog; }

        private readonly IDebuger _Debuger;
        public IDebuger Debuger => _Debuger;

        public DialogsProvider(IDebuger debuger, TBotCore.Config.RawData.DialogsContainer root)
        {
            _Debuger = debuger;

            Dialogs = new List<Dialog>();
            DialogsTable = new Dictionary<string, Dialogs.Dialog>();

            // create root dialog
            try
            {
                _RootDialog = new RootDialog(root as Config.RawData.Dialog);
                DialogsTable.Add(_RootDialog.Id, _RootDialog);
                Dialogs.Add(_RootDialog);
            }
            catch(Exception e)
            {
                Debuger?.LogCritical(new DebugMessage($"Couldn't initialize root dialog!", "DialogsProvider()", e));
            }

            // fill other dialogs
            List<Dialog> dialogs = DialogInitializer(root, RootDialog);
            _RootDialog.SetSubDialogs(dialogs.Where(x => x.Id == _RootDialog.Id).ToList());
            foreach(var dia in dialogs)
            {
                if (DialogsTable.ContainsKey(dia.Id))
                {
                    Debuger?.LogError(new DebugMessage($"Dialog with Id = '{dia.Id}' already exist!\r\n" +
                        $"Try give it another name or delete duplicates.", "DialogInitializer()", "Dialog was skipped!"));
                    continue;
                }

                DialogsTable.Add(dia.Id, dia);
                Dialogs.Add(dia);
            }
            
            // fill support buttons
            SupportButtons = new Dictionary<string, Button>();
            foreach (var btn in root.SupportButtons)
            {
                try
                {
                    var type = Type.GetType(btn.Type);
                    Button button = Activator.CreateInstance(type, btn) as Button;
                    SupportButtons.Add(btn.Id, button);
                }
                catch(Exception e)
                {
                    Debuger?.LogWarning(new DebugMessage($"Couldn't convert support button '{btn.Id}'", "DialogsProvider()", e));
                }
            }
        }

        /// <summary>
        /// Use recursion to extract subdialogs
        /// </summary>
        private List<Dialog> DialogInitializer(Config.RawData.Dialog entryPoint, Dialog owner)
        {
            if (entryPoint == null)
                return new List<Dialog>();

            List<Dialog> result = new List<Dialog>();
            foreach(var dia in entryPoint.Dialogs)
            {
                try
                {
                    var type = Type.GetType(dia.Type);
                    Dialog dialog = Activator.CreateInstance(type, dia, owner) as Dialog;
                    result.Add(dialog);

                    // check if dialog can keep other dialogs
                    if(dialog is IDialogsContainer)
                    {
                        List<Dialog> range = DialogInitializer(dia, dialog);
                        (dialog as DialogContainer).
                            SetSubDialogs(range.Where(x=> x.Owner.Id == dialog.Id).ToList());
                        result.AddRange(range);
                    }
                }
                catch (Exception e)
                {
                    Debuger?.LogError(new DebugMessage($"Couldn't convert dialog '{dia.Id}'", "DialogInitializer()", e));
                }
            }

            return result;
        }

        /// <summary>
        /// Returns dialog in response, if user able to acces it
        /// </summary>
        public BotResponse GetDialog(string dialogId, IUser user)
        {
            Dialog dialog = GetDialog(dialogId);
            if(dialog == null)
            {
                // dialog has missed by some reason...
                // send user to root and make log
                Debuger.LogError(new DebugMessage($"Couldn't find dialog '{dialogId}'", "GetDialog()"));
                return new BotResponse(RootDialog.Id, BotResponse.ResponseType.Dialog, user);
            }

            if (dialog.ValidateUser(user.UserRole))
                return new BotResponse(dialog.Id, BotResponse.ResponseType.Dialog, user);

            // else return access exception
            return new BotResponse("txt_accessDenied", BotResponse.ResponseType.Exception, user);
        }

        /// <summary>
        /// Returns dialog by it's id.
        /// Use only in editor to prevent user access bugs
        /// </summary>
        public Dialog GetDialog(string dialogId)
        {
            if (DialogsTable.ContainsKey(dialogId))
                return DialogsTable[dialogId];

            return null;
        }
    }
}
