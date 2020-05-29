using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Config;
using TBotCore.Core;
using TBotCore.Core.Data;
using TBotCore.Db;

namespace TBotCore
{
    /// <summary>
    /// Factory to create concrete instances of some extendable core classes
    /// </summary>
    public abstract class DIcontainerBase
    {
        #region Db
        /// <summary>
        /// Create concrete instance of UserConroller
        /// </summary>
        public abstract BaseUserController CreateUserController();
        /// <summary>
        /// Create concrete instance of User
        /// </summary>
        public abstract IUser CreateUser();
        /// <summary>
        /// Create concrete instance of Chat
        /// </summary>
        public abstract IChat CreateChat();
        /// <summary>
        /// Create concrete instance of user personal log transaction
        /// requires user and message
        /// </summary>
        public abstract IUserLogMessage CreateUserLogMessage(IUser user);
        /// <summary>
        /// Create concrete instance of settings. required user reference
        /// </summary>
        public abstract IUserPreferences CreateUserPreferences(IUser user);
        /// <summary>
        /// Create concrete instance of user access rights. required reference to user
        /// </summary>
        public abstract IUserRole CreateUserRole(IUser user);
        #endregion

        #region Configs
        public virtual BotConfigs CreateConfigs() { return new BotConfigs(); }
        public virtual BotConfigs CreateConfigs(Config.RawData.Configs config) { return new BotConfigs(config); }

        public virtual ConfigSerializer CreateSerializer() { return new ConfigSerializer(); }
        public virtual ConfigSerializer CreateSerializer(string fileName) { return new ConfigSerializer(fileName); }

        public virtual ProxyManager CreateProxyController() { return new ProxyManager(); }
        #endregion

        #region Core
        public virtual BotAPI CreateBotApi(Proxy proxy = null) { return new BotAPI(proxy); }

        public virtual BotApiStarter CreateBotInitializer() { return new BotApiStarter(); }

        public virtual UIDispatcher CreateUiDispatcher(UserInputContextController controller) { return new UIDispatcher(controller); }

        public virtual UserInputContextController CreateUserContextController() { return new UserInputContextController(); }

        public virtual DataParser CreateCallbackParser() { return new DataParser(); }

        public virtual ConfigSerializer CreateConfigSerializer() { return new ConfigSerializer(); }

        public virtual ConfigSerializer CreateConfigSerializer(string path) { return new ConfigSerializer(path); }

        public virtual OperationsContainer CreateOperations() { return new OperationsContainer(); }

        public virtual ChatCommandsProvider CreateCommands() { return new ChatCommandsProvider(); }
        #endregion
    }
}
