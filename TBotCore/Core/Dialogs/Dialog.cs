using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rd = TBotCore.Config.RawData;
using TBotCore.Core.Data;
using TBotCore.Db;
using TBotCore.Core.Operations;

namespace TBotCore.Core.Dialogs
{
    /// <summary>
    /// Base dialog class or simple endpoint dialog
    /// </summary>
    class Dialog : IButton
    {
        /// <summary>
        /// Returns full dialog message.
        /// But in some cases can be applied pagination.
        /// </summary>
        public virtual string Content { get; protected set; }
        public Operations.BaseOperation Operation { get; protected set; }

        public string Id { get; protected set; }
        public string DisplayedName { get; protected set; }
        public string Data { get; protected set; }
        public int DisplayPriority { get; protected set; }
        public Dialog Owner { get; protected set; }
        public List<Button> SupportButtons { get; protected set; }

        // Constructors
        public Dialog(string id)
        {
            if (String.IsNullOrEmpty(id))
                throw new ArgumentNullException("Id");

            Id = id;
        }
        public Dialog(Rd.Dialog dialog, Dialog owner) : this(dialog.Id)
        {
            DisplayedName = dialog.Name;
            Content = dialog.Message.FirstOrDefault();
            Data = dialog.Data;
            DisplayPriority = dialog.DisplayPriority;

            Owner = owner;
            Operation = BotManager.Core.BotOperations.GetOperation(dialog.Operation);
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
    }
}
