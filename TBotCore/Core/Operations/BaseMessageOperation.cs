using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotCore.Core.Operations
{
    /// <summary>
    /// Basic class for all messaging involved operations
    /// </summary>
    abstract class BaseMessageOperation : BaseOperation
    {
        public BaseMessageOperation(OperationsContainer owner) : base(owner)
        {
            RequiredArgsName = new List<string>() { "Content", "ChatId", "ReplyMarkdown" };
        }

        /// <summary>
        /// Return converted basic message arguments
        /// </summary>
        protected virtual MessageParams? GetMessageParams(OperationArgs args)
        {
            MessageParams result = new MessageParams();
            try
            {
                result.ChatId = Convert.ToInt64(args["ChatId"]);
                result.Content = args["Content"]?.ToString();
                result.ReplyMarkup = (IReplyMarkup)args["ReplyMarkdown"];
                return result;
            }
            catch { return null; }
        }
        
        public struct MessageParams
        {
            public long ChatId;
            public string Content;
            public IReplyMarkup ReplyMarkup;
        }
    }
}
