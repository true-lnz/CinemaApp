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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CINEMA_APP
{
    public partial class TicketsTable : Form
    {
        public TicketsTable()
        {
            InitializeComponent();
        }
        SqlDataAdapter dataAdapter = new SqlDataAdapter();
        DataTable dataTable = new DataTable();

        public void LoadTable()
        {
            string query = "SELECT * FROM Ticket"; // Пример запроса на выборку всех фильмов

            using (SqlConnection connection = new SqlConnection(Program.connectionString))
            {
                try
                {
                    connection.Open();

                    dataAdapter.SelectCommand = new SqlCommand(query, new SqlConnection(Program.connectionString));

                    SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);

                    dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                    dataAdapter.InsertCommand = commandBuilder.GetInsertCommand();
                    dataAdapter.DeleteCommand = commandBuilder.GetDeleteCommand();

                    dataAdapter.Fill(dataTable);

                    comboBox2.DataSource = dataTable;
                    comboBox2.DisplayMember = "Session_id";
                    comboBox2.ValueMember = "Session_id";

                    bindingSource1.DataSource = dataTable;
                    dataGridView1.DataSource = bindingSource1;
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Произошла ошибка при получении данных: " + ex.Message);
                }
                dataGridView1.Columns[0].HeaderText = "Номер билета";
                dataGridView1.Columns[1].HeaderText = "Цена";
                dataGridView1.Columns[2].HeaderText = "Номер места";
                dataGridView1.Columns[3].Visible = false;
                dataGridView1.Columns[4].HeaderText = "Номер сессии";
                dataGridView1.Columns[5].HeaderText = "Покупатель";
                DataGridViewTextBoxColumn comboBoxColumn = new DataGridViewTextBoxColumn();
                comboBoxColumn.Name = "Row";
                comboBoxColumn.HeaderText = "Ряд";
                dataGridView1.Columns.Insert(6, comboBoxColumn);
                dataGridView1.Columns[6].DisplayIndex = 3;
                dataGridView1.Columns[2].DisplayIndex = 4;
                dataGridView1.Columns[4].DisplayIndex = 5;
                dataGridView1.Columns[5].DisplayIndex = 6;
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                dataAdapter.Update((DataTable)bindingSource1.DataSource);
                isSaveNeeded = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении изменений: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bindingSource1.RemoveCurrent();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DataRow newRow = dataTable.NewRow();

            // Задать значения для новой строки
            newRow[1] = 0;
            newRow[2] = 0;

            // Добавить новую строку в DataTable
            dataTable.Rows.Add(newRow);

            button5_Click(null, new EventArgs());

            bindingSource1.MoveLast();
        }
        static decimal GetTicketPrice(int sessionId)
        {
            decimal ticketPrice = 0;

            using (SqlConnection connection = new SqlConnection(Program.connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT dbo.GetBasePrice() AS CurrentBasePrice;", connection))
                {
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        ticketPrice = Convert.ToDecimal(result);
                    }
                }
            }

            return ticketPrice;
        }
        private void TicketsTable_Load(object sender, EventArgs e)
        {
            LoadTable();
        }

        bool isSaveNeeded = false;
        private void TicketsTable_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isSaveNeeded == true)
            {
                if (MessageBox.Show("Сохранить изменения?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    e.Cancel = true;
                    button5_Click(null, new EventArgs());
                }
            }
        }

        private void button5_MouseEnter(object sender, EventArgs e)
        {
            button5.ForeColor = Color.White;

        }

        private void button5_MouseLeave(object sender, EventArgs e)
        {

            button5.ForeColor = Color.Gray;
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.ForeColor = Color.White;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.ForeColor = Color.Gray;
        }
        private void UpdateRowById(int id, string d)
        {
            // Найти строку в DataGridView по идентификатору (1 столбец)
            DataGridViewRow rowToUpdate = null;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[0].Value != null && int.TryParse(row.Cells[0].Value.ToString(), out int rowId) && rowId == id)
                {
                    rowToUpdate = row;
                    break;
                }
            }

            // Если строка найдена, обновить данные
            if (rowToUpdate != null)
            {
                // Обновление данных в найденной строке
                rowToUpdate.Cells[5].Value = d;

                // Опционально: обновить отображение данных в DataGridView
                dataGridView1.Refresh();
            }
            else
            {
                MessageBox.Show("Строка с указанным идентификатором не найдена.");
            }
        }

        int currentId = 0;
        private void button4_Click(object sender, EventArgs e)
        {
            UpdateRowById((int)currentId, textBox3.Text);
            UpdateBasePrice(Convert.ToDecimal(textBox4.Text));
        }
        static void UpdateBasePrice(decimal newBasePrice)
        {
            using (SqlConnection connection = new SqlConnection(Program.connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("UpdateBasePrice", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Добавление параметра для передачи новой базовой цены
                    cmd.Parameters.Add(new SqlParameter("@newBasePrice", SqlDbType.Decimal)
                    {
                        Value = newBasePrice,
                        Precision = 18, // Уточнение точности для decimal
                        Scale = 2 // Уточнение масштаба для decimal
                    });

                    try
                    {
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Base price updated successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating base price: {ex.Message}");
                        // Обработка ошибок по вашему усмотрению
                    }
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                object cellValue = dataGridView1.SelectedRows[0].Cells[0].Value;

                if (cellValue != DBNull.Value)
                {
                    Int32.TryParse(cellValue.ToString(), out currentId);
                    textBox1.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
                    textBox2.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
                    textBox3.Text = dataGridView1.SelectedRows[0].Cells[5].Value.ToString();
                    comboBox2.SelectedIndex = comboBox2.FindStringExact(dataGridView1.SelectedRows[0].Cells["Session_id"].Value?.ToString());
                    textBox4.Text = GetTicketPrice(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Session_id"].Value?.ToString())).ToString();
                }
                else
                {
                    // Обработка ситуации, когда значение в ячейке равно DBNull.Value
                    // Возможно, вы захотите очистить текстовые поля или выполнить другие действия
                    // в зависимости от логики вашего приложения.
                }
            }
        }

        private void button4_MouseEnter(object sender, EventArgs e)
        {
            button4.ForeColor = Color.White;
        }

        private void button4_MouseLeave(object sender, EventArgs e)
        {
            button4.ForeColor = Color.Gray;

        }

    }
}
