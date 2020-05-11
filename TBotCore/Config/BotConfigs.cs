using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Debug;
using TBotCore.Editor;

namespace TBotCore.Config
{
    /// <summary>
    /// Bot settings container.
    /// </summary>
    public class BotConfigs : IEnumerable, IDebugUnit, IEditable<BotConfigs.EditebleBotConfigs>
    {
        protected readonly IDebuger _Debuger;
        public IDebuger Debuger => _Debuger;

        //General configs
        public string BotName { get; protected set; }
        public string BotHash { get; protected set; }
        public int BasicDelay { get; protected set; }
        public bool UseProxy { get; protected set; }

        /// <summary>
        /// Contains string and language data
        /// </summary>
        public TextProcessor TextStrings { get; protected set; }

        protected Dictionary<string, ConfigValue> Values;
        public ConfigValue this[string index]
        {
            get
            {
                if (Values.ContainsKey(index)) 
                    return Values[index];
                else
                {
                    Debuger?.LogError(new Debug.DebugMessage($"Value with key '{index}' not found in configs!", "BotConfigs.Indexer"));
                    return null;
                }
            }
        }

        public BotConfigs(IDebuger debuger)
        {
            _Debuger = debuger;
            Values = new Dictionary<string, ConfigValue>();
            TextStrings = new TextProcessor(Debuger);
        }
        public BotConfigs(IDebuger debuger, RawData.Configs config) : this(debuger)
        {
            if (config == null)
                throw new ArgumentNullException();

            BotName = config.BotName;
            BotHash = config.BotHash;
            BasicDelay = config.BasicDelay;

            foreach(RawData.ConfigValue val in config.CustomValues)
            {
                if (!Values.ContainsKey(val.Key))
                    Values.Add(val.Key, new ConfigValue(val));
            }
            TextStrings = new TextProcessor(Debuger, config.LanguageValues);
        }

        /// <summary>
        /// Return list of custom values
        /// </summary>
        public List<ConfigValue> GetCustomValues()
        {
            List<ConfigValue> result = new List<ConfigValue>();
            foreach (var val in Values)
                result.Add(val.Value);

            return result;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        #region Editors things
        public EditebleBotConfigs GetEditable()
        {
            return new EditebleBotConfigs(this);
        }

        /// <summary>
        /// Return editable list of custom values
        /// </summary>
        public List<ConfigValue.EditebleConfigValue> GetEditableValues()
        {
            List<ConfigValue.EditebleConfigValue> result = new List<ConfigValue.EditebleConfigValue>();
            List<ConfigValue> vals = GetCustomValues();
            foreach(var v in vals)
            {
                result.Add(v.GetEditable());
            }
            return result;
        }

        public class EditebleBotConfigs : IEntityEditor<BotConfigs>
        {
            public BotConfigs Owner { get; private set; }

            public EditebleBotConfigs(BotConfigs owner) { Owner = owner; }

            // editable properties
            public string BotName
            {
                get => Owner.BotName;
                set => Owner.BotName = value;
            }
            public string BotHash
            {
                get => Owner.BotHash;
                set => Owner.BotHash = value;
            }
            public int BasicDelay
            {
                get => Owner.BasicDelay;
                set => Owner.BasicDelay = value;
            }
            public bool UseProxy
            {
                get => Owner.UseProxy;
                set => Owner.UseProxy = value;
            }
        }
        #endregion
    }
}
