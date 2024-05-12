using PdfSharp.Charting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CINEMA_APP
{
    public partial class ViewReports : Form
    {
        int mode = 0;
        public ViewReports(int mode)
        {
            InitializeComponent();
            this.mode = mode;
        }
        SqlDataAdapter dataAdapter = new SqlDataAdapter();
        DataTable dataTable = new DataTable();
        private void TopSession_Load(object sender, EventArgs e)
        {
            string query = "SELECT * FROM SessionSuccessView;";
            if (mode == 1)
                query = "SELECT * FROM CurrentMonthSessionsView;";
            else if (mode == 2)
                query = "SELECT * FROM TopRatedMoviesView;";
            else if (mode == 3)
                query = "SELECT * FROM TopSellingMoviesView;";

            using (SqlConnection connection = new SqlConnection(Program.connectionString))
            {
                try
                {
                    connection.Open();

                    dataAdapter.SelectCommand = new SqlCommand(query, new SqlConnection(Program.connectionString));

                    dataAdapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;
                    if (mode == 1)
                        dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Ascending);
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Произошла ошибка при получении данных: " + ex.Message);
                }
            }
        }
    }
}
