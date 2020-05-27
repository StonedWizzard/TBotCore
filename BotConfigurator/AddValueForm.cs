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
    public partial class AddValueForm : Form
    {
        public string ResultValue { get; private set; }
        private Func<string, bool> Validator;

        public AddValueForm()
        {
            InitializeComponent();
        }
        public AddValueForm(string header, Func<string, bool> validator) : this()
        {
            this.Text = header;
            Validator = validator;
        }

        private void CancellButon_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrEmpty(ValueKeyBox.Text))
            {
                MessageBox.Show("Key/Id field is empty!");
                return;
            }

            if(Validator != null)
            {
                if(Validator(ValueKeyBox.Text) == false)
                {
                    MessageBox.Show("Key/Id field is invalid\r\nPosible such Id already exist!");
                    return;
                }
            }

            ResultValue = ValueKeyBox.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
