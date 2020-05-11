using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TBotCore.Core.Data;
using TBotCore.Db;
using Telegram.Bot;

namespace TBotCore.Core.Operations
{
    /// <summary>
    /// Base telegram bot dialog/button operation
    /// </summary>
    abstract class BaseOperation : IRequestValidate
    {
        /// <summary>
        /// Name (or Id, if you like) of operation
        /// </summary>
        public static string Name { get; protected set; }
        /// <summary>
        /// Container witch owns this instance
        /// </summary>
        public readonly OperationsContainer Owner;
        /// <summary>
        /// List of required arguments for operation
        /// </summary>
        protected List<string> RequiredArgsName;
        /// <summary>
        /// Refernce to telegram api 
        /// </summary>
        protected TelegramBotClient TelegramApi;

        public BaseOperation(OperationsContainer owner) 
        {
            Name = "null";
            Owner = owner;
            TelegramApi = owner.TelegramApi;
            RequiredArgsName = new List<string>();
        }

        public async virtual Task<OperationResult> Execute(IUser user)
        {
            return await Execute(new OperationArgs(user));
        }

        public async virtual Task<OperationResult> Execute(OperationArgs args)
        {
            // checks if all arguments for concrete operation is defined
            if (RequiredArgsName.Intersect(args.Args.Keys).Any())
                return new OperationResult(null, OperationResult.OperationResultType.Failed, "One or more arguments for operation is missed!");

            if (!ValidateUser(args.User.UserRole))
                return new OperationResult(null, OperationResult.OperationResultType.Failed, "User not valid to call operation!");

            // return empty response to next handler
            await Task.Delay(BotManager.Core.Configs.BasicDelay);
            return new OperationResult(null, OperationResult.OperationResultType.Unknown);
        }

        public virtual bool ValidateUser(IUserRole role)
        {
            return true;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
