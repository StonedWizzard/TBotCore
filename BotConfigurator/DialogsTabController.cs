using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using TBotCore.Core.Dialogs;

namespace BotConfigurator
{
    class DialogsTabController : BaseTabController
    {
        EditorController Owner;
        Dialog.EditableDialog SelectedItem;
        int CurrentPage;

        public DialogsTabController(EditorController owner)
        {
            Owner = owner;
            ShowUi(false);
        }

        public override void OnEntityChange()
        {
            // fill support buttons list
            Owner.Form.DialogFlagBox.Items.Clear();
            foreach (var v in Owner.Core.Dialogs.GetButtons())
                Owner.Form.DialogFlagBox.Items.Add(v, false);

            // select new one
            try
            {
                if(Owner.Form.DialogsBox.SelectedNode.Tag is PaginatedDialog)
                    SelectedItem = (Owner.Form.DialogsBox.SelectedNode.Tag as PaginatedDialog).GetEditable();
                else
                    SelectedItem = (Owner.Form.DialogsBox.SelectedNode.Tag as Dialog).GetEditable();
                CurrentPage = 0;
            }
            catch { SelectedItem = null; }

            ShowUi(SelectedItem != null);
            if (SelectedItem != null)
            {
                DisplaySelected();
                SetEditableState();
            }
        }

        public override void OnTabClose()
        {
            SaveSelected();
        }

        public override void OnTabOpen()
        {
            FillDialogsTree();
            OnEntityChange();
        }

        public override void ShowUi(bool show)
        {
            Owner.Form.DialogIdBox.Visible = show;
            Owner.Form.DialogNameBox.Visible = show;
            Owner.Form.DialogDisplayOrderBox.Visible = show;
            Owner.Form.DialogTypeBox.Visible = show;
            Owner.Form.DialogFlagBox.Visible = show;
            Owner.Form.DialogOperationBox.Visible = show;
            Owner.Form.DialogDataBox.Visible = show;
            Owner.Form.DialogContentPageBox.Visible = show;
            Owner.Form.DialogContentBox.Visible = show;

            // labels also
            Owner.Form.label10.Visible = show;
            Owner.Form.label11.Visible = show;
            Owner.Form.label12.Visible = show;
            Owner.Form.label14.Visible = show;
            Owner.Form.label13.Visible = show;
            Owner.Form.label17.Visible = show;
            Owner.Form.label18.Visible = show;
            Owner.Form.label15.Visible = show;
            Owner.Form.label16.Visible = show;
        }

        /// <summary>
        /// Fill dialogs nodes
        /// </summary>
        private void FillDialogsTree()
        {
            // save name of old node to select it
            string oldNode = Owner.Form.DialogsBox.SelectedNode?.Name;
            Owner.Form.DialogsBox.Nodes.Clear();

            // create root node
            var rootDia = Owner.Core.Dialogs.RootDialog;
            TreeNode rootNode = new TreeNode();
            rootNode.Tag = rootDia;
            rootNode.Text = rootDia.Id;
            rootNode.Name = rootDia.Id;
            AddChilds(rootNode);

            // create registration node
            var regDia = Owner.Core.Dialogs.RegistrationDialog;
            TreeNode regNode = new TreeNode();
            regNode.Tag = regDia;
            regNode.Text = regDia.Id;
            regNode.Name = regDia.Id;
            AddChilds(regNode);

            Owner.Form.DialogsBox.Nodes.Add(rootNode);
            Owner.Form.DialogsBox.Nodes.Add(regNode);

            SelectedItem = null;
            ShowUi(false);
            Owner.Form.DialogsBox.ExpandAll();

            // try select old node
            var node = Owner.Form.DialogsBox.Nodes.Find(oldNode, true);
            if (node.Count() > 0)
            {
                Owner.Form.DialogsBox.SelectedNode = node.First();
                OnEntityChange();
            }
        }
        /// <summary>
        /// Calls from FillDialogs
        /// </summary>
        private void AddChilds(TreeNode node)
        {
            Dialog container = node.Tag as Dialog;
            if (container == null) return;

            foreach(Dialog dialog in container.Dialogs)
            {
                TreeNode child = new TreeNode();
                child.Tag = dialog;
                child.Text = dialog.Id;
                child.Name = dialog.Id;

                // use recursion to fill this node
                AddChilds(child);
                node.Nodes.Add(child);
            }
        }

        /// <summary>
        /// Save changes to selected item
        /// </summary>
        public bool SaveSelected()
        {
            if(SelectedItem != null)
            {
                bool idCahnged = SelectedItem.Id != Owner.Form.DialogIdBox.Text;

                SelectedItem.Id = Owner.Form.DialogIdBox.Text;
                SelectedItem.DisplayedName = Owner.Form.DialogNameBox.Text;
                SelectedItem.Operation = new TBotCore.Core.Operations.OperationFiller(Owner.Form.DialogOperationBox.Text);
                SelectedItem.Data = Owner.Form.DialogDataBox.Text;
                SelectedItem.DisplayPriority = (int)Owner.Form.DialogDisplayOrderBox.Value;
                SelectedItem.SupportButtons = Owner.Form.DialogFlagBox.CheckedItems.Cast<TBotCore.Core.Dialogs.Button>().ToList();

                var isCasted = Owner.Core.Dialogs.GetEditable().
                    CastDialog(SelectedItem.Id, Owner.DialogTypes[Owner.Form.DialogTypeBox.Text]); //<<<----- type conversion
                if (isCasted.Item1)     //change node reference
                    SelectedItem = isCasted.Item2.GetEditable();

                if(isCasted.Item1 || idCahnged)
                {
                    Owner.Form.DialogsBox.SelectedNode.Tag = SelectedItem.EditableObject;
                    Owner.Form.DialogsBox.SelectedNode.Text = SelectedItem.Id;
                    Owner.Form.DialogsBox.SelectedNode.Name = SelectedItem.Id;
                }

                // work with pagination
                if (SelectedItem.EditableObject is PaginatedDialog)
                {
                    var selectedDia = SelectedItem as PaginatedDialog.EditablePaginatedDialog;
                    if (selectedDia == null) return false;

                    var maxIndx = selectedDia.Content.Count;
                    int page = (int)Owner.Form.DialogContentPageBox.Value;
                    if (page < maxIndx)
                    {
                        selectedDia.Content[page] = Owner.Form.DialogContentBox.Text;
                        CurrentPage = page;
                    }
                    else
                        return false;
                }
                else
                {
                    SelectedItem.Content = Owner.Form.DialogContentBox.Text;
                }

                return true;
            }
            return false;
        }

        private void DisplaySelected()
        {
            Owner.Form.DialogIdBox.Text = SelectedItem.Id;
            Owner.Form.DialogNameBox.Text = SelectedItem.DisplayedName;
            Owner.Form.DialogTypeBox.Text =
                Owner.DialogTypes.FirstOrDefault(x => x.Value == SelectedItem.EditableObject.GetType()).Key;
            Owner.Form.DialogOperationBox.Text = SelectedItem.Operation?.ToString();
            Owner.Form.DialogDataBox.Text = SelectedItem.Data;

            //controll priority value
            if(SelectedItem.DisplayPriority < Owner.Form.DialogDisplayOrderBox.Minimum ||
                SelectedItem.DisplayPriority > Owner.Form.DialogDisplayOrderBox.Maximum)
            {
                SelectedItem.DisplayPriority = (int)Owner.Form.DialogDisplayOrderBox.Minimum;
            }
            Owner.Form.DialogDisplayOrderBox.Value = SelectedItem.DisplayPriority;

            // mark selected support buttons
            var slectedBtns = Owner.Form.DialogFlagBox.Items.Cast<TBotCore.Core.Dialogs.Button>().Intersect(SelectedItem.SupportButtons).ToList();
            foreach (var slct in slectedBtns)
            {
                int indx = Owner.Form.DialogFlagBox.Items.IndexOf(slct);
                Owner.Form.DialogFlagBox.SetItemChecked(indx, true);
            }

            if (SelectedItem.EditableObject is PaginatedDialog)
            {
                var selectedDia = SelectedItem as PaginatedDialog.EditablePaginatedDialog;
                if (selectedDia == null) return;

                var maxIndx = selectedDia.Content.Count;
                Owner.Form.DialogContentPageBox.Maximum = maxIndx - 1;
                Owner.Form.DialogContentBox.Text = selectedDia.Content[0];
                CurrentPage = 0;
            }
            else Owner.Form.DialogContentBox.Text = SelectedItem.Content;
        }

        /// <summary>
        /// Depends on selected item enable/disable fields
        /// It allow to protect from changes Root and Registration dialogs
        /// </summary>
        private void SetEditableState()
        {
            bool enable = !(SelectedItem.EditableObject is RootDialog || 
                SelectedItem.EditableObject is RegistrationDialog);

            Owner.Form.DialogIdBox.Enabled = enable;
            Owner.Form.DialogNameBox.Enabled = enable;
            Owner.Form.DialogTypeBox.Enabled = enable;
            Owner.Form.RemoveDialogButton.Enabled = enable;

            bool enablePagination = SelectedItem.EditableObject is PaginatedDialog;
            Owner.Form.DialogContentPageBox.Visible = enablePagination;
            Owner.Form.label16.Visible = enablePagination;
        }


        /// <summary>
        /// Adds new dialog to selected node
        /// or root node by default
        /// </summary>
        public void AddDialog()
        {
            string parentDia = SelectedItem != null ? SelectedItem.Id : "Root";
            int r = Owner.Core.Dialogs.GetNumSuffix();
            bool result = Owner.Core.Dialogs.GetEditable().InsertDialog(new Dialog($"new_dialog{r}"), parentDia);
            if(!result)
            {
                MessageBox.Show("Can't add dialog!", "Error!");
                return;
            }
            FillDialogsTree();
            OnEntityChange();
        }

        public void RemoveDialog()
        {
            if(SelectedItem != null)
            {
                if (MessageBox.Show($"Are you realy want to delete dialog '{SelectedItem.Id}' and all it's childs?",
                    "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes) { return; }
                    
                bool result = false;
                if ((SelectedItem.EditableObject is RootDialog || SelectedItem.EditableObject is RegistrationDialog) == false)
                {
                    result = Owner.Core.Dialogs.GetEditable().RemoveDialog(SelectedItem.Id);
                    FillDialogsTree();
                    OnEntityChange();
                }

                if (!result)
                    MessageBox.Show("Can't remove dialog!", "Error!");
            }
        }

        public void OnContentPageChanged()
        {
            if (SelectedItem == null) return;
            if ((SelectedItem.EditableObject is PaginatedDialog) == false) return;

            var selectedDia = SelectedItem as PaginatedDialog.EditablePaginatedDialog;
            if (selectedDia == null) return;
            var maxIndx = selectedDia.Content.Count;
            bool removeFlag = maxIndx != Owner.Form.DialogContentPageBox.Maximum;

            // save previous value if not deleted!
            if (CurrentPage < maxIndx && removeFlag)
                selectedDia.Content[CurrentPage] = Owner.Form.DialogContentBox.Text;

            // update max value
            Owner.Form.DialogContentPageBox.Maximum = maxIndx - 1;

            CurrentPage = (int)Owner.Form.DialogContentPageBox.Value;
            CurrentPage = CurrentPage < 0 ? 0 : CurrentPage;
            CurrentPage = CurrentPage < maxIndx ? CurrentPage : maxIndx - 1;
            Owner.Form.DialogContentBox.Text = selectedDia.Content[CurrentPage];
        }

        public void RemoveContentPage()
        {
            if (SelectedItem == null) return;
            if ((SelectedItem.EditableObject is PaginatedDialog) == false) return;

            var selectedDia = SelectedItem as PaginatedDialog.EditablePaginatedDialog;
            if (selectedDia == null) return;

            int page = (int)Owner.Form.DialogContentPageBox.Value;
            selectedDia.Content.RemoveAt(page);

            Owner.Form.DialogContentPageBox.Value = page - 1 >= 0 ? page - 1 : page;
            if (page == 0)
                OnContentPageChanged();
        }

        public void AddContentPage()
        {
            if (SelectedItem == null) return;
            if ((SelectedItem.EditableObject is PaginatedDialog) == false) return;

            var selectedDia = SelectedItem as PaginatedDialog.EditablePaginatedDialog;
            if (selectedDia == null) return;

            selectedDia.Content.Add("");

            Owner.Form.DialogContentPageBox.Maximum += 1;
            Owner.Form.DialogContentPageBox.Value += 1;
        }
    }
}
