using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBotCore.Config
{
    /// <summary>
    /// Contains language settings and text strings
    /// </summary>
    public sealed class TextProcessor
    {
        public List<string> Languages { get; private set; }
        public List<string> TextStrings { get; private set; }

        private Dictionary<string, StringDictionary> TextDict;
        public string this[string lang, string str]
        {
            get 
            {
                // language is not defined - return raw string key
                if (!LanguageExist(lang))
                {
                    return str;
                }

                try 
                { 
                    return TextDict[lang][str]; 
                }
                catch { return str; }
            }
        }

        private Debug.IDebuger Debuger;


        public TextProcessor(Debug.IDebuger debuger)
        {
            Debuger = debuger;
            TextDict = new Dictionary<string, StringDictionary>();
            Languages = new List<string>();
            TextStrings = new List<string>();
        }

        public TextProcessor(Debug.IDebuger debuger, RawData.LangValuesContainer textConfig) : this(debuger)
        {
            Languages = textConfig.Languages.Distinct().ToList();
            TextStrings = textConfig.TextStrings.Distinct().ToList();
            TextDict = new Dictionary<string, StringDictionary>();

            // we get lists of languages and strings
            // so go throught them to initialize dictionary
            // and by the way read language fields
            foreach(string lang in Languages)
            {
                TextDict.Add(lang, new StringDictionary());

                // select specified language section, down below extract from buffer language data
                List<RawData.ConfigValue> stringsBuffer = textConfig.StringsData.FirstOrDefault(x => x.Section == lang)?.Array;

                foreach(string str in TextStrings)
                {
                    string translation = stringsBuffer == null ? str : 
                        (string)stringsBuffer.First(x => x.Key == str).Value;
                    TextDict[lang].Add(str, translation);
                }
            }
        }


        /// <summary>
        /// Check if such language supported by bot
        /// </summary>
        public bool LanguageExist(string lang) { return Languages.Contains(lang); }

        /// <summary>
        /// Add new language 
        /// and duplicate all text strings to new dictionary
        /// </summary>
        public bool AddLanguage(string lang)
        {
            if (!LanguageExist(lang))
            {
                Languages.Add(lang);
                Dictionary<string, string> newLangDictionary = new Dictionary<string, string>();

                //copy all text strings to newDict

                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds new text string
        /// </summary>
        public void AddTextString(string key)
        {

        }

        /// <summary>
        /// Remove specific text string from all collections
        /// </summary>
        public void RemoveTextString(string key)
        {

        }

        /// <summary>
        /// Edit specific text string
        /// </summary>
        public void EditTextString(string lang, string key, string value)
        {

        }
    }
}
