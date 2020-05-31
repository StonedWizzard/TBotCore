using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Db;
using TBotCore.Core.Dialogs;

namespace TBotCore.Core.Data
{
    /// <summary>
    /// Describe the user current dialog state.
    /// Context controller make decisions using this class.
    /// </summary>
    public class UserContextState
    {
        /// <summary>
        /// Reference to user
        /// </summary>
        public IUser User { get; protected set; }
        /// <summary>
        /// Contains dictionary of responses to dialogs
        /// Dict<dialog_id, response>
        /// </summary>
        public Dictionary<string, BotRequest> ResponcesCache { get; protected set; }
        /// <summary>
        /// Type of current state
        /// </summary>
        public ContextState CurrentState { get; protected set; }
        /// <summary>
        /// Reference to current dialog
        /// </summary>
        public Dialog CurrentDialog { get; protected set; }
        /// <summary>
        /// Last chat message. Bigger then actual value means keep message and print new one
        /// when UiDispatcher display messages
        /// </summary>
        public int LastMsgId { get; protected set; }
        /// <summary>
        /// Index of currentle showed DialogContentPage
        /// </summary>
        public int CurrentContentPage { get; protected set; }

        /// <summary>
        /// Define if context occupied by other thread or not;
        /// </summary>
        public bool IsContextOccupied { get; protected set; }

        #region
        public UserContextState(IUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            User = user;
            CurrentState = ContextState.Empty;
            ResponcesCache = new Dictionary<string, BotRequest>();
        }
        public UserContextState(IUser user, Dialog dialog) : this(user)
        {
            CurrentDialog = dialog;
        }
        public UserContextState(IUser user, Dialog dialog, ContextState state) : this(user, dialog)
        {
            CurrentState = state;
        }
        public UserContextState(IUser user, Dialog dialog, int msgId) : this(user, dialog)
        {
            LastMsgId = msgId;
        }
        public UserContextState(IUser user, Dialog dialog, ContextState state, int msgId) : this(user, dialog, state)
        {
            LastMsgId = msgId;
            CurrentState = state;
        }
        #endregion

        /// <summary>
        /// Adds user response to response cache
        /// </summary>
        public void AddResponse(string diaId, BotRequest request)
        {
            if(!request.IsNull())
                ResponcesCache.Add(diaId, request);
        }

        object occMutex = new object();
        public bool OccupieContext()
        {
            if (IsContextOccupied) return false;

            lock (occMutex) { IsContextOccupied = true; }
            return true;
        }

        object rMutex = new object();
        public void RealiseContex()
        {
            lock (rMutex)
                IsContextOccupied = false;
        }

        public enum ContextState
        {
            Empty,
            AwaitInput,
        }
    }
}
