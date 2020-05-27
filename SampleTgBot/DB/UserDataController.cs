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

        public async override Task<bool> CompleateRegistration(IUser user)
        {
            try
            {
                using AppDbContext dbContext = new AppDbContext();
                var dbUser = await dbContext.Users.FirstOrDefaultAsync(x => x.UserId == user.UserId);
                dbUser.Update(user);
                dbUser.IsRegistered = true;

                int x = await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Log?.LogError(new TBotCore.Debug.DebugMessage($"Can't compleate registration! [UserId: {user.UserId}, UserName: {user.UserName}]",
                     "CompleateRegistration()", e));
                return false;
            }
        }

        public async override Task<bool> CreateUser(IUser user)
        {
            try
            {
                User dbUser = User.GetNewUser(user);
                using AppDbContext dbContext = new AppDbContext();
                dbContext.Users.Add(dbUser);
                int x = await dbContext.SaveChangesAsync();
                return true;
            }
            catch(Exception e)
            {
                Log?.LogError(new TBotCore.Debug.DebugMessage($"Can't create new user! [UserId: {user.UserId}, UserName: {user.UserName}]",
                    "CreateUser()", e));
                return false;
            }
        }

        public async override Task<bool> UpdateUserInfo(IUser user)
        {
            try
            {
                using AppDbContext dbContext = new AppDbContext();
                var dbUser = await dbContext.Users.FirstOrDefaultAsync(x => x.UserId == user.UserId);
                dbUser.Update(user);

                int x = await dbContext.SaveChangesAsync();
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
            if(!result)
            {
                try
                {
                    using AppDbContext dbContext = new AppDbContext();
                    var dbUser = await dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);
                    UserCache.Add(dbUser.UserId, dbUser);

                    result = true;
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
    }
}
