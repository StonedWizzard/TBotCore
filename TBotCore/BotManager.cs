using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TBotCore.Debug;
using TBotCore.Config;
using TBotCore.Core;
using TBotCore.Db;

namespace TBotCore
{
    /// <summary>
    /// Response for initialization of all bot modules.
    /// This class like a start point for whole telegram bot.
    /// </summary>
    class BotManager
    {
        static BotManager _Core;
        /// <summary>
        /// Global (singleton) variable, to acces configs
        /// </summary>
        public static BotManager Core
        {
            get => _Core;
            set
            {
                //prevents changes of core instance
                if (_Core == null) _Core = value;
                else
                    throw new InvalidOperationException("Core already initialized and can't be changed while working!");
            }
        }

        protected LogController _LogController;
        public LogController LogController { get => _LogController; }

        public BotConfigs Configs { get; protected set; }
        public OperationsContainer BotOperations { get; protected set; }
        public DialogsProvider Dialogs { get; protected set; }
        public BotAPI BotApiManager { get; protected set; }

        // Constructors
        public BotManager(LogController logController = null, OperationsContainer operationsContainer = null)
        {
            // In first initialize loggers. 
            // without them should be tought debug
            _LogController = logController == null ? new LogController(new TextLogger()) : logController;
            LogController.LogSystem(new DebugMessage("Initializing TBotCore..."));

            // load botOperations
            // on dialogs delegates binding this thing must be initialized
            BotOperations = operationsContainer == null ? new OperationsContainer(LogController) : operationsContainer;

            // load bot configs, text data and dialogs
            // at the same time converts serializeble data to working optimized format
            LogController.LogSystem(new DebugMessage("Initializing configs..."));
            ConfigWorker configWorker = new ConfigWorker(LogController);
            var confResult = configWorker.ReadConfigs();

            // load fails. get default configs
            // and provide instant saving
            if (!confResult.Item1)
            {
                LogController.LogSystem(new DebugMessage("Loading configs fails! Used default settings!"));
                Configs = new BotConfigs(LogController, Config.RawData.Configs.GetDefaultConfigs());
                configWorker.SaveConfig(Configs);
            }
            // Ok!
            else
                Configs = confResult.Item2;

            Core = this;

            // set reference to api...
            // but only afte full initialization of BotApiManager
            BotOperations.SetApi(BotApiManager.Api);
        }

    }
}
