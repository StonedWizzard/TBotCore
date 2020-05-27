using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TBotCore.Config;
using TBotCore.Core;
using TBotCore.Debug;

namespace TBotCore.Editor
{
    /// <summary>
    /// Works with ready to use configs throught editor
    /// </summary>
    public class EditorCore : IBotCore
    {
        public BotConfigs Configs { get; private set; }
        public DialogsProvider Dialogs { get; private set; }
        public OperationsContainer Operations { get; private set; }
        public bool IsEditor { get; } = true;

        public BotAPI BotApiManager { get; private set; }

        public ChatCommandsProvider Commands { get; private set; }

        public IDebuger LogController { get; private set; }

        public DIcontainerBase Repository { get; private set; }

        public bool IsInitialized => throw new NotImplementedException();

        /// <summary>
        /// Default constructor.
        /// Same thing as create new configs vith default values
        /// </summary>
        public EditorCore()
        {
            // set repository first
            Repository = new EditorRepository();

            Operations = new OperationsContainer();
            BotManager.SetCore(this);
            var rawConfigs = Config.RawData.Configs.GetDefaultConfigs();

            Configs = new BotConfigs(rawConfigs);
            Dialogs = new DialogsProvider(rawConfigs.Dialogs);
        }

        public EditorCore(BotConfigs configs, DialogsProvider dialogs)
        {
            if (configs == null)
                throw new ArgumentNullException("configs");

            Operations = new OperationsContainer();
            BotManager.SetCore(this);

            Configs = configs;
            Dialogs = dialogs;
        }
    }
}
