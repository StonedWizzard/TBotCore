using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SampleTgBot.DB;

using TBotCore.Db;
using TBotCore.Debug;

namespace SampleTgBot.DB
{
    internal class UserDataController : BaseUserController
    {
        readonly IDebuger Log;

        public UserDataController() : base() 
        {
            UserCache = new Dictionary<long, IUser>();
            Log = Program.Log; 
        }

        /// <summary>
        /// Mark user as compleatly registered in system
        /// </summary>
        public async override Task<bool> CompleateRegistration(IUser user)
        {
            try
            {
                using AppDbContext dbContext = new AppDbContext();
                var dbUser = await dbContext.Users.FirstOrDefaultAsync(x => x.UserId == user.UserId);
                dbUser.IsRegistered = true;

                int x = await dbContext.SaveChangesAsync();
                UpdateCache(dbUser);

                // add user log record
                string logMsg = $"User compleate registration - @{user} [UserId = {user.UserId}]";
                await CreateLogMessage(user.UserId, logMsg);
                TBotCore.BotManager.Core.LogController.LogMessage(new DebugMessage(logMsg));
                return true;
            }
            catch (Exception e)
            {
                Log?.LogError(new TBotCore.Debug.DebugMessage($"Can't compleate registration! [UserId: {user.UserId}, UserName: {user.UserName}]",
                     "CompleateRegistration()", e));
                return false;
            }
        }

        /// <summary>
        /// Create new user entity in db.
        /// Don't mean compleat registrtion procedure!
        /// </summary>
        public async override Task<bool> CreateUser(IUser user)
        {
            try
            {
                using AppDbContext dbContext = new AppDbContext();
                User dbUser = User.GetNewUser(user);
               
                dbContext.Users.Add(dbUser);
                dbContext.UserPreferences.Add((UserPreferences)dbUser.UserPreferences);
                dbContext.UserRoles.Add((UserRole)dbUser.UserRole);

                int x = await dbContext.SaveChangesAsync();
                UpdateCache(dbUser);

                // add user log record
                string logMsg = $"Registered new user - @{user.UserName} [UserId = {user.UserId}]";
                await CreateLogMessage(user.UserId, logMsg);
                TBotCore.BotManager.Core.LogController.LogMessage(new DebugMessage(logMsg));

                return true;
            }
            catch(Exception e)
            {
                Log?.LogError(new TBotCore.Debug.DebugMessage($"Can't create new user! [UserId: {user.UserId}, UserName: {user.UserName}]",
                    "CreateUser()", e));
                return false;
            }
        }

        /// <summary>
        /// Just update user record in db
        /// </summary>
        public async override Task<bool> UpdateUserInfo(IUser user)
        {
            try
            {
                using AppDbContext dbContext = new AppDbContext();
                var dbUser = await dbContext.Users.FirstOrDefaultAsync(x => x.UserId == user.UserId);
                dbUser.Update(user);

                int x = await dbContext.SaveChangesAsync();
                UpdateCache(dbUser);
                return true;
            }
            catch (Exception e)
            {
                Log?.LogError(new TBotCore.Debug.DebugMessage($"Can't update user! [UserId: {user.UserId}, UserName: {user.UserName}]",
                    "UpdateUserInfo()", e));
                return false;
            }
        }


        public async override Task<bool> IsUserExist(long userId)
        {
            bool result = await base.IsUserExist(userId);
            if(result == false)
            {
                try
                {
                    using AppDbContext dbContext = new AppDbContext();
                    var dbUser = await dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);
                    UpdateCache(dbUser);

                    result = dbUser != null;
                }
                catch (Exception e)
                {
                    Log?.LogError(new TBotCore.Debug.DebugMessage($"Something wrong?! [UserId: {userId}]",
                    "IsUserExist()", e));
                }
            }

            return result;
        }
        public async override Task<bool> IsUserExist(IUser user)
        {
            return await IsUserExist(user.UserId);
        }

        /// <summary>
        /// Create log message for specific user
        /// </summary>
        public async Task<bool> CreateLogMessage(long userId, string msg)
        {
            try
            {
                using AppDbContext dbContext = new AppDbContext();
                UserLogEntry logEntry = new UserLogEntry(userId, msg);
                dbContext.UsersLog.Add(logEntry);

                int x = await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Log?.LogError(new TBotCore.Debug.DebugMessage($"Something wrong?! [UserId: {userId}]",
                "CreateLogMessage()", e));
                return false;
            }
        }

        public override async Task<IUser> GetUser(long userId)
        {
            try
            {
                using AppDbContext dbContext = new AppDbContext();
                var dbUser = await dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);
                if (dbUser == null) return dbUser;

                dbUser.UserPreferences = await dbContext.UserPreferences.FirstOrDefaultAsync(x => x.UserId == userId);
                dbUser.UserRole = await dbContext.UserRoles.FirstOrDefaultAsync(x => x.UserId == userId);
                return dbUser;
            }
            catch (Exception e)
            {
                Log?.LogError(new TBotCore.Debug.DebugMessage($"Can't get user! [UserId: {userId}]",
                    "UpdateUserInfo()", e));
                return null;
            }
        }
    }
}
