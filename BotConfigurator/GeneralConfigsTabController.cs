using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotConfigurator
{
    class GeneralConfigsTabController : BaseTabController
    {
        EditorController Owner;

        public GeneralConfigsTabController(EditorController owner)
        {
            Owner = owner;
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
        }

        /// <summary>
        /// Always display ui fields to edit, so nothing to do
        /// </summary>
        public override void ShowUi(bool show) { }
    }
}
