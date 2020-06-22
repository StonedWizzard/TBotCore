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
    /// This operation allow switch current dialog to root
    /// </summary>
    public class SendToRootOperation : BaseOperation
    {
        public SendToRootOperation(OperationsContainer owner) : base(owner) { }

        public override async Task<OperationResult> Execute(OperationArgs args)
        {
            // checks if all arguments for concrete operation is defined
            if (!RequiredArgsName.Intersect(args.Args.Keys).Any())
                return new OperationResult(null, OperationResult.OperationResultType.Failed, "One or more arguments for operation is missed!");

            // return empty response to next handler
            await Task.Delay(BotManager.Core.Configs.BasicDelay);
            try
            {
                Dialog dia = BotManager.Core.Dialogs.RootDialog;
                var result = await dia.Execute(args.User);

                return new OperationResult(result, OperationResult.OperationResultType.Success);
            }
            catch (Exception e)
            {
                return new OperationResult(null, OperationResult.OperationResultType.Failed, $"{e.Message}");
            }
        }

        public override async Task<OperationResult> Execute(IUser user)
        {
            return await Execute(new OperationArgs(user));
        }
    }
}
