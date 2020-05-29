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
    public class BotManager : IBotCore
    {
        static IBotCore _Core;
        /// <summary>
        /// Global (singleton) variable, to acces configs
        /// </summary>
        public static IBotCore Core
        {
            get => _Core;
            set
            {
                //prevents changes of core instance
                if (_Core == null || value.IsEditor) _Core = value;
                else
                    throw new InvalidOperationException("Core already initialized and can't be changed while working!");
            }
        }

        protected LogController _LogController;
        public IDebuger LogController { get => _LogController; }



        // IBotCore implement
        public ChatCommandsProvider Commands { get; protected set; }
        public DIcontainerBase Repository { get; protected set; }
        public BotConfigs Configs { get; protected set; }
        public OperationsContainer Operations { get; protected set; }
        public DialogsProvider Dialogs { get; protected set; }
        public BotAPI BotApiManager { get; protected set; }
        public bool IsInitialized { get; protected set; }
        public bool IsEditor { get; } = false;

        

        /// <summary>
        /// Constructor is step one.
        /// Here initialized logs agregator and load configs
        /// </summary>
        public BotManager(DIcontainerBase repository, LogController logController)
        {
            // In first initialize loggers. 
            // without them should be tought to debug
            if (logController == null) throw new ArgumentException("logController");
            _LogController = logController;

            LogController.LogSystem(new DebugMessage("Initializing TBotCore..."));


            // initialize kind of abstract factory
            LogController.LogSystem(new DebugMessage("Initializing DI container..."));
            if (repository == null) throw new ArgumentException("repository");
            Repository = repository;

            LogController.LogSystem(new DebugMessage("Initializing operations container..."));
            Operations = Repository.CreateOperations();
            LogController.LogSucces(new DebugMessage("Ok!"));

            LogController.LogSystem(new DebugMessage("Initializing bot commands..."));
            Commands = Repository.CreateCommands();
            LogController.LogSucces(new DebugMessage("Ok!"));

            // set singleton reference
            SetCore(this);

            // load bot configs, text data and dialogs
            // at the same time converts serializeble data to working optimized format
            LogController.LogSystem(new DebugMessage($"Read and initializing configs(settings, dialogs, buttons, strings and onter)..."));
            ConfigSerializer configWorker = Repository.CreateConfigSerializer();
            var confResult = configWorker.ReadConfigs();

            // load fails. get default configs
            // and provide instant saving
            if (!confResult.Item1)
            {
                LogController.LogWarning(new DebugMessage("Loading configs fails! Used default settings!"));
                Configs = new BotConfigs(Config.RawData.Configs.GetDefaultConfigs());
                Dialogs = new DialogsProvider(new Config.RawData.DialogsContainer());

                configWorker.SaveConfig(Configs, Dialogs);
            }
            // Ok!
            else
            {
                Configs = confResult.Item2;
                Dialogs = confResult.Item3;
            }
            LogController.LogSucces(new DebugMessage("Ok!"));
            LogController.LogSucces(new DebugMessage("BotManager is initialized!"));
        }

        /// <summary>
        /// Finaly try to start bot
        /// </summary>
        public virtual BotManager StartBot()
        {
            // initialize bot API
            BotApiStarter botApiStartup = Repository.CreateBotInitializer();
            LogController.LogSystem(new DebugMessage("Start bot..."));
            BotApiManager = botApiStartup.InitializeApi();
            BotApiManager.StartReceiving();
            this.IsInitialized = true;
            return this;
        }


        public static void SetCore(IBotCore core)
        {
            if (core == null)
                throw new ArgumentNullException("core");

            Core = core;
            core.LogController?.LogSucces(new DebugMessage("BotCore activated!"));
        }
    }
}
