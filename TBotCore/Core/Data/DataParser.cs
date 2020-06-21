using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TBotCore.Core.Dialogs;

namespace TBotCore.Core.Data
{
    /// <summary>
    /// Parse and convert callback data from buttons
    /// </summary>
    public sealed class DataParser
    {
        DialogsProvider DialogsProvider { get; }

        public DataParser()
        {
            DialogsProvider = BotManager.Core.Dialogs;
        }

        /// <summary>
        /// Generate callback data for concrete button
        /// </summary>
        public string GetData(IButton callbackSource)
        {
            if (callbackSource == null)
                throw new ArgumentNullException("callbackSource");

            // deserialize data string from button/dialog
            dynamic data = null;
            var cntType = callbackSource is Dialog ? 
                CallbackData.ContentTypeEnum.Dialog : CallbackData.ContentTypeEnum.Operation;

            if (String.IsNullOrEmpty(callbackSource.Data))
                data = new { Content = "null", ContentType = cntType };
            else
                data = JsonConvert.DeserializeObject(callbackSource.Data);

            // create CallbackData obj
            CallbackData callbackData = new CallbackData
            {
                Id = callbackSource.Id,     //16bytes
                T = data.ContentType,       //1byte
                D = data.Content,           //26bytes
                // and 15bytes reserved for JSON formatting
                // total 64 - it's maximum callback data size
            };

            // ...and serialize to string, wich we rwturn
            string result = JsonConvert.SerializeObject(callbackData);
            return result;
        }

        /// <summary>
        /// Reads raw data and generate callbackData
        /// </summary>
        public CallbackData ParseData(string rawData)
        {
            if (String.IsNullOrEmpty(rawData))
                throw new ArgumentNullException();

            CallbackData callbackData = JsonConvert.DeserializeObject<CallbackData>(rawData);
            callbackData.SetSenderRef(DialogsProvider.GetEntity(callbackData.Id));

            return callbackData;
        }
    }
}
