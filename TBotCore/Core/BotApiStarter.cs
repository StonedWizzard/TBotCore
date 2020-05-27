using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Config;
using TBotCore.Debug;

namespace TBotCore.Core
{
    /// <summary>
    /// Provide initialization of ApiManager.
    /// Constructor get type of ApiManager and initialize later
    /// </summary>
    public class BotApiStarter
    {
        protected ProxyManager ProxyManager;
        protected bool ForcedLoad;

        public BotApiStarter() 
        {
            ProxyManager = BotManager.Core.Repository.CreateProxyController();
            ForcedLoad = false;
        }

        public virtual BotAPI InitializeApi()
        {
            BotAPI result = null;
            do
            {
                BotManager.Core?.LogController?.LogImportant(new DebugMessage("Try to initialize telegram Api!"));
                var proxy = ProxyManager.Next();

                try
                {
                    result = BotManager.Core.Repository.CreateBotApi(proxy);
                }
                catch(Exception e)
                {
                    BotManager.Core?.LogController?.LogError(new DebugMessage("Initialization fails!", "InitializeApi()", e));
                    Task.Delay(BotManager.Core.Configs.BasicDelay * 10);
                }
            }
            while (result == null && ForcedLoad);

            return result;
        }
    }
}
