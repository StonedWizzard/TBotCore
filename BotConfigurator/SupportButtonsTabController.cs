using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using TBotCore.Core.Dialogs;

namespace BotConfigurator
{
    class SupportButtonsTabController : BaseTabController
    {
        EditorController Owner;
        TBotCore.Core.Dialogs.Button.EditableButton SelectedItem;

        public SupportButtonsTabController(EditorController owner)
        {
            Owner = owner;
            ShowUi(false);
        }

        public override void OnEntityChange()
        {
            // save old selected item
            SaveSelected();

            // select new one
            SelectedItem = null;
            if (Owner.Form.SupportButtonsListView.SelectedItems.Count > 0)
                SelectedItem = Owner.Form.SupportButtonsListView.SelectedItems[0].Tag as TBotCore.Core.Dialogs.Button.EditableButton;

            Owner.Form.RemoveDialogButton.Enabled = SelectedItem != null;
            DisplaySelected();
        }

        public override void OnTabClose()
        {
            bool result = SaveSelected();
            if (!result)
            {
                // show message box
            }
        }

        public override void OnTabOpen()
        {
            Owner.Form.SupportButtonsListView.Clear();

            foreach (var btn in Owner.Core.Dialogs.GetButtons())
            {
                var itm = Owner.Form.SupportButtonsListView.Items.Add(btn.Id);
                itm.Name = btn.Id;
                itm.Tag = btn.GetEditable();
            }
        }

        public override void ShowUi(bool show)
        {
            Owner.Form.ButtonIdBox.Visible = show;
            Owner.Form.ButtondisplayedName.Visible = show;
            Owner.Form.ButtonDisplayPriorityBox.Visible = show;
            Owner.Form.ButtonDataBox.Visible = show;

            // labels also
            Owner.Form.label23.Visible = show;
            Owner.Form.label22.Visible = show;
            Owner.Form.label21.Visible = show;
            Owner.Form.label20.Visible = show;
        }



        private bool SaveSelected()
        {
            if (SelectedItem != null)
            {
                SelectedItem.Id = Owner.Form.ButtonIdBox.Text;
                SelectedItem.DisplayedName = Owner.Form.ButtondisplayedName.Text;
                SelectedItem.Data = Owner.Form.ButtonDataBox.Text;
                SelectedItem.DisplayPriority = (int)Owner.Form.ButtonDisplayPriorityBox.Value;
                return true;
            }
            return false;
        }

        private void DisplaySelected()
        {
            if (SelectedItem == null)
            {
                ShowUi(false);
                return;
            }

            Owner.Form.ButtonIdBox.Text = SelectedItem.Id;
            Owner.Form.ButtondisplayedName.Text = SelectedItem.DisplayedName;
            Owner.Form.ButtonDataBox.Text = SelectedItem.Data;

            //controll priority value
            if (SelectedItem.DisplayPriority < Owner.Form.ButtonDisplayPriorityBox.Minimum ||
                SelectedItem.DisplayPriority > Owner.Form.ButtonDisplayPriorityBox.Maximum)
            {
                SelectedItem.DisplayPriority = (int)Owner.Form.ButtonDisplayPriorityBox.Minimum;
            }
            Owner.Form.ButtonDisplayPriorityBox.Value = SelectedItem.DisplayPriority;

            ShowUi(true);
        }


        public void CreateButton()
        {
            Owner.Core.Dialogs.GetEditable().InsertButton();

            // refresh list
            OnTabOpen();
        }

        public void RemoveButton()
        {
            Owner.Core.Dialogs.GetEditable().RemoveButton(SelectedItem.Id);
            OnTabOpen();
        }
    }
}
