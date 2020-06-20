using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Db;
using TBotCore.Debug;

namespace TBotCore.Core.Data
{
    /// <summary>
    /// Tracks user input and callbacks.
    /// Controlls bot behaviour based on user inputs
    /// </summary>
    public class UserInputContextController
    {
        /// <summary>
        /// Virtual (means only in ram, not serialized or saved anyway) dictionary
        /// where key is userId and value - context with response cache
        /// </summary>
        private Dictionary<long, UserContextState> ContextCollection;
        private HashSet<long> IgnoredContexts;

        public UserInputContextController()
        {
            ContextCollection = new Dictionary<long, UserContextState>();
            IgnoredContexts = new HashSet<long>();
        }

        /// <summary>
        /// Returns current user state or create new one
        /// </summary>
        public UserContextState GetUserState(IUser user)
        {
            if (IgnoredContexts.Contains(user.UserId))
                return null;

            if (!ContextCollection.ContainsKey(user.UserId))
            {
                UserContextState state = new UserContextState(user);
                SetState(state);
                return state;
            }
            else return ContextCollection[user.UserId];
        }


        public void SetState(UserContextState state)
        {
            // first initialize context if not initialized
            if (!ContextCollection.ContainsKey(state.User.UserId))
                ContextCollection.Add(state.User.UserId, state);

            else
                ContextCollection[state.User.UserId] = state;
        }

        /// <summary>
        /// Clear current state of user. Use carefully!
        /// </summary>
        public void ClearState(IUser user)
        {
            if (ContextCollection.ContainsKey(user.UserId))
                ContextCollection[user.UserId] = new UserContextState(user);
        }


        /// ??????????????????????????????????????????????????????????????????????????
        public BotResponse AddRequest(IUser user, BotRequest request)
        {
            return null;
        }

        /// <summary>
        /// Set user context to ignored. It means controller not return user state
        /// and next handling of requests is imposible.
        /// Prevent (mostly) react bot to it's own messages, but also can be used to block users.
        /// </summary>
        public void SetIgnored(long user, bool ignored = true)
        {
            if(ignored)
                IgnoredContexts.Add(user);
            else
                IgnoredContexts.Remove(user);
        }
    }
}