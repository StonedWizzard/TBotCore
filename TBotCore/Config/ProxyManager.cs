using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBotCore.Config
{
    /// <summary>
    /// Wrapper above proxy list. Provide access to proxies in selected orders.
    /// (for example get random proxies should prevent repeating same things)
    /// </summary>
    public class ProxyManager
    {
        public IEnumerable<Proxy> Proxies { get; }
        private int currentIndx;

        public ProxyManager()
        {
            Proxies = BotManager.Core.Configs.Proxies;
            currentIndx = 0;
        }

        public virtual Proxy Next()
        {
            if(Proxies.Count() == 0)
            {
                BotManager.Core?.LogController?.LogWarning(new Debug.DebugMessage("Proxy list is empty!"));
                return null;
            }

            if (currentIndx + 1 > Proxies.Count())
            {
                BotManager.Core?.LogController?.LogWarning(new Debug.DebugMessage("Proxy list is end, set proxy pointer to start."));
                currentIndx = 0;
            }
            else currentIndx += 1;

            return Proxies.ToArray()[currentIndx];
        }
 
    }
}
