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

            // bind tabs and controllers
            GeneralConfigsTabControll.TabPages["BotConfigsTabPage"].Tag = EditorController.GeneralConfigsTabController;
            GeneralConfigsTabControll.TabPages["CustomSettingsTabPage"].Tag = EditorController.CustomSettingsTabController;

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

        private void createNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditorController.CreateNew();
        }

        private void GeneralConfigsTabControll_Selecting(object sender, TabControlCancelEventArgs e)
        {
            var prevCntrl = GeneralConfigsTabControll.SelectedTab.Tag as BaseTabController;
            if (prevCntrl != null)
                prevCntrl.OnTabClose();

            var controller = e.TabPage.Tag as BaseTabController;
            if(controller != null) controller.OnTabOpen();
        }

        private void CustomValuesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            EditorController.CustomSettingsTabController.OnEntityChange();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditorController.Save();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditorController.SaveAs();
        }
    }
}
