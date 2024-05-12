using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CINEMA_APP
{
    public partial class PersonDialog : Form
    {
        public PersonDialog()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
                DialogResult = DialogResult.OK;
            else
            {
                SystemSounds.Exclamation.Play();
                textBox1.Focus();
                textBox1.SelectionStart = 0;
            }
        }
    }
}
