using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using TBotCore.Config;

namespace BotConfigurator
{
    class LanguageTabController : BaseTabController
    {
        EditorController Owner;
        TextProcessor.EditebleTextProcessor TextData;

        public LanguageTabController(EditorController owner)
        {
            Owner = owner;
        }


        public override void OnEntityChange()
        {
            
        }

        public override void OnTabClose()
        {
            
        }

        
        public override void OnTabOpen()
        {
            FillGrid();
        }

        public override void ShowUi(bool show)
        {
            // show buttons when selected items...
        }

        /// <summary>
        /// Fill data grid
        /// </summary>
        private void FillGrid()
        {
            TextData = Owner.Core.Configs.TextStrings.GetEditable();
            Owner.Form.LanguageDataGrid.Rows.Clear();
            Owner.Form.LanguageDataGrid.Columns.Clear();

            // create 1st column with text keys
            var column = Owner.Form.LanguageDataGrid.Columns.Add("text_keys", "Text Key");
            Owner.Form.LanguageDataGrid.Columns[column].ReadOnly = true;

            bool columnsCreated = false;

            // create dataGrid rows
            foreach (string key in TextData.EditableObject.TextStrings)
            {
                List<string> rowData = new List<string>();
                rowData.Add(key);

                foreach (string lang in TextData.EditableObject.Languages)
                {
                    // create language columns in datagrid
                    if (!columnsCreated)
                        Owner.Form.LanguageDataGrid.Columns.Add(lang, lang);

                    var v = TextData.EditableObject[lang, key];
                    rowData.Add(v);
                }

                Owner.Form.LanguageDataGrid.Rows.Add(rowData.ToArray());
                columnsCreated = true;
            }
        }

        /// <summary>
        /// When datagrid edit mode ends change correspondent value in collection
        /// </summary>
        public void EditValue(int col, int row)
        {
            // column name is lang
            string lang = Owner.Form.LanguageDataGrid.Columns[col].Name;
            string key =            // get txt string key by cell value
                Owner.Form.LanguageDataGrid.Rows[row].Cells[0].Value as string;

            string value = Owner.Form.LanguageDataGrid.Rows[row].Cells[col].Value as string;
            TextData.EditTextString(lang, key, value);
        }

        public void AddNewTextString()
        {
            var form = new AddValueForm("Add new TextString", x => !TextData.IsTextStringExist(x));
            if(form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextData.AddTextString(form.ResultValue);
                FillGrid();
            }
        }

        public void AddNewLanguage()
        {
            var form = new AddValueForm("Add new Language", x => !TextData.IsLanguageExist(x));
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextData.AddLanguage(form.ResultValue);
                FillGrid();
            }
        }

        public void RemoveTextString()
        {
            var cell = Owner.Form.LanguageDataGrid.SelectedCells;
            if (cell != null)
            {
                if (cell.Count == 0) return;
                int rowIndex = cell[0].RowIndex;

                string key = Owner.Form.LanguageDataGrid.Rows[rowIndex].Cells[0].Value as string;
                var confirm = MessageBox.Show($"Are you want delete text string:\r\n'{key}'","Warning",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if(confirm == DialogResult.Yes)
                {
                    TextData.RemoveTextString(key);
                    FillGrid();
                }
            }
        }

        public void RemoveLanguage()
        {
            var cell = Owner.Form.LanguageDataGrid.SelectedCells;
            if (cell != null)
            {
                if (cell.Count == 0) return;
                int colIndex = cell[0].ColumnIndex;
                
                string lang = Owner.Form.LanguageDataGrid.Columns[colIndex].Name as string;

                var confirm = MessageBox.Show($"Are you want delete language:\r\n'{lang}'", "Warning",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.Yes)
                {
                    TextData.RemoveLanguage(lang);
                    FillGrid();
                }
            }
        }
    }
}
