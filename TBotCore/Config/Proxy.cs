using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TBotCore.Editor;

namespace TBotCore.Config
{
    public class Proxy : IEditable<Proxy.EditableProxy>
    {
        public string Ip { get; private set; }
        public int Port { get; private set; }
        public string Login { get; private set; }
        public string Password { get; private set; }

        public Proxy(string ip, int port, string login = null, string password = null)
        {
            Ip = ip;
            Port = port;
            Login = login;
            Password = password;
        }

        public Proxy(RawData.ProxyValue proxy) : 
            this(proxy.Ip, proxy.Port, proxy.Login, proxy.Password) { }



        public EditableProxy GetEditable()
        {
            return new EditableProxy(this);
        }

        public class EditableProxy : IEntityEditor<Proxy>
        {
            public Proxy EditableObject { get; private set; }

            public EditableProxy(Proxy owner) { EditableObject = owner; }


            public string Ip
            {
                get => EditableObject.Ip;
                set => EditableObject.Ip = value;
            }
            public int Port
            {
                get => EditableObject.Port;
                set => EditableObject.Port = value;
            }
            public string Login
            {
                get => EditableObject.Login;
                set => EditableObject.Login = value;
            }
            public string Password
            {
                get => EditableObject.Password;
                set => EditableObject.Password = value;
            }
        }
    }
}
