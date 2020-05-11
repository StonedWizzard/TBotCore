using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Db;

namespace TBotCore.Core.Operations
{
    /// <summary>
    /// Collection of arguments given to operation
    /// </summary>
    class OperationArgs
    {
        public IUser User { get; protected set; }
        public Dictionary<string, object> Args { get; protected set; }

        public OperationArgs(IUser user)
        {
            User = user;
            Args = new Dictionary<string, object>();
        }

        public OperationArgs(IUser user, Dictionary<string, object> args) : this(user)
        {
            Args = args;
        }

        public object this[string indx]
        {
            get
            {
                if (Args.ContainsKey(indx))
                    return Args[indx];
                else return null;
            }
        }

        /// <summary>
        /// Merge arguments dictionaries
        /// </summary>
        public void MergeArgs(Dictionary<string, object> args)
        {
            if (args == null)
                return;

            foreach(var arg in args)
            {
                Args.Add(arg.Key, arg.Value);
            }
        }
    }
}
