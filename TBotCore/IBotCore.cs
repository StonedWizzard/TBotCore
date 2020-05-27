using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TBotCore.Config;
using TBotCore.Core;

namespace TBotCore
{
    public interface IBotCore
    {
        BotAPI BotApiManager { get; }
        public BotConfigs Configs { get; }
        public OperationsContainer Operations { get; }
        public DialogsProvider Dialogs { get; }
        public ChatCommandsProvider Commands { get; }
        public Debug.IDebuger LogController { get; }
        public DIcontainerBase Repository { get; }

        public bool IsEditor { get; }

        public bool IsInitialized { get; }
    }
}
