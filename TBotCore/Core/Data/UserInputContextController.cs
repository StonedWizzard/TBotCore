using System;
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
        Dictionary<long, UserContextState> ContextCollection;

        public UserInputContextController()
        {
            ContextCollection = new Dictionary<long, UserContextState>();
        }

        /// <summary>
        /// Returns current user state or create new one
        /// </summary>
        public UserContextState GetUserState(IUser user)
        {
            if (!ContextCollection.ContainsKey(user.UserId))
            {
                UserContextState state = new UserContextState(user);
                return state;
            }
            else return ContextCollection[user.UserId];
        }


        public void SetState(UserContextState state)
        {
            // first initialize context if not initialized
            if (!ContextCollection.ContainsKey(state.User.Id))
                ContextCollection.Add(state.User.Id, state);

            else
                ContextCollection[state.User.Id] = state;
        }

        /// <summary>
        /// Clear current state of user. Use carefully!
        /// </summary>
        public void ClearState(IUser user)
        {
            if (ContextCollection.ContainsKey(user.Id))
                ContextCollection[user.Id] = new UserContextState(user);
        }


        /// ??????????????????????????????????????????????????????????????????????????
        public BotResponse AddRequest(BotRequest request)
        {
            return null;
        }
    }
}
