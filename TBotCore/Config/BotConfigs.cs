using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
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
    public class BotConfigs : IEnumerable, IEditable<BotConfigs.EditebleBotConfigs>
    {
        //General configs
        public string BotName { get; protected set; }
        public string BotHash { get; protected set; }
        public int BasicDelay { get; protected set; }
        public bool UseProxy { get; protected set; }

        /// <summary>
        /// Contains string and language data
        /// </summary>
        public TextProcessor TextStrings { get; protected set; }

        List<Proxy> _Proxies;
        public IEnumerable<Proxy> Proxies 
        {
            get => _Proxies.ToImmutableList();
            protected set => _Proxies = value.ToList();
        }

        protected Dictionary<string, ConfigValue> Values;
        public ConfigValue this[string index]
        {
            get
            {
                if (Values.ContainsKey(index)) 
                    return Values[index];
                else
                {
                    BotManager.Core?.LogController?.LogError(new Debug.DebugMessage($"Value with key '{index}' not found in configs!", "BotConfigs.Indexer"));
                    return null;
                }
            }
        }


        public BotConfigs()
        {
            Values = new Dictionary<string, ConfigValue>();
            TextStrings = new TextProcessor();
            Proxies = new List<Proxy>();
        }
        public BotConfigs(RawData.Configs config) : this()
        {
            if (config == null)
                throw new ArgumentNullException("config");

            // initialize basic variables
            BotName = config.BotName;
            BotHash = config.BotHash;
            BasicDelay = config.BasicDelay;

            // fill custom variables
            foreach(RawData.ConfigValue val in config.CustomValues.Array)
            {
                if (!Values.ContainsKey(val.Key))
                    Values.Add(val.Key, new ConfigValue(val));
                else
                    BotManager.Core?.LogController?
                        .LogWarning(new DebugMessage($"Custom value with key '{val.Key}' already exist!\r\nEntity was skiped.", "BotConfigs()", ""));
            }

            // text processor initialize by itself
            TextStrings = new TextProcessor(config.LanguageValues);

            // fill proxy values
            foreach(var prox in config.ProxyServers)
                _Proxies.Add(new Proxy(prox));
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

        public bool IsValueExist(string key)
        {
            return Values.ContainsKey(key);
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
            public BotConfigs EditableObject { get; private set; }

            public EditebleBotConfigs(BotConfigs owner) { EditableObject = owner; }


            // editable properties
            public string BotName
            {
                get => EditableObject.BotName;
                set => EditableObject.BotName = value;
            }
            public string BotHash
            {
                get => EditableObject.BotHash;
                set => EditableObject.BotHash = value;
            }
            public int BasicDelay
            {
                get => EditableObject.BasicDelay;
                set => EditableObject.BasicDelay = value;
            }
            public bool UseProxy
            {
                get => EditableObject.UseProxy;
                set => EditableObject.UseProxy = value;
            }


            public bool IsValueExist(string key) => EditableObject.IsValueExist(key);

            public bool AddValue(string key)
            {
                if(!IsValueExist(key))
                {
                    EditableObject.Values.Add(key, new ConfigValue(key, "new_value", typeof(string)));
                    return true;
                }

                return false;
            }

            public bool RemoveValue(string key)
            {
                if(IsValueExist(key))
                {
                    EditableObject.Values.Remove(key);
                }
                return true;
            }


            public void AddProxy(Proxy proxy)
            {
                EditableObject._Proxies.Add(proxy);
            }

            public void ClearProxies()
            {
                EditableObject._Proxies.Clear();
            }

            public bool RemoveProxy(Proxy proxy)
            {
                return EditableObject._Proxies.Remove(proxy);
            }
        }
        #endregion
    }
}
