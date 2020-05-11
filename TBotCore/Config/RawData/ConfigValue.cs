using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TBotCore.Config.RawData
{
    /// <summary>
    /// Serializeble value
    /// </summary>
    [Serializable]
    public class ConfigValue
    {
        public ConfigValue() { }

        public ConfigValue(Config.ConfigValue configValue)
        {
            Key = configValue.Key;
            Value = configValue.Value;
            ValueType = configValue.ValueType.ToString();

            Name = configValue.Name;
            Description = configValue.Description;
            ValueFlag = configValue.ValueFlags;
        }

        // Main fields
        public string Key;
        public object Value;
        public string ValueType;

        // Editor fields
        public string Name;
        public string Description;
        public ValueFlags ValueFlag;

        [Flags]
        public enum ValueFlags
        {
            Default = 0x0,
            ReadOnly = 0x1,
            Hiden = 0x2,
        }
    }
}
