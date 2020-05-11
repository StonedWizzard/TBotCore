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
    class OperationsContainer : IDebugUnit
    {
        /// <summary>
        /// Refernce to telegram api 
        /// </summary>
        public TelegramBotClient TelegramApi { get; protected set; }

        protected readonly IDebuger _Debuger;
        public IDebuger Debuger => _Debuger;

        protected Dictionary<string, BaseOperation> Operations;

        /// <summary>
        /// Create default operation container with default operations
        /// </summary>
        public OperationsContainer(IDebuger debuger)
        {
            _Debuger = debuger;
            Operations = new Dictionary<string, BaseOperation>();

            // add default operations
            Operations.Add(SendMessage.Name, new SendMessage(this));
        }

        public void SetApi(TelegramBotClient api) { TelegramApi = api; }

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
        /// </summary>
        public BaseOperation GetOperation(string opId)
        {
            if (Operations.ContainsKey(opId))
                return Operations[opId];

            Debuger?.LogError(new DebugMessage($"Operation '{opId}' didn't registered in operations container!", "GetOperation()"));
            return null;
        }
    }
}
