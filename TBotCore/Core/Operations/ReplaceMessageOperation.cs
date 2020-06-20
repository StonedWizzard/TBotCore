using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotCore.Core.Operations
{
    class ReplaceMessageOperation : BaseMessageOperation
    {
        public ReplaceMessageOperation(OperationsContainer owner) : base(owner) { }

        public async override Task<OperationResult> Execute(OperationArgs args)
        {
            if (args == null)
                return new OperationResult(null, OperationResult.OperationResultType.Failed, "Operation args can't be null!");

            var response = await base.Execute(args);
            if (response.ResultType == OperationResult.OperationResultType.Failed)
                return response;

            var msgArgs = GetMessageParams(args);
            if (msgArgs == null)
                return new OperationResult(null, OperationResult.OperationResultType.Failed, "Can't parse operation arguments!");

            // do the job
            try
            {
                var msg = await BotManager.Core.BotApiManager.Api.EditMessageTextAsync(msgArgs.Value.ChatId, msgArgs.Value.MsgId, msgArgs.Value.Content,
                    Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: msgArgs.Value.ReplyMarkup as InlineKeyboardMarkup);
                return new OperationResult(msg.MessageId, OperationResult.OperationResultType.Success);
            }
            catch (Exception e)
            {
                return new OperationResult(e, OperationResult.OperationResultType.Failed, e.Message);
            }
        }
    }
}
