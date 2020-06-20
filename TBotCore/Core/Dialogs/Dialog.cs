using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rd = TBotCore.Config.RawData;
using TBotCore.Core.Data;
using TBotCore.Db;
using TBotCore.Core.Operations;
using TBotCore.Editor;

namespace TBotCore.Core.Dialogs
{
    /// <summary>
    /// Base dialog class.
    /// Can store other dialogs like nodes
    /// </summary>
    public class Dialog : IButton, IEditable<Dialog.EditableDialog>
    {
        protected const string RootPath = "*>";

        /// <summary>
        /// Returns full dialog message.
        /// But in some cases can be applied pagination.
        /// </summary>
        public virtual string Content { get; protected set; }
        public BaseOperation Operation { get; protected set; }

        public string Id { get; protected set; }
        public string DisplayedName { get; protected set; }
        public string Data { get; protected set; }
        public int DisplayPriority { get; protected set; }

        /// <summary>
        /// Reference to parent
        /// </summary>
        public Dialog Owner { get; protected set; }
        /// <summary>
        /// Reference to contained dialogs
        /// </summary>
        public List<Dialog> Dialogs { get; protected set; }
        /// <summary>
        /// List of references to support buttons defined in bot.
        /// </summary>
        public List<Button> SupportButtons { get; protected set; }
        public string Path { get; internal set; }

        // Constructors
        public Dialog(string id)
        {
            if (String.IsNullOrEmpty(id))
                throw new ArgumentNullException("Id");
            if (id.IsBloated(16))
                throw new Exception($"Id value lenght cant exceed size of 16 chars!\r\nDialog/Button Id = '{id}'");

            Id = id;
            Dialogs = new List<Dialog>();
            SupportButtons = new List<Button>();
        }
        public Dialog(Rd.Dialog dialog, Dialog owner) : this(dialog.Id)
        {
            if (dialog.Data.IsBloated(26))
                throw new Exception($"Data lenght cant exceed size of 26 chars!\r\nDialog/Button Id = '{dialog.Id}'");

            DisplayedName = dialog.Name;
            Content = dialog.Message?.FirstOrDefault();
            Data = dialog.Data;
            DisplayPriority = dialog.DisplayPriority;
            Dialogs = InitializeSubDialogs(dialog, this);

            Owner = owner;
            Operation = BotManager.Core.Operations.GetOperation(dialog.Operation, BotManager.Core.IsEditor);
        }
        /// <summary>
        /// Good old recursion again initialize dialogs tree
        /// </summary>
        private List<Dialog> InitializeSubDialogs(Rd.Dialog dialog, Dialog owner)
        {
            List<Dialog> result = new List<Dialog>();
            foreach(var dia in dialog.Dialogs)
            {
                Dialog d = new Dialog(dia, owner);
                result.Add(d);
            }

            return result;
        }

        /// <summary>
        /// Returns true if user has access to this dialog.
        /// Method used to validate user actions and define show button in menu or not.
        /// </summary>
        public virtual bool ValidateUser(Db.IUserRole role)
        {
            return true;
        }

        /// <summary>
        /// Executes dialog/button.
        /// Dialogs returns redirect or messages. Buttons mostly just result in message;
        /// </summary>
        public async virtual Task<BotResponse> Execute(IUser user)
        {
            if (!ValidateUser(user.UserRole))
                return new BotExceptionResponse(null, "txt_accessDenied", user, true);

            OperationResult opResult = new OperationResult();
            object data = null;

            // aware context controller about op. execution
            if (Operation != null)
            {
                opResult = await Operation.Execute(user);
                if(opResult.ResultType == OperationResult.OperationResultType.Failed ||
                    opResult.ResultType == OperationResult.OperationResultType.Unknown)
                {
                    return new BotExceptionResponse(opResult.ExceptionMessage, "txt_internalServerError", user, true);
                }
                else
                {
                    // keep op. data
                    data = opResult.Result;
                }
            }

            BotResponse response = new BotResponse(data, BotResponse.ResponseType.Dialog, user, this);
            return response;
        }


        /// <summary>
        /// Returns sorted subdialogs
        /// </summary>
        public virtual List<Dialog> GetSubDialogs(Dialog currentDialog = null)
        {
            return Dialogs.OrderBy(x => x.DisplayPriority).ToList();
        }

        /// <summary>
        /// Returns all subdialogs (required for editor mostly)
        /// </summary>
        public List<Dialog> GetAllSubDialogs()
        {
            return Dialogs.ToList();
        }



        public virtual EditableDialog GetEditable()
        {
            return new EditableDialog(this);
        }

        public class EditableDialog : IEntityEditor<Dialog>
        {
            public Dialog EditableObject { get; private set; }

            public EditableDialog(Dialog owner) { EditableObject = owner; }

            public string Id
            {
                get => EditableObject.Id;
                set => EditableObject.Id = value;
            }
            public BaseOperation Operation
            {
                get => EditableObject.Operation;
                set => EditableObject.Operation = value;
            }
            public string Content
            {
                get => EditableObject.Content;
                set => EditableObject.Content = value;
            }
            public string DisplayedName
            {
                get => EditableObject.DisplayedName;
                set => EditableObject.DisplayedName = value;
            }
            public string Data
            {
                get => EditableObject.Data;
                set => EditableObject.Data = value;
            }
            public int DisplayPriority
            {
                get => EditableObject.DisplayPriority;
                set => EditableObject.DisplayPriority = value;
            }
            public List<Dialog> Dialogs
            {
                get => EditableObject.Dialogs;
                set => EditableObject.Dialogs = value;
            }
            public List<Button> SupportButtons
            {
                get => EditableObject.SupportButtons;
                set => EditableObject.SupportButtons = value;
            }

            public Dialog Owner
            {
                get => EditableObject.Owner;
                set => EditableObject.Owner = value;
            }


            /// <summary>
            /// Cast dialog type to selected type.
            /// Root and Registration dialogs can't be converted!
            /// </summary>
            public Dialog CastDialog(Type type)
            {
                Type currType = EditableObject.GetType();

                // we cant convert root or registration dialogs
                // also no reason convert variable to the same type...
                if (currType == type)
                    return EditableObject;

                try
                {
                    // keep parent and included dialogs save
                    Dialog owner = EditableObject.Owner;
                    List<Dialog> dialogs = EditableObject.Dialogs;

                    Rd.Dialog dia = new Rd.Dialog(EditableObject);
                    dia.Type = type.ToString();

                    Dialog dialog = Activator.CreateInstance(type, dia, owner) as Dialog;
                    EditableObject = dialog;
                    EditableObject.Dialogs = dialogs;
                    return EditableObject;
                }
                catch
                {
                    return EditableObject;
                }
            }
        }
    }
}
