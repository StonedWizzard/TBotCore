using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TBotCore.Debug;
using TBotCore.Editor;

namespace TBotCore.Config
{
    /// <summary>
    /// Contains language settings and text strings
    /// </summary>
    public sealed class TextProcessor : IEditable<TextProcessor.EditebleTextProcessor>
    {
        List<string> _Languages;
        public IEnumerable<string> Languages 
        {
            get => _Languages.ToImmutableList();
            private set => _Languages = value.ToList();
        }
        List<string> _TextStrings;
        public IEnumerable<string> TextStrings 
        {
            get => _TextStrings.ToImmutableList(); 
            private set => _TextStrings = value.ToList(); 
        }

        private Dictionary<string, StringDictionary> TextDict;
        public string this[string lang, string str]
        {
            get
            {
                string result = str;
                // language is not defined - return raw string key
                if (!IsLanguageExist(lang))
                {
                    BotManager.Core?.LogController?.LogWarning(new DebugMessage($"Language '{lang}' not fiound!"));
                    return result;
                }

                try { result = TextDict[lang][str]; }
                catch (Exception e)
                {
                    BotManager.Core?.LogController?.LogWarning(new DebugMessage("Text string error!", $"TextProcessor[{lang},{str}]", e));
                }
                return result == null ? str : result;
            }
        }

        public TextProcessor()
        {
            TextDict = new Dictionary<string, StringDictionary>();
            Languages = new List<string>();
            TextStrings = new List<string>();
        }
        public TextProcessor(RawData.LangValuesContainer textConfig) : this()
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
        public bool IsLanguageExist(string lang) { return Languages.Contains(lang); }

        /// <summary>
        /// Check if such string key exist
        /// </summary>
        public bool IsTextStringExist(string key) { return TextStrings.Contains(key); }



        #region Editors things
        public EditebleTextProcessor GetEditable()
        {
            return new EditebleTextProcessor(this);
        }

        public class EditebleTextProcessor : IEntityEditor<TextProcessor>
        {
            public TextProcessor EditableObject { get; private set; }

            public EditebleTextProcessor(TextProcessor owner) { EditableObject = owner; }

            /// <summary>
            /// Add new language 
            /// and duplicate all text strings to new dictionary
            /// </summary>
            public bool AddLanguage(string lang)
            {
                if (!IsLanguageExist(lang))
                {
                    EditableObject._Languages.Add(lang);
                    StringDictionary newLangDictionary = new StringDictionary();

                    //copy all text strings to newDict
                    foreach (string str in EditableObject._TextStrings)
                        newLangDictionary.Add(str, "");

                    EditableObject.TextDict.Add(lang, newLangDictionary);
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Adds new text string
            /// </summary>
            public bool AddTextString(string key)
            {
                return AddTextString(key, "");
            }
            /// <summary>
            /// Adds new text string and value within
            /// </summary>
            public bool AddTextString(string key, string value)
            {
                if(!IsTextStringExist(key))
                {
                    foreach (string lang in EditableObject.Languages)
                        EditableObject.TextDict[lang].Add(key, value);

                    EditableObject._TextStrings.Add(key);
                    return true;
                }

                // can't add new textString with same Id (key)
                return false;
            }


            public bool IsTextStringExist(string key)
            {
                return EditableObject.IsTextStringExist(key);
            }

            public bool IsLanguageExist(string lang)
            {
                return EditableObject.IsLanguageExist(lang);
            }


            /// <summary>
            /// Remove specific text string from all collections
            /// </summary>
            public bool RemoveTextString(string key)
            {
                if (IsTextStringExist(key))
                {
                    foreach (string lang in EditableObject.Languages)
                        EditableObject.TextDict[lang].Remove(key);

                    EditableObject._TextStrings.Remove(key);
                    return true;
                }

                return true;
            }

            public bool RemoveLanguage(string lang)
            {
                if (IsLanguageExist(lang))
                {
                    EditableObject.TextDict.Remove(lang);
                    EditableObject._Languages.Remove(lang);

                    return true;
                }

                return true;
            }

            /// <summary>
            /// Edit specific text string
            /// </summary>
            public void EditTextString(string lang, string key, string value)
            {
                EditableObject.TextDict[lang][key] = value;
            }
        }
        #endregion
    }
}
