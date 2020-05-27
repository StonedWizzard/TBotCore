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
        Dictionary<int, UserContextState> ContextCollection;

        public UserInputContextController()
        {
            ContextCollection = new Dictionary<int, UserContextState>();
        }

        /// <summary>
        /// Checks if user content exist and availebe. 
        /// Returns true if context availeble or can be created.
        /// </summary>
        public bool IsUserContextAvaileble(int userId)
        {
            if(ContextCollection.ContainsKey(userId))
            {
                // check context state
                UserContextState context = ContextCollection[userId];
                if (context.CurrentState == UserContextState.ContextState.Empty)
                    return true;

                // other cases means it not availeble
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns current user state or null
        /// </summary>
        public UserContextState GetUserState(int userId)
        {
            if (IsUserContextAvaileble(userId)) return null;
            else return ContextCollection[userId];
        }
        public UserContextState GetUserState(IUser user)
        {
            return GetUserState(user.UserId);
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
