using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Core.Data;
using TBotCore.Core.Operations;
using TBotCore.Db;
using Rd = TBotCore.Config.RawData;
namespace TBotCore.Core.Dialogs
{
    /// <summary>
    /// This class only just a filler.
    /// Do the same logic as SerialDialog (show dialogs one by one and collect responses)
    /// but applies to registration process
    /// </summary>
    public sealed class RegistrationDialog : SerialDialog
    {
        public RegistrationDialog(Rd.Dialog dialog, Dialog owner) :
            base(dialog, owner) 
        {
            // set operation anyway
            Operation = BotManager.Core.Repository.CreteRegistrationOp();
            Path = RootPath;
        }

        public override async Task<BotResponse> Execute(IUser user)
        {
            // get next dialog and call it 
            // if it last or no any dialogs - return container and execute it
            Dialog next = Next(BotManager.Core.BotApiManager.ContextController.GetUserState(user).CurrentDialog);
            object data = null;
            if(next == this)
            {
                var opResult = await Operation.Execute(user);
                if (opResult.ResultType == OperationResult.OperationResultType.Failed || opResult.ResultType == OperationResult.OperationResultType.Unknown)
                {
                    return new BotExceptionResponse(opResult.ExceptionMessage, "txt_internalServerError", user, true);
                }
                else data = opResult.Result;
                
                BotResponse response = new BotResponse(data, BotResponse.ResponseType.Dialog, user, this);
                return response;
            }
            return await next.Execute(user);
        }
    }
}
