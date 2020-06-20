using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TBotCore.Core;

namespace TBotCore.Config.RawData
{
    /// <summary>
    /// Serializeble config object
    /// </summary>
    [Serializable]
    public class Configs
    {
        const string CustomValuesSectionName = "CustomValues";

        /// <summary>
        /// Bot name
        /// </summary>
        public string BotName;
        /// <summary>
        /// Bot hash
        /// </summary>
        public string BotHash;
        /// <summary>
        /// basic response
        /// </summary>
        public int BasicDelay;

        /// <summary>
        /// Collection of custom properties
        /// </summary>
        public ConfigValuesContainer CustomValues;

        /// <summary>
        /// Bot text strings
        /// </summary>
        public LangValuesContainer LanguageValues;

        /// <summary>
        /// Collection of serializeble proxies
        /// </summary>
        public List<ProxyValue> ProxyServers;

        /// <summary>
        /// Container with bot dialogs, support buttons
        /// Also represent root dialog
        /// </summary>
        public DialogsContainer Dialogs;

        public Configs()
        {
            CustomValues = new ConfigValuesContainer(CustomValuesSectionName);
            LanguageValues = new LangValuesContainer();
            ProxyServers = new List<ProxyValue>();
            Dialogs = new DialogsContainer();
        }

        /// <summary>
        /// Build serializeble configs from work data
        /// </summary>
        public Configs(BotConfigs configs, DialogsProvider dialogs)
        {
            BasicDelay = configs.BasicDelay;
            BotHash = configs.BotHash;
            BotName = configs.BotName;

            CustomValues = new ConfigValuesContainer(configs.GetCustomValues(), CustomValuesSectionName);
            LanguageValues = new LangValuesContainer(configs.TextStrings);

            // fill proxies
            ProxyServers = new List<ProxyValue>();
            foreach (var prox in configs.Proxies)
                ProxyServers.Add(new ProxyValue(prox));

            Dialogs = new DialogsContainer(dialogs);
        }

        /// <summary>
        /// Returns default configs object
        /// </summary>
        public static Configs GetDefaultConfigs()
        {
            Configs result = new Configs();
            result.BasicDelay = 100;
            result.BotHash = "Null";
            result.BotName = "NoName";

            result.LanguageValues = LangValuesContainer.GetDefaultStrings();
            result.Dialogs = new DialogsContainer();

            //fill value containers
            result.CustomValues.Array.Add(new ConfigValue()
            {
                Key = "OnCriticalShutdownDelay",
                Value = 4000,
                ValueFlag = ConfigValue.ValueFlags.Default,
                ValueType = typeof(int).ToString(),
                Name = "On critical error shutdown delay",
                Description = "Set value in miliseconds which indicates time after critical error before app restart.\r\nBy default - 4000ms."
            });
            result.CustomValues.Array.Add(new ConfigValue()
            {
                Key = "ShowDialogPath",
                Value = true,
                ValueFlag = ConfigValue.ValueFlags.Default,
                ValueType = typeof(bool).ToString(),
                Name = "Show dialog path",
                Description = "Enable/Disable displaying user current position in dialogs structure."
            });
            result.CustomValues.Array.Add(new ConfigValue()
            {
                Key = "IsExtendedRegistration",
                Value = false,
                ValueFlag = ConfigValue.ValueFlags.Default,
                ValueType = typeof(bool).ToString(),
                Name = "Extended registration",
                Description = "Set 'true' if Bot requires extended registration dialogs. This mean using registration dialog!"
            });
            result.CustomValues.Array.Add(new ConfigValue()
            {
                Key = "ButtonsPaginationValue",
                Value = 32,
                ValueFlag = ConfigValue.ValueFlags.Default,
                ValueType = typeof(int).ToString(),
                Name = "Paginated buttons per response",
                Description = "Define how many buttons should display in message"
            });
            result.CustomValues.Array.Add(new ConfigValue()
            {
                Key = "ReplaceDialogs",
                Value = true,
                ValueFlag = ConfigValue.ValueFlags.Default,
                ValueType = typeof(bool).ToString(),
                Name = "Replace inline dialogs",
                Description = "Enable dialogs replacement during navigation."
            });
            return result;
        }
    }
}
