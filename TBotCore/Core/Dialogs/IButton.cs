using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBotCore.Core.Dialogs
{
    /// <summary>
    /// Provide properties to correct displaing item like TgButton
    /// </summary>
    interface IButton : IRequestValidate
    {
        /// <summary>
        /// Unique ID in settings storage and virtual memory.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Displayed text of button
        /// </summary>
        string DisplayedName { get; }

        /// <summary>
        /// Button data. 
        /// </summary>
        string Data { get; }

        /// <summary>
        /// Defines where display button for user
        /// then lower value, then higher position
        /// </summary>
        int DisplayPriority { get; }

        public Task<Data.BotResponse> Execute(Db.IUser user);
    }
}
