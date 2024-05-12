using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CINEMA_APP.CinemaMainForm;

namespace CINEMA_APP
{
    public partial class toggleTables : Form
    {
        CinemaMainForm temp;
        public toggleTables(CinemaMainForm temp)
        {
            InitializeComponent();
            this.temp = temp;
            
        }


        private void toggleTables_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.ForeColor = Color.White;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.ForeColor = Color.Gray;
        }
        private void button2_MouseEnter(object sender, EventArgs e)
        {
            button2.ForeColor = Color.White;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.ForeColor = Color.Gray;
        }
        private void button3_MouseEnter(object sender, EventArgs e)
        {
            button3.ForeColor = Color.White;
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            button3.ForeColor = Color.Gray;
        }
        private void button4_MouseEnter(object sender, EventArgs e)
        {
            button4.ForeColor = Color.White;
        }

        private void button4_MouseLeave(object sender, EventArgs e)
        {
            button4.ForeColor = Color.Gray;
        }
        private void button5_MouseEnter(object sender, EventArgs e)
        {
            button5.ForeColor = Color.White;
        }

        private void button5_MouseLeave(object sender, EventArgs e)
        {
            button5.ForeColor = Color.Gray;
        }
        private void button6_MouseEnter(object sender, EventArgs e)
        {
            button6.ForeColor = Color.White;
        }

        private void button6_MouseLeave(object sender, EventArgs e)
        {
            button6.ForeColor = Color.Gray;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            temp.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            CinemasTable cinemasTable = new CinemasTable();

            cinemasTable.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FilmsTable filmsTable = new FilmsTable();
            filmsTable.Show();
            // Расчет новой позиции для Form2, чтобы прилипнуть к правой стороне Form1
            var newPosition = new Point(this.Right, this.Top);
            filmsTable.Location = newPosition;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            RowPlaceTable placeTable = new RowPlaceTable();
            placeTable.Show();


        }

        private void button4_Click(object sender, EventArgs e)
        {
            HallTable hallTable = new HallTable();

            hallTable.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SessionsTable sessionsTable = new SessionsTable();
            sessionsTable.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            TicketsTable tickets = new TicketsTable();
            tickets.Show();
        }

        private void toggleTables_Load(object sender, EventArgs e)
        {

        }
    }
}
