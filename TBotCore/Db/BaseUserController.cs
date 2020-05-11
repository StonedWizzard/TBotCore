using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Debug;

namespace TBotCore.Db
{
    /// <summary>
    /// Define methods to work with user data.
    /// Reads and update user records in Db
    /// </summary>
    abstract class BaseUserController : IDebugUnit
    {
        /// <summary>
        /// Provides user cache for optimization (reduces db calls)
        /// </summary>
        protected Dictionary<int, IUser> UserCache;
        protected Type UserInstanceType;

        protected readonly IDebuger _Debuger;
        public IDebuger Debuger => _Debuger;

        protected BaseUserController(IDebuger debuger, Type userInstanceType)
        {
            _Debuger = debuger;
            UserInstanceType = userInstanceType;
        }

        /// <summary>
        /// Checks if such user record keeps in Db
        /// Attention - not overrided method checks only cache!
        /// </summary>
        public async virtual Task<bool> IsUserExist(int userId)
        {
            await Task.Delay(1);

            if (UserCache.ContainsKey(userId)) return true;
            else return false;
        }
        /// <summary>
        /// Checks if such user record keeps in Db
        /// </summary>
        public async virtual Task<bool> IsUserExist(IUser user)
        {
            await Task.Delay(1);

            if (UserCache.ContainsKey(user.UserId)) return true;
            else return false;
        }

        /// <summary>
        /// Creates user record in Db.
        /// Didn't mean full registration!
        /// </summary>
        abstract public Task<bool> CreateUser(IUser user);

        /// <summary>
        /// Updates user info in db.
        /// Call this method as little as possible due to db calls!
        /// </summary>
        abstract public Task<bool> UpdateUserInfo(IUser user);

        /// <summary>
        /// Mark user as fully registered and save changes to Db
        /// </summary>
        abstract public Task<bool> CompleateRegistration(IUser user);

        /// <summary>
        /// Converts telegram user record to apopriate user instance
        /// </summary>
        public virtual IUser ConvertUser(Telegram.Bot.Types.User user, bool isRegistered = false)
        {
            IUser result = Activator.CreateInstance(UserInstanceType) as IUser;
            if (result == null)
                throw new InvalidCastException($"Cannot convert user [Id={user.Id}] to apopriate type!");

            result.UserId = user.Id;
            result.LastName = user.LastName;
            result.FirstName = user.FirstName;
            result.UserName = user.Username;
            result.IsRegistered = isRegistered;

            return result;
        }
    }
}
