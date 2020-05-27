using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BotConfigurator
{
    public partial class MainEditorForm : Form
    {
        EditorController EditorController;

        public MainEditorForm()
        {
            InitializeComponent();
            EditorController = new EditorController(this);

            // initialize allowed types for customValues and dialog
            CustomValueTypeBox.Items.AddRange(EditorController.ValueTypes);
            DialogTypeBox.Items.AddRange(EditorController.DialogTypes.Keys.ToArray());

            // bind tabs and controllers
            GeneralConfigsTabControll.TabPages["BotConfigsTabPage"].Tag = EditorController.GeneralConfigsTabController;
            GeneralConfigsTabControll.TabPages["CustomSettingsTabPage"].Tag = EditorController.CustomSettingsTabController;
            GeneralConfigsTabControll.TabPages["LanguageConfigsTabPage"].Tag = EditorController.LanguageTabController;
            GeneralConfigsTabControll.TabPages["DialogsConfigTabPage"].Tag = EditorController.DialogsTabController;
            GeneralConfigsTabControll.TabPages["SupportButtonsTabPage"].Tag = EditorController.SupportButtonsTabController;

            EditorController.OnLoadStateChanged += EditorController_OnLoadStateChanged;
        }

        /// <summary>
        /// Show/hide whole ui when configs load/closed
        /// </summary>
        private void EditorController_OnLoadStateChanged(object sender, EventArgs e)
        {
            GeneralConfigsTabControll.Visible = EditorController.IsLoaded;

            // emulate opening first tab on loading
            if (EditorController.IsLoaded)
            {
                var controller = GeneralConfigsTabControll.SelectedTab.Tag as BaseTabController;
                if (controller != null) controller.OnTabOpen();
            }
        }

        private void GeneralConfigsTabControll_Selecting(object sender, TabControlCancelEventArgs e)
        {
            var prevCntrl = GeneralConfigsTabControll.SelectedTab.Tag as BaseTabController;
            if (prevCntrl != null)
                prevCntrl.OnTabClose();

            var controller = e.TabPage.Tag as BaseTabController;
            if (controller != null) controller.OnTabOpen();
        }

        private void ProxyDataGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            EditorController.GeneralConfigsTabController.EditProxyValue(e.ColumnIndex, e.RowIndex);
        }

        private void AddNewProxyBtn_Click(object sender, EventArgs e)
        {
            EditorController.GeneralConfigsTabController.AddNewProxy();
        }

        private void RemoveSelectedProxyBtn_Click(object sender, EventArgs e)
        {
            EditorController.GeneralConfigsTabController.RemoveSelectedProxy();
        }

        private void ClearProxyListBtn_Click(object sender, EventArgs e)
        {
            EditorController.GeneralConfigsTabController.ClearProxies();
        }

        #region ToolStrip
        private void createNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditorController.CreateNew();
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditorController.Save();
        }
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditorController.SaveAs();
        }
        private void openConfigsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditorController.OpenConfigs();
        }
        #endregion

        #region CustomValues
        private void CustomValuesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            EditorController.CustomSettingsTabController.OnEntityChange();
        }
        private void AddNewCustomValueButton_Click(object sender, EventArgs e)
        {
            EditorController.CustomSettingsTabController.AddNewValue();
        }
        private void RemoveCustomValueButton_Click(object sender, EventArgs e)
        {
            EditorController.CustomSettingsTabController.RemoveValue();
        }
        #endregion

        #region LanguageTab
        private void LanguageDataGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            EditorController.LanguageTabController.EditValue(e.ColumnIndex, e.RowIndex);
        }
        private void AddNewLangStrBtn_Click(object sender, EventArgs e)
        {
            EditorController.LanguageTabController.AddNewTextString();
        }
        private void AddNewLangBtn_Click(object sender, EventArgs e)
        {
            EditorController.LanguageTabController.AddNewLanguage();
        }
        private void RemoveStringButton_Click(object sender, EventArgs e)
        {
            EditorController.LanguageTabController.RemoveTextString();
        }
        private void RemoveLanguageButton_Click(object sender, EventArgs e)
        {
            EditorController.LanguageTabController.RemoveLanguage();
        }




        #endregion

        #region Dialogs
        private void DialogsBox_AfterSelect(object sender, TreeViewEventArgs e)
        {
            EditorController.DialogsTabController.OnEntityChange();
        }

        private void AddDialogButton_Click(object sender, EventArgs e)
        {
            EditorController.DialogsTabController.AddDialog();
        }

        private void RemoveDialogButton_Click(object sender, EventArgs e)
        {
            EditorController.DialogsTabController.RemoveDialog();
        }

        private void DialogsBox_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            // save old selected item
            EditorController.DialogsTabController.SaveSelected();
        }

        private void DialogContentPageBox_ValueChanged(object sender, EventArgs e)
        {
            EditorController.DialogsTabController.OnContentPageChanged();
        }

        private void AddDialogPageBtn_Click(object sender, EventArgs e)
        {
            EditorController.DialogsTabController.AddContentPage();
        }

        private void RemoveDialogPageBtn_Click(object sender, EventArgs e)
        {
            EditorController.DialogsTabController.RemoveContentPage();
        }
        #endregion


        private void SupportButtonsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            EditorController.SupportButtonsTabController.OnEntityChange();
        }

        private void AddSupBtnButton_Click(object sender, EventArgs e)
        {
            EditorController.SupportButtonsTabController.CreateButton();
        }

        private void RemoveSupBtnButton_Click(object sender, EventArgs e)
        {
            EditorController.SupportButtonsTabController.CreateButton();
        }
    }
}
