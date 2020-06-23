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
    /// Attention - maximum serialized data size is 64 byte
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
        public string Id { get; set; }

        /// <summary>
        /// Define what kind of content stored in Content field
        /// </summary>
        public ContentTypeEnum T { get; set; }

        /// <summary>
        /// Define additional data, args and etc.
        /// </summary>
        public string D { get; set; }
        #endregion

        public enum ContentTypeEnum
        {
            Unknown,
            Dialog,
            Button,
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
