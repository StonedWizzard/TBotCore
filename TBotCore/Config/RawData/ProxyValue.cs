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
        public string Ip;
        public int Port;
        public string Login;
        public string Password;

        public ProxyValue() { /*support serialization*/ }

        public ProxyValue(Proxy proxy)
        {
            Ip = proxy.Ip;
            Port = proxy.Port;
            Login = proxy.Login;
            Password = proxy.Password;
        }
    }
}
