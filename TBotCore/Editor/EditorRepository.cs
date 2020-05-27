using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Db;

namespace TBotCore.Editor
{
    class EditorRepository : DIcontainerBase
    {
        public override IChat CreateChat()
        {
            throw new NotImplementedException();
        }

        public override IUser CreateUser()
        {
            throw new NotImplementedException();
        }

        public override BaseUserController CreateUserController()
        {
            throw new NotImplementedException();
        }

        public override IUserLogMessage CreateUserLogMessage(IUser user)
        {
            throw new NotImplementedException();
        }

        public override IUserPreferences CreateUserPreferences(IUser user)
        {
            throw new NotImplementedException();
        }

        public override IUserRole CreateUserRole(IUser user)
        {
            throw new NotImplementedException();
        }
    }
}
