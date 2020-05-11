using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Debug;

namespace TBotCore.Config
{
    /// <summary>
    /// Converts serializeble configs to application format
    /// or app configs to serializeble format
    /// </summary>
    public class Converter : IDebugUnit
    {
        IDebuger _Debuger;
        public IDebuger Debuger => _Debuger;

        public Converter(IDebuger debuger)
        {
            _Debuger = debuger;
        }


        /// <summary>
        /// Converts optimized work on settings to serializeble format
        /// TODO - Dialogs!!!
        /// </summary>
        public virtual RawData.Configs ToSerializeble(BotConfigs configs)
        {
            if (configs == null)
                throw new ArgumentNullException();

            RawData.Configs result = new RawData.Configs(configs);
            return result;
        }

        /// <summary>
        /// Converts serializeble configs object to work on settings
        /// TODO - Dialogs?
        /// </summary>
        public virtual BotConfigs FromSerializeble(RawData.Configs configs)
        {
            if (configs == null)
                throw new ArgumentNullException();

            BotConfigs result = new BotConfigs(Debuger, configs);
            return result;
        }
    }
}
