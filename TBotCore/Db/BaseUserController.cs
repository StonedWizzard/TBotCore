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
    public abstract class BaseUserController
    {
        /// <summary>
        /// Provides user cache for optimization (reduces db calls)
        /// </summary>
        protected Dictionary<long, IUser> UserCache;

        /// <summary>
        /// Create user record in cache or update existed
        /// </summary>
        protected virtual void UpdateCache(IUser user)
        {
            if (user == null) return;

            if (UserCache.ContainsKey(user.UserId))
                UserCache[user.UserId] = user;
            else
                UserCache.Add(user.UserId, user);
        }

        /// <summary>
        /// Checks if such user record keeps in Db
        /// Attention - not overrided method checks only cache!
        /// </summary>
        public async virtual Task<bool> IsUserExist(long userId)
        {
            await Task.Delay(1);
            return UserCache.ContainsKey(userId);
        }
        /// <summary>
        /// Checks if such user record keeps in Db
        /// </summary>
        public async virtual Task<bool> IsUserExist(IUser user)
        {
            await Task.Delay(1);
            return UserCache.ContainsKey(user.UserId);
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
            IUser result = BotManager.Core.Repository.CreateUser();
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
