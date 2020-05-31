using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TBotCore.Debug;
using TBotCore.Core.Operations;
using Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TBotCore.Core
{
    /// <summary>
    /// Dictionary<string, BaseOperation> where contains telegram operations
    /// by they string Id. Container provides binding operators and dialog execution delegates
    /// during dialogs initioalization on bot start
    /// </summary>
    public class OperationsContainer
    {
        /// <summary>
        /// Refernce to telegram api 
        /// </summary>
        public TelegramBotClient TelegramApi => BotManager.Core?.BotApiManager?.Api;

        protected Dictionary<string, BaseOperation> Operations;

        /// <summary>
        /// Create default operation container with default operations
        /// </summary>
        public OperationsContainer()
        {
            Operations = new Dictionary<string, BaseOperation>();

            // add default operations
            InitializeOperations();
        }

        public virtual void InitializeOperations()
        {
            Operations.Add(nameof(SendMessageOperation), new SendMessageOperation(this));
        }

        public BaseOperation this[string indx]
        {
            get
            {
                return GetOperation(indx);
            }
        }

        /// <summary>
        /// Return reference to operation by id.
        /// When operation missed or id is incorrect - return null and generate logMessage
        /// If useFiller = true than return operation filler to make serialization compability
        /// </summary>
        public BaseOperation GetOperation(string opId, bool useFiller = false)
        {
            if (String.IsNullOrEmpty(opId)) return null;

            if (Operations.ContainsKey(opId))
                return Operations[opId];

            // create virtual operation
            else if (useFiller)
                return new OperationFiller(opId);

            BotManager.Core?.LogController?
                .LogError(new DebugMessage($"Operation '{opId}' didn't registered in operations container!", "GetOperation()"));
            return null;
        }
    }
}
