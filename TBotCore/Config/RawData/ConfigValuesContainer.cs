using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TBotCore.Config.RawData
{
    /// <summary>
    /// Represents serializeble values container
    /// </summary>
    [Serializable]
    public class ConfigValuesContainer
    {
        public ConfigValuesContainer() 
        {
            Array = new List<ConfigValue>(); 
            Section = "null";
        }
        public ConfigValuesContainer(string section) : this()
        {
            Section = section;
        }
        public ConfigValuesContainer(IList<TBotCore.Config.ConfigValue> values, string section) : this(section)
        {
            foreach(var val in values)
            {
                ConfigValue cval = new ConfigValue(val);
                Array.Add(cval);
            }
        }

        public string Section;
        public List<ConfigValue> Array;

        public void Add(ConfigValue value) { Array.Add(value); }
    }
}
