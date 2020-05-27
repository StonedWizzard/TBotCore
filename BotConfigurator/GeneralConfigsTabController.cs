using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TBotCore.Config;

namespace BotConfigurator
{
    class GeneralConfigsTabController : BaseTabController
    {
        EditorController Owner;
        List<Proxy.EditableProxy> ProxiesList;

        public GeneralConfigsTabController(EditorController owner)
        {
            Owner = owner;
            ProxiesList = new List<Proxy.EditableProxy>();
        }

        /// <summary>
        /// Nothing to do - no any entyties
        /// </summary>
        public override void OnEntityChange() { }

        /// <summary>
        /// Save changes to virtual data from ui elements
        /// </summary>
        public override void OnTabClose()
        {
            var edit = Owner.Core.Configs.GetEditable();

            edit.BotHash = Owner.Form.BotHashTokenBox.Text;
            edit.BotName = Owner.Form.BotNameBox.Text;
            edit.BasicDelay = (int)Owner.Form.BasicDelayBox.Value;
            edit.UseProxy = Owner.Form.EnableProxyBox.Checked;
        }

        /// <summary>
        /// Read and display ui from virtual data
        /// </summary>
        public override void OnTabOpen()
        {
            var edit = Owner.Core.Configs.GetEditable();

            Owner.Form.BotHashTokenBox.Text = edit.BotHash;
            Owner.Form.BotNameBox.Text = edit.BotName;
            Owner.Form.BasicDelayBox.Value = edit.BasicDelay;
            Owner.Form.EnableProxyBox.Checked = edit.UseProxy;

            UpdateProxyList();
        }

        /// <summary>
        /// Always display ui fields to edit, so nothing to do
        /// </summary>
        public override void ShowUi(bool show) { }


        private void UpdateProxyList()
        {
            // just ignore exception
            try
            {
                Owner.Form.ProxyDataGrid.Rows.Clear();
            }
            catch { return; }
            ProxiesList.Clear();

            var list = Owner.Core.Configs.Proxies;
            foreach (var proxy in list)
            {
                var e = proxy.GetEditable();
                ProxiesList.Add(e);

                var row = Owner.Form.ProxyDataGrid.Rows.Add(e.Ip, e.Port, e.Login, e.Password);
                Owner.Form.ProxyDataGrid.Rows[row].Tag = e;
            }
        }

        public void EditProxyValue(int col, int row)
        {
            // column name is name of property
            string valueName = Owner.Form.ProxyDataGrid.Columns[col].Name;
            string value = Owner.Form.ProxyDataGrid.Rows[row].Cells[col].Value as string;
            var e = Owner.Form.ProxyDataGrid.Rows[row].Tag as Proxy.EditableProxy;
            if (e == null) return;

            if (valueName == "Ip") e.Ip = value;
            else if (valueName == "Port")
            {
                int i = e.Port;
                try
                {
                    // try convert to int
                    e.Port = Convert.ToInt32(value);
                }
                catch
                {
                    // if fails - return to previous value
                    Owner.Form.ProxyDataGrid.Rows[row].Cells[col].Value = i;
                }
            }
            else if (valueName == "Login") e.Login = value;
            else if (valueName == "Password") e.Password = value;

            UpdateProxyList();
        }

        public void AddNewProxy()
        {
            var e = new Proxy("192.168.0.1", 999);
            Owner.Core.Configs.GetEditable().AddProxy(e);
            ProxiesList.Add(e.GetEditable());
            var row = Owner.Form.ProxyDataGrid.Rows.Add(e.Ip, e.Port, e.Login, e.Password);

            UpdateProxyList();
        }

        public void ClearProxies()
        {
            var confirm = MessageBox.Show($"Are you want clear Proxies list?", "Warning",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                Owner.Core.Configs.GetEditable().ClearProxies();
                UpdateProxyList();
            }
        }

        public void RemoveSelectedProxy()
        {
            var rows = Owner.Form.ProxyDataGrid.SelectedRows;
            if(rows != null)
            {
                var e = Owner.Core.Configs.GetEditable();
                for (int i = 0; i < rows.Count; i++)
                {
                    var r = rows[i];
                    Proxy.EditableProxy proxy = r.Tag as Proxy.EditableProxy;
                    e.RemoveProxy(proxy.EditableObject);
                    ProxiesList.Remove(proxy);
                }
            }
            UpdateProxyList();
        }
    }
}