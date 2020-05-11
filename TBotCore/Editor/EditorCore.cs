using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TBotCore.Config;

namespace TBotCore.Editor
{
    /// <summary>
    /// Works with ready to use consigs throught editor
    /// </summary>
    public class EditorCore
    {
        public BotConfigs Configs { get; private set; }

        /// <summary>
        /// Default constructor.
        /// Same thing as create new configs vith default values
        /// </summary>
        public EditorCore()
        {
            Configs = new BotConfigs(null, Config.RawData.Configs.GetDefaultConfigs());
        }

    }
}
