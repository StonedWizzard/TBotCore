using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CD = TBotCore.Core.Dialogs;

namespace TBotCore.Config.RawData
{
    [Serializable]
    class Button
    {
        /// <summary>
        /// Dialog/Button id
        /// </summary>
        public string Id;
        /// <summary>
        /// Dialog/Button name
        /// </summary>
        public string Name;
        /// <summary>
        /// Button data
        /// </summary>
        public string Data;
        /// <summary>
        /// Serialized entity type
        /// </summary>
        public string Type;
        /// <summary>
        /// Defines show order of this dialog
        /// </summary>
        public int DisplayPriority;

        public Button()
        {
            Id = "";
            Name = "";
            Data = "";
            DisplayPriority = -1;
            Type = typeof(CD.Button).ToString();
        }

        public Button(CD.IButton button)
        {
            Id = button.Id;
            Name = button.DisplayedName;
            Data = button.Data;
            DisplayPriority = button.DisplayPriority;

            // get concrete type of instance
            Type = button.GetType().ToString();
        }
    }
}
