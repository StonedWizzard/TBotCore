using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TBotCore.Core;
using TBotCore.Core.Data;
using TBotCore.Db;

using Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TBotCore.Core.Operations
{
    /// <summary>
    /// Send to user a new message
    /// </summary>
    class SendMessage : BaseMessageOperation
    {
        public SendMessage(OperationsContainer owner) : base(owner) { }

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
                var msg = await TelegramApi.SendTextMessageAsync
                    (msgArgs.Value.ChatId, msgArgs.Value.Content, Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: msgArgs.Value.ReplyMarkup);
                return new OperationResult(msg.MessageId, OperationResult.OperationResultType.Success);
            }
            catch (Exception e)
            {
                return new OperationResult(e, OperationResult.OperationResultType.Failed, e.Message);
            }
        }
    }
}
