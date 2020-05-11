using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBotCore.Config.RawData
{
    [Serializable]
    public class LangValuesContainer
    {
        public LangValuesContainer()
        {
            Languages = new List<string>();
            TextStrings = new List<string>();
            StringsData = new List<ConfigValuesContainer>();
        }

        public LangValuesContainer(TextProcessor textProcessor)
        {
            Languages = textProcessor.Languages;
            TextStrings = textProcessor.TextStrings;

            // get string of string type 
            // and go throught all languages
            string strType = typeof(string).ToString();
            foreach (string lang in Languages)
            {
                // in each lang iteration create lang section
                ConfigValuesContainer langSection = new ConfigValuesContainer(lang);

                // and finaly build text strings entries
                foreach (string txt in TextStrings)
                {
                    ConfigValue txtData = new ConfigValue()
                    {
                        Key = txt,
                        Value = textProcessor[lang, txt],
                        ValueType = strType
                    };
                    langSection.Array.Add(txtData);
                }
            }
        }

        /// <summary>
        /// List of availeble languages
        /// </summary>
        public List<string> Languages;
        /// <summary>
        /// List of text strings
        /// </summary>
        public List<string> TextStrings;
        /// <summary>
        /// Collection of text translation
        /// </summary>
        public List<ConfigValuesContainer> StringsData;
    }
}
