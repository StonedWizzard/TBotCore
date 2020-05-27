using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TBotCore;
using TBotCore.Editor;
using TBotCore.Config;
using System.Windows.Forms;
using TBotCore.Core.Dialogs;

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

        /// <summary>
        /// List of registered values types to choose in editor
        /// </summary>
        public readonly string[] ValueTypes = new[]
        {
            typeof(bool).ToString(),
            typeof(int).ToString(),
            typeof(long).ToString(),
            typeof(double).ToString(),
            typeof(string).ToString(),
        };

        /// <summary>
        /// List of registered dialog types to choose in editor
        /// </summary>
        public readonly Dictionary<string, Type> DialogTypes = new Dictionary<string, Type>()
        {
            {"Dialog" ,typeof(Dialog) },
            {"PaginatedDialog" ,typeof(PaginatedDialog) },
            {"SerialDialog" ,typeof(SerialDialog) },
            {"RootDialog" ,typeof(RootDialog) },
            {"RegistrationDialog" ,typeof(RegistrationDialog) },
        };

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

        string _FileName;
        public string FileName 
        {
            get => _FileName;
            private set
            {
                _FileName = value;

                if (Form != null)
                {
                    string s = String.IsNullOrEmpty(_FileName) ? "" : _FileName;    // file name placer
                    Form.Text = $"Bot Configurator [{s}]";
                }
            }
        }

        /// <summary>
        /// List of child tab controllers
        /// It allow us call tabSwitchMethods, to keep and save changes
        /// </summary>
        private List<BaseTabController> Childs;
        public GeneralConfigsTabController GeneralConfigsTabController;
        public CustomSettingsTabController CustomSettingsTabController;
        public LanguageTabController LanguageTabController;
        public DialogsTabController DialogsTabController;
        public SupportButtonsTabController SupportButtonsTabController;

        /// <summary>
        /// Controller constructor
        /// </summary>
        /// <param name="form">reference to window where all ui magic happens</param>
        public EditorController(MainEditorForm form)
        {
            IsLoaded = false;
            FileName = null;
            Core = new EditorCore();
            Form = form;
            
            GeneralConfigsTabController = new GeneralConfigsTabController(this);
            CustomSettingsTabController = new CustomSettingsTabController(this);
            LanguageTabController = new LanguageTabController(this);
            DialogsTabController = new DialogsTabController(this);
            SupportButtonsTabController = new SupportButtonsTabController(this);

            Childs = new List<BaseTabController>();
            Childs.Add(GeneralConfigsTabController);
            Childs.Add(CustomSettingsTabController);
            Childs.Add(LanguageTabController);
            Childs.Add(DialogsTabController);
            Childs.Add(SupportButtonsTabController);
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

        /// <summary>
        /// Save existed data to early opened file
        /// or show save dialog
        /// </summary>
        public void Save()
        {
            if (!IsLoaded) return;

            // call 'save as'
            if(String.IsNullOrEmpty(FileName))
                SaveAs();
            
            else
                _Save(FileName);
        }

        /// <summary>
        /// Open SaveAs dialog to save data as new file
        /// </summary>
        public void SaveAs()
        {
            if (!IsLoaded) return;
            
            var result = Form.SaveConfigDialog.ShowDialog();
            if(result == DialogResult.OK)
            {
                _Save(Form.SaveConfigDialog.FileName);
            }
        }

        private void _Save(string path)
        {
            // save all changes by call controller childs OnTabClose()
            foreach (var tab in Childs)
                tab.OnTabClose();

            ConfigSerializer worker = new ConfigSerializer(path);
            bool opRes = worker.SaveConfig(Core.Configs, Core.Dialogs);
            if (!opRes)
            {
                MessageBox.Show("Not saved!", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            FileName = path;
        }

        /// <summary>
        /// Shows file choose dialog to load configs
        /// </summary>
        public void OpenConfigs()
        {
            // show notification if some configs currently opened
            if(IsLoaded)
            {
                var confirm = MessageBox.Show($"File [{FileName}] is currently open!\r\nOpening new configs will result in loss of unsaved data!\r\nContinue?",
                    "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (confirm != DialogResult.Yes) return;
            }

            var result = Form.OpenConfigDialog.ShowDialog();
            if(result == DialogResult.OK)
            {
                ConfigSerializer worker = new ConfigSerializer(Form.OpenConfigDialog.FileName);
                var opRes = worker.ReadConfigs();
                if (!opRes.Item1)
                {
                    MessageBox.Show("Can't open file", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Core = new EditorCore(opRes.Item2, opRes.Item3);
                FileName = Form.OpenConfigDialog.FileName;
                IsLoaded = true;
            }
        }
    }
}
