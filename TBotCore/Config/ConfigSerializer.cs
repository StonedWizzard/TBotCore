using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Core;

namespace TBotCore.Config
{
    /// <summary>
    /// This worker serialize and deserialize bot configs.
    /// By default it's Xml serializer, but class can be inherited
    /// </summary>
    public class ConfigSerializer
    {
        public virtual string FileName { get; protected set; } = "TBotConfigs.xml";

        public ConfigSerializer() { }
        public ConfigSerializer(string fileName) : this()
        {
            FileName = fileName;
        }

        /// <summary>
        /// Serialize config data to file.
        /// </summary>
        public virtual bool SaveConfig(BotConfigs configs, DialogsProvider dialogs)
        {
            bool result = false;
            BotManager.Core?.LogController?.LogMessage(new Debug.DebugMessage($"Saving configs file '{FileName}'..."));
            try
            {
                using FileStream fs = new FileStream(FileName, FileMode.Create);
                try
                {
                    if (configs == null)
                        throw new ArgumentNullException("config", "Configs object can't be null!");

                    RawData.Configs serializedData = new RawData.Configs(configs, dialogs);
                    XmlSerializer serializer = new XmlSerializer(typeof(RawData.Configs));
                    serializer.Serialize(fs, serializedData);
                    result = true;
                    BotManager.Core?.LogController?.LogSucces(new Debug.DebugMessage($"Saving configs file is success!"));
                }
                catch (Exception e)
                {
                    BotManager.Core?.LogController?
                        .LogError(new Debug.DebugMessage($"Coudn't serialize data!\r\nError: {e.Message}", "ConfigWorker.SaveConfig()", e));
                }
                finally { fs?.Dispose(); }
            }
            catch(Exception e)
            {
                BotManager.Core?.LogController?
                    .LogError(new Debug.DebugMessage($"Coudn't save file '{FileName}'\r\nError: {e.Message}", "ConfigWorker.SaveConfig()", e));
            }

            return result;
        }

        /// <summary>
        /// Deserialize and return config object from file
        /// </summary>
        public virtual (bool, BotConfigs, DialogsProvider) ReadConfigs()
        {
            bool resultFlag = false;
            BotConfigs resultConfigs = null;
            DialogsProvider resultDialogs = null;

            BotManager.Core?.LogController?.LogMessage(new Debug.DebugMessage($"Reading configs file '{FileName}'..."));

            try
            {
                using FileStream fs = new FileStream(FileName, FileMode.Open);
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(RawData.Configs));
                    RawData.Configs deserializedData = (RawData.Configs)serializer.Deserialize(fs);

                    resultConfigs = new BotConfigs(deserializedData);
                    resultDialogs = new DialogsProvider(deserializedData.Dialogs);
                    resultFlag = true;
                    BotManager.Core?.LogController
                        ?.LogSucces(new Debug.DebugMessage($"Reading configs file is success!"));
                }
                catch (Exception e)
                {
                    BotManager.Core?.LogController?
                        .LogError(new Debug.DebugMessage($"Coudn't deserialize data!\r\nError: {e.Message}", "ConfigWorker.ReadConfigs()", e));
                }
                finally { fs?.Dispose(); }
            }
            catch (Exception e)
            {
                BotManager.Core?.LogController?
                    .LogError(new Debug.DebugMessage($"Coudn't read file '{FileName}'\r\nError: {e.Message}", "ConfigWorker.ReadConfigs()", e));
            }
            return (resultFlag, resultConfigs, resultDialogs);
        }
    }
}
