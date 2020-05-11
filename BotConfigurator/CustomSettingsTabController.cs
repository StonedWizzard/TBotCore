using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TBotCore.Config;

namespace BotConfigurator
{
    class CustomSettingsTabController : BaseTabController
    {
        EditorController Owner;
        List<ConfigValue.EditebleConfigValue> ConfigValues;
        ConfigValue.EditebleConfigValue SelectedValue;

        public CustomSettingsTabController(EditorController owner)
        {
            Owner = owner;
            ConfigValues = new List<ConfigValue.EditebleConfigValue>();
            SelectedValue = null;
        }

        public void LoadEntities()
        {
            ConfigValues = Owner.Core.Configs.GetEditableValues();
        }

        private void SaveSelected()
        {
            if (SelectedValue != null)
            {
                try
                {
                    // if here all ok - we can continue
                    // otherwise show message and do nothing

                    Type t = Type.GetType(Owner.Form.CustomValueTypeBox.Text);
                    string obj = Owner.Form.CustomValueBox.Text;
                    object op = Convert.ChangeType(obj, t);
                }
                catch
                {
                    MessageBox.Show("Type/Value mismatch!\r\nChanges not saved!", "Error!");
                    return;
                }

                SelectedValue.Key = Owner.Form.CustomValueKeyBox.Text;
                SelectedValue.Description = Owner.Form.CustomValueDescriptionBox.Text;
                SelectedValue.Name = Owner.Form.CustomValueDisplayNameBox.Text;
                SelectedValue.Value = Owner.Form.CustomValueBox.Text;   // can be error - type mismatch, maybe
                SelectedValue.ValueType = Type.GetType(Owner.Form.CustomValueTypeBox.Text);
            }
        }

        public override void OnEntityChange()
        {
            if (Owner.Form.CustomValuesList.SelectedItems.Count == 0)
            {
                ShowUi(false);
                return;
            }
            // save current selection changes
            SaveSelected();

            // pickup new selection
            SelectedValue = Owner.Form.CustomValuesList.SelectedItems[0].Tag as ConfigValue.EditebleConfigValue;

            Owner.Form.CustomValueBox.Text = SelectedValue.Value.ToString();
            Owner.Form.CustomValueKeyBox.Text = SelectedValue.Key.ToString();
            Owner.Form.CustomValueTypeBox.Text = SelectedValue.ValueType.ToString();
            // handle flags by other way!!!!!!!!!
            //Owner.Form.CustomValueFlagsBox.Text = SelectedValue.Value.ToString();
            Owner.Form.CustomValueDisplayNameBox.Text = SelectedValue.Name.ToString();
            Owner.Form.CustomValueDescriptionBox.Text = SelectedValue.Description.ToString();

            ShowUi(true);
        }

        public override void OnTabClose()
        {
            //save state of selected value
            SaveSelected();
        }

        public override void OnTabOpen()
        {
            Owner.Form.CustomValuesList.Clear();
            foreach (var v in ConfigValues)
            {
                var lstItm = Owner.Form.CustomValuesList.Items.Add(v.ToString());
                lstItm.Tag = v;
            }
            ShowUi(false);
        }

        public override void ShowUi(bool show)
        {
            Owner.Form.CustomValueBox.Visible = show;
            Owner.Form.CustomValueKeyBox.Visible = show;
            Owner.Form.CustomValueTypeBox.Visible = show;
            Owner.Form.CustomValueFlagsBox.Visible = show;
            Owner.Form.CustomValueDisplayNameBox.Visible = show;
            Owner.Form.CustomValueDescriptionBox.Visible = show;

            //labels hide/show also
            Owner.Form.label4.Visible = show;
            Owner.Form.label5.Visible = show;
            Owner.Form.label7.Visible = show;
            Owner.Form.label6.Visible = show;
            Owner.Form.label9.Visible = show;
            Owner.Form.label8.Visible = show;
        }
    }
}
