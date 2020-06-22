using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Core.Dialogs;
using TBotCore.Db;

namespace TBotCore.Core.Operations
{
    /// <summary>
    /// Operation allow navigate back to previous dialog
    /// </summary>
    public class SendBackOperation : BaseOperation
    {
        public SendBackOperation(OperationsContainer owner) : base(owner) 
        {
            RequiredArgsName = new List<string>() { "CurrentDialog" };
        }

        public override async Task<OperationResult> Execute(OperationArgs args)
        {
            // checks if all arguments for concrete operation is defined
            if (!RequiredArgsName.Intersect(args.Args.Keys).Any())
                return new OperationResult(null, OperationResult.OperationResultType.Failed, "One or more arguments for operation is missed!");

            if (!ValidateUser(args.User.UserRole))
                return new OperationResult(null, OperationResult.OperationResultType.Failed, "User not valid to call this operation!");

            if(!args.Args.ContainsKey("CurrentDialog"))
                return new OperationResult(null, OperationResult.OperationResultType.Failed, "CurrentDialog arg is missed!");

            // return empty response to next handler
            await Task.Delay(BotManager.Core.Configs.BasicDelay);
            try
            {
                Dialog dia = args.Args["CurrentDialog"] as Dialog;
                var result = await dia.Owner.Execute(args.User);

                return new OperationResult(result, OperationResult.OperationResultType.Success);
            }
            catch(Exception e)
            {
                return new OperationResult(null, OperationResult.OperationResultType.Failed, $"{e.Message}");
            }
        }

        public override Task<OperationResult> Execute(IUser user)
        {
            throw new Exception($"SendBackOperation - method call is not allowed!");
        }
    }
}
