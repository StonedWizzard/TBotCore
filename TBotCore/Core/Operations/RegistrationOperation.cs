using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Db;

namespace TBotCore.Core.Operations
{
    public class RegistrationOperation : BaseOperation
    {
        public RegistrationOperation(OperationsContainer owner) : base(owner) { }

        public override async Task<OperationResult> Execute(IUser user)
        {
            try
            {
                bool result = await BotManager.Core.BotApiManager.UsersController.CompleateRegistration(user);
                if (result)
                    return new OperationResult(user, OperationResult.OperationResultType.Success);
                else
                    return new OperationResult(null, OperationResult.OperationResultType.Failed);
            }
            catch (Exception e)
            {
                return new OperationResult(e, OperationResult.OperationResultType.Failed, e.Message);
            }
        }

        public override async Task<OperationResult> Execute(OperationArgs args)
        {
            if (args == null)
                return new OperationResult(null, OperationResult.OperationResultType.Failed, "Operation args can't be null!");

            var response = await base.Execute(args);
            if (response.ResultType == OperationResult.OperationResultType.Failed)
                return response;


            try
            {
                bool result = await BotManager.Core.BotApiManager.UsersController.CompleateRegistration(args.User);
                if (result)
                    return new OperationResult(args.User, OperationResult.OperationResultType.Success);
                else
                    return new OperationResult(null, OperationResult.OperationResultType.Failed);
            }
            catch (Exception e)
            {
                return new OperationResult(e, OperationResult.OperationResultType.Failed, e.Message);
            }
        }
    }
}
