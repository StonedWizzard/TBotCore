using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBotCore.Config.RawData
{
    /// <summary>
    /// Determinates serializeble proxy server value
    /// </summary>
    [Serializable]
    public class ProxyValue
    {
        public string Adress;
        public int Port;
        public string Login;
        public string Password;
    }
}
