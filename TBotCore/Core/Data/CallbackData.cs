using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TBotCore.Core.Dialogs;

namespace TBotCore.Core.Data
{
    /// <summary>
    /// Represent a formatted callback data of any button
    /// </summary>
    public class CallbackData
    {
        /// <summary>
        /// Sender of this callback
        /// </summary>
        [JsonIgnore]
        public IButton Sender { get; protected set; }

        #region Serializeble Data
        /// <summary>
        /// Id of button wich sended callback
        /// </summary>
        public string SenderId { get; set; }
        /// <summary>
        /// Type of sender (service button or dialog)
        /// </summary>
        public string SenderType { get; set; }
        /// <summary>
        /// Name of dialog or operation to execute
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// Define what kind of content stored in Content field
        /// </summary>
        public ContentTypeEnum ContentType { get; set; }
        /// <summary>
        /// Arguments or other data to support content execution
        /// </summary>
        public dynamic Data { get; set; }
        #endregion

        public enum ContentTypeEnum
        {
            Unknown,
            Dialog,
            Operation,
        }

        /// <summary>
        /// Set reference to sender just for quick access
        /// </summary>
        public void SetSenderRef(IButton sender)
        {
            if (sender == null)
                throw new ArgumentNullException("sender");

            Sender = sender;
        }
    }
}
