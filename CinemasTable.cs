using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CINEMA_APP
{
    public partial class CinemasTable : Form
    {
        public CinemasTable()
        {
            InitializeComponent();
        }

        SqlDataAdapter dataAdapter = new SqlDataAdapter();
        private void CinemasTable_Load(object sender, EventArgs e)
        {
            LoadTable();
        }
        public void LoadTable()
        {
            string query = "SELECT * FROM Cinema"; // Пример запроса на выборку всех фильмов

            using (SqlConnection connection = new SqlConnection(Program.connectionString))
            {
                connection.Open();

                dataAdapter.SelectCommand = new SqlCommand(query, new SqlConnection(Program.connectionString));

                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);

                dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                dataAdapter.InsertCommand = commandBuilder.GetInsertCommand();
                dataAdapter.DeleteCommand = commandBuilder.GetDeleteCommand();

                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                bindingSource1.DataSource = dataTable;
                dataGridView1.DataSource = bindingSource1;
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[1].HeaderText = "Название кинотеатра";
                dataGridView1.Columns[2].HeaderText = "Адрес";
                dataGridView1.Columns[3].HeaderText = "Номер телефона";

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bindingSource1.RemoveCurrent();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            bindingSource1.AddNew();

            DataRowView newRowView = (DataRowView)bindingSource1.Current;

            newRowView[1] = textBox1.Text; 
            newRowView[2] = textBox2.Text; 
            newRowView[3] = maskedTextBox1.Text.Replace(" ", "");

            bindingSource1.EndEdit();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                bindingSource1.EndEdit();
                dataAdapter.Update((DataTable)bindingSource1.DataSource);
                isSaveNeeded = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении изменений: " + ex.Message);
            }
        }

        bool isSaveNeeded = false;
        private void CinemasTable_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isSaveNeeded == true)
            {
                if (MessageBox.Show("Сохранить изменения?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    e.Cancel = true;
                    button5_Click(null, new EventArgs());
                    this.Close();
                }
            }
        }
        int currentId = 0;
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                object cellValue = dataGridView1.SelectedRows[0].Cells[0].Value;

                if (cellValue != DBNull.Value)
                {
                    Int32.TryParse(cellValue.ToString(), out currentId);
                    textBox1.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                    textBox2.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
                    maskedTextBox1.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
                }
                else
                {
                    // Обработка ситуации, когда значение в ячейке равно DBNull.Value
                    // Возможно, вы захотите очистить текстовые поля или выполнить другие действия
                    // в зависимости от логики вашего приложения.
                }
            }
        }


        private void UpdateRowById(int id, string name, string address, string phone)
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
                rowToUpdate.Cells[1].Value = name;
                rowToUpdate.Cells[2].Value = address;
                rowToUpdate.Cells[3].Value = phone;

                // Опционально: обновить отображение данных в DataGridView
                dataGridView1.Refresh();
            }
            else
            {
                MessageBox.Show("Строка с указанным идентификатором не найдена.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateRowById((int)currentId, textBox1.Text, textBox2.Text, maskedTextBox1.Text.Replace(" ", ""));
        }

        private void button5_MouseEnter(object sender, EventArgs e)
        {
            button5.ForeColor = Color.White;
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button5.ForeColor = Color.White;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button5.ForeColor = Color.Gray;

        }
    }
}