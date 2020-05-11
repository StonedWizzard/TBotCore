using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TBotCore;
using TBotCore.Editor;
using TBotCore.Config;
using System.Windows.Forms;

namespace BotConfigurator
{
    /// <summary>
    /// General controller over whole config window
    /// </summary>
    class EditorController
    {
        /// <summary>
        /// Editor core, wich provide acces to change work-on data
        /// </summary>
        public EditorCore Core { get; private set; }
        public readonly MainEditorForm Form;

        bool _isLoaded;
        public bool IsLoaded 
        { 
            get => _isLoaded;
            private set
            {
                _isLoaded = value;
                OnLoadStateChanged?.Invoke(this, null);
            }
        }
        public string FileName { get; private set; }


        public GeneralConfigsTabController GeneralConfigsTabController;
        public CustomSettingsTabController CustomSettingsTabController;

        /// <summary>
        /// Controller constructor
        /// </summary>
        /// <param name="form">reference to window where all ui magic happens</param>
        public EditorController(MainEditorForm form)
        {
            IsLoaded = false;
            FileName = null;
            Core = null;
            Form = form;

            GeneralConfigsTabController = new GeneralConfigsTabController(this);
            CustomSettingsTabController = new CustomSettingsTabController(this);
        }

        // ***EVENTS***
        public event EventHandler OnLoadStateChanged;

        /// <summary>
        /// Create new instance of edited data
        /// </summary>
        public void CreateNew()
        {
            Core = new EditorCore();
            CustomSettingsTabController.LoadEntities();

            IsLoaded = true;
        }

        public void Save()
        {
            if (!IsLoaded) return;

            // call 'save as'
            if(String.IsNullOrEmpty(FileName))
                SaveAs();
            
            else
            {
                ConfigWorker worker = new ConfigWorker(null, Form.SaveConfigDialog.FileName);
                worker.SaveConfig(Core.Configs);
            }
        }

        public void SaveAs()
        {
            if (!IsLoaded) return;
            var result = Form.SaveConfigDialog.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.OK)
            {
                ConfigWorker worker = new ConfigWorker(null, Form.SaveConfigDialog.FileName);
                bool opRes = worker.SaveConfig(Core.Configs);
                if(!opRes)
                {
                    //MessageBox.Show("");
                }
            }
        }
    }
}
