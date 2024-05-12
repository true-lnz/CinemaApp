using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CINEMA_APP
{
    public partial class Auth : Form
    {
        CinemaMainForm temp;
        public Auth(CinemaMainForm temp)
        {
            InitializeComponent();
            this.temp = temp;
        }

        string adminLogin = "админ";
        string password = "1234";

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            button2.ForeColor = Color.White;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.ForeColor = Color.Gray;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.ToLower() == adminLogin && textBox2.Text.ToLower() == password)
            {
                label2.ForeColor = SystemColors.Highlight;
                label2.Text = "Успех!";
                await Task.Delay(1500);
                toggleTables toggleTables = new toggleTables(temp);
                toggleTables.Show();
                this.Hide();
            }
            else
            {
                label2.Text = "Неверный пароль!";
                label2.ForeColor = Color.Red;
            }
        }

        private void Auth_FormClosing(object sender, FormClosingEventArgs e)
        {
            temp.Show();
        }

        private void Auth_Load(object sender, EventArgs e)
        {

        }
    }
}
