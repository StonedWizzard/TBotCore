using SampleTgBot.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Db;

namespace SampleTgBot
{
    class Repository : TBotCore.DIcontainerBase
    {
        public override IChat CreateChat()
        {
            return new Chat();
        }

        public override IUser CreateUser()
        {
            return new User();
        }

        public override BaseUserController CreateUserController()
        {
            return new UserDataController();
        }

        public override IUserLogMessage CreateUserLogMessage(IUser user)
        {
            throw new NotImplementedException();
        }

        public override IUserPreferences CreateUserPreferences(IUser user)
        {
            return new UserPreferences();
        }

        public override IUserRole CreateUserRole(IUser user)
        {
            return new UserRole();
        }

        public override IUserAddInfo CreateUserAddInfo(IUser user)
        {
            UserAddInfo result = new UserAddInfo();
            result.UserId = user.UserId;

            return result;
        }
    }
}
