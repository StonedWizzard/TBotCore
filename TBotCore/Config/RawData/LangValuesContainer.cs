using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TBotCore.Config.RawData
{
    [Serializable]
    public class LangValuesContainer
    {
        // get string of string type 
        private static string StrType = typeof(string).ToString();

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

        public LangValuesContainer()
        {
            Languages = new List<string>();
            TextStrings = new List<string>();
            StringsData = new List<ConfigValuesContainer>();
        }

        public LangValuesContainer(TextProcessor textProcessor)
        {
            Languages = textProcessor.Languages.ToList();
            TextStrings = textProcessor.TextStrings.ToList();
            StringsData = new List<ConfigValuesContainer>();

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
                        ValueType = StrType,
                    };
                    langSection.Array.Add(txtData);
                }

                StringsData.Add(langSection);
            }
        }


        public static LangValuesContainer GetDefaultStrings()
        {
            LangValuesContainer result = new LangValuesContainer();

            SetString(result, "en", "txt_accessDenied", "Access denied!");
            SetString(result, "en", "txt_internalServerError", "Internal server error occured!");
            SetString(result, "en", "txt_rootDialogName", "Root");
            SetString(result, "en", "txt_registrationDialogName", "Registration");
            SetString(result, "en", "txt_registrationDia_content1", "Wellcome...");

            SetString(result, "ru", "txt_accessDenied", "Нет доступа!");
            SetString(result, "ru", "txt_internalServerError", "Произошла внутренняя ошибка!");
            SetString(result, "ru", "txt_rootDialogName", "Главная");
            SetString(result, "ru", "txt_registrationDialogName", "Регистрация");
            SetString(result, "ru", "txt_registrationDia_content1", "Добро пожаловать...");

            //txt_registrationDialogName
            //txt_registrationDia_content1

            return result;
        }

        private static void SetString(LangValuesContainer data, string lang, string key, string value)
        {
            if (!data.Languages.Contains(lang))
            {
                data.Languages.Add(lang);
            }
            if (!data.TextStrings.Contains(key))
            {
                data.TextStrings.Add(key);
            }

            // create lang entity
            ConfigValue txtData = new ConfigValue()
            {
                Key = key,
                Value = value,
                ValueType = StrType
            };

            var langSection = data.StringsData.FirstOrDefault(x => x.Section == lang);
            if(langSection == null)
            {
                // lang section not created, so we all know what to do
                ConfigValuesContainer section = new ConfigValuesContainer(lang);
                section.Array.Add(txtData);
                data.StringsData.Add(section);
            }
            else
                langSection.Add(txtData);
        }
    }
}
