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
using TBotCore.Editor;

namespace TBotCore.Core
{
    /// <summary>
    /// Contains and controlls dialogs in bot
    /// </summary>
    public sealed class DialogsProvider : IEditable<DialogsProvider.EditableDialogsProvider>
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

        /// <summary>
        /// Shortcut ref to root dialog
        /// </summary>
        public RootDialog RootDialog { get; }

        /// <summary>
        /// Shortcut reference for registration dialog
        /// </summary>
        public RegistrationDialog RegistrationDialog { get; }


        public DialogsProvider(Config.RawData.DialogsContainer root)
        {
            Dialogs = new List<Dialog>();
            DialogsTable = new Dictionary<string, Dialogs.Dialog>();

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
                catch (Exception e)
                {
                    BotManager.Core?.LogController?.LogWarning(new DebugMessage($"Couldn't convert support button '{btn.Id}'", "DialogsProvider()", e));
                }
            }

            // create root dialog
            try
            {
                RootDialog = new RootDialog(root as Config.RawData.Dialog);
                RootDialog.GetEditable().SupportButtons = GetButtons(root.Buttons);

                RegistrationDialog = new RegistrationDialog(root.RegistrationDialog, null);
                RegistrationDialog.GetEditable().SupportButtons = GetButtons(root.RegistrationDialog.Buttons);

                DialogsTable.Add(RootDialog.Id, RootDialog);
                Dialogs.Add(RootDialog);
                DialogsTable.Add(RegistrationDialog.Id, RegistrationDialog);
                Dialogs.Add(RegistrationDialog);
            }
            catch (Exception e)
            {
                BotManager.Core?.LogController?.LogCritical(new DebugMessage($"Couldn't initialize root dialog!", "DialogsProvider()", e));
                throw e;
            }

            // fill other dialogs and bind supportButtons
            RootDialog.GetEditable().Dialogs = DialogInitializer(root, RootDialog);
            RegistrationDialog.GetEditable().Dialogs = DialogInitializer(root.RegistrationDialog, RegistrationDialog);
        }

        /// <summary>
        /// Use recursion to extract subdialogs
        /// </summary>
        private List<Dialog> DialogInitializer(Config.RawData.Dialog entryPoint, Dialog owner)
        {
            if (entryPoint == null)
                return new List<Dialog>();

            List<Dialog> result = new List<Dialog>();
            foreach (var dia in entryPoint.Dialogs)
            {
                try
                {
                    var type = Type.GetType(dia.Type);
                    Dialog dialog = Activator.CreateInstance(type, dia, owner) as Dialog;

                    // check table
                    if (DialogsTable.ContainsKey(dia.Id))
                    {
                        BotManager.Core?.LogController?.LogError(new DebugMessage($"Dialog with Id = '{dia.Id}' already exist!\r\n" +
                            $"Try give it another name or delete duplicates.", "DialogInitializer()", "Dialog was skipped!"));
                        continue;
                    }

                    // set buttons
                    foreach(var btn in dia.Buttons)
                        dialog.GetEditable().SupportButtons.Add(GetButton(btn));

                    // serch more dialogs
                    if (dia.Dialogs.Count > 0)
                    {
                        dialog.GetEditable().Dialogs = DialogInitializer(dia, dialog);
                    }

                    // add results
                    result.Add(dialog);
                    DialogsTable.Add(dialog.Id, dialog);
                    Dialogs.Add(dialog);
                }
                catch (Exception e)
                {
                    BotManager.Core?.LogController?.LogError(new DebugMessage($"Couldn't convert dialog '{dia.Id}'", "DialogInitializer()", e));
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
            if (dialog == null)
            {
                // dialog has missed by some reason...
                // send user to root and make log
                BotManager.Core?.LogController?.LogError(new DebugMessage($"Couldn't find dialog '{dialogId}'", "GetDialog()"));
                return new BotResponse(RootDialog.Id, BotResponse.ResponseType.Dialog, user);
            }

            if (dialog.ValidateUser(user.UserRole))
                return new BotResponse(dialog.Id, BotResponse.ResponseType.Dialog, user);

            // else return access exception
            return new BotResponse("txt_accessDenied", BotResponse.ResponseType.Exception, user);
        }

        /// <summary>
        /// Returns dialog by it's id.
        /// </summary>
        public Dialog GetDialog(string dialogId)
        {
            if (DialogsTable.ContainsKey(dialogId))
                return DialogsTable[dialogId];

            return null;
        }

        /// <summary>
        /// Return support button by it's id
        /// </summary>
        public Button GetButton(string buttonId)
        {
            if (SupportButtons.ContainsKey(buttonId))
                return SupportButtons[buttonId];

            return null;
        }

        /// <summary>
        /// Return list of all support buttons in bot
        /// </summary>
        public List<Button> GetButtons()
        {
            return SupportButtons.Values.ToList();
        }

        /// <summary>
        /// Return list of all support buttons in list
        /// </summary>
        public List<Button> GetButtons(List<string> btns)
        {
            var keys = SupportButtons.Keys.Intersect(btns);
            List<Button> result = new List<Button>();
            foreach (var btnKey in keys)
            {
                if (SupportButtons.ContainsKey(btnKey))
                    result.Add(SupportButtons[btnKey]);
                else
                    BotManager.Core?.LogController?
                        .LogWarning(new DebugMessage("Requested support button '{btnKey}' not found in dictionary!"));
            }
            return result;
        }

        /// <summary>
        /// Return count of dialogs to make unique new_dialog Id
        /// </summary>
        public int GetNumSuffix()
        {
            return Dialogs.Count;
        }



        public EditableDialogsProvider GetEditable()
        {
            return new EditableDialogsProvider(this);
        }

        public class EditableDialogsProvider : IEntityEditor<DialogsProvider>
        {
            private readonly Type rootType = typeof(RootDialog);
            private readonly Type regType = typeof(RegistrationDialog);

            public DialogsProvider EditableObject { get; private set; }

            public EditableDialogsProvider(DialogsProvider owner) { EditableObject = owner; }


            public bool InsertDialog(Dialog dia, string parentId = "Root")
            {
                if(!EditableObject.DialogsTable.ContainsKey(dia.Id))
                {
                    Dialog parent = EditableObject.GetDialog(parentId);
                    // prevent serial dialogs have childs
                    if (parent.Owner is SerialDialog)
                        return false;

                    // prevent serial dialogs have dialogs other than simple dialog
                    if (parent is SerialDialog && !(dia is Dialog))
                        return false;

                    parent.GetEditable().Dialogs.Add(dia);
                    dia.GetEditable().Owner = parent;

                    EditableObject.DialogsTable.Add(dia.Id, dia);
                    EditableObject.Dialogs.Add(dia);
                    return true;
                }
                return false;
            }

            public bool RemoveDialog(string id)
            {
                if (EditableObject.DialogsTable.ContainsKey(id))
                {
                    // get dialog to remove and his master ;)
                    Dialog removedDia = EditableObject.DialogsTable[id];
                    //prevent removing reg and root dialogs
                    if (removedDia is RootDialog || removedDia is RegistrationDialog)
                        return false;

                    Dialog parent = removedDia.Owner;

                    // remove from parent list
                    parent.GetEditable().Dialogs.Remove(removedDia);
                    
                    // clear dialog provider list
                    EditableObject.DialogsTable.Remove(removedDia.Id);
                    EditableObject.Dialogs.Remove(removedDia);

                    return true;
                }
                return false;
            }

            /// <summary>
            /// Convert dialog type
            /// </summary>
            public (bool, Dialog) CastDialog(string id, Type type)
            {
                (bool, Dialog) result = (false, null);
                if (EditableObject.DialogsTable.ContainsKey(id))
                {
                    Dialog dia = EditableObject.DialogsTable[id];
                    if (dia == null) return result;

                    result = (false, dia);

                    // check types - conversion to root and registration imposible
                    if (type == rootType || type == regType)
                        return result;

                    // cast dialog and keep new reference
                    dia = dia.GetEditable().CastDialog(type);
                    result = (true, dia);

                    Dialog parent = dia.Owner;
                    if(parent != null)
                    {
                        var edPar = parent.GetEditable();
                        edPar.Dialogs.RemoveAll(x => x.Id == id);
                        edPar.Dialogs.Add(dia);
                    }

                    EditableObject.DialogsTable.Remove(id);
                    EditableObject.Dialogs.RemoveAll(x => x.Id == id);

                    EditableObject.DialogsTable.Add(id, dia);
                    EditableObject.Dialogs.Add(dia);
                    
                }
                return result;
            }

            /// <summary>
            /// Insert new button
            /// </summary>
            public bool InsertButton()
            {
                string newBtnId = $"new_button{EditableObject.SupportButtons.Count}";

                var btn = new TBotCore.Config.RawData.Button()
                {
                    Id = newBtnId,
                    Name = newBtnId,
                    DisplayPriority = 0,
                    Data = @"{ ContentType: """", Content: """", Data: {}}",
                    Type = typeof(Button).ToString()
                };

                return InsertButton(new Button(btn));
            }
            public bool InsertButton(Button button)
            {
                if (button == null) return false;
                if (EditableObject.SupportButtons.ContainsKey(button.Id)) return false;

                EditableObject.SupportButtons.Add(button.Id, button);
                return true;
            }

            public bool RemoveButton(string id)
            {
                if (EditableObject.SupportButtons.ContainsKey(id)) return false;

                EditableObject.SupportButtons.Remove(id);
                return true;
            }
        }
    }
}
