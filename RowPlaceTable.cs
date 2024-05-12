using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CINEMA_APP
{
    public partial class RowPlaceTable : Form
    {
        public RowPlaceTable()
        {
            InitializeComponent();
        }
        SqlDataAdapter dataAdapter = new SqlDataAdapter();
        DataTable dataTable = new DataTable();

        bool dataLoad = false;
        public void LoadTable()
        {
            bindingSource1.DataSource = null;
            dataTable.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            dataLoad = true;
            string query = "SELECT * FROM RowPlace";

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

                    bindingSource1.DataSource = dataTable;
                    dataGridView1.DataSource = bindingSource1;
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Произошла ошибка при получении данных: " + ex.Message);
                }
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[1].HeaderText = "Номер ряда";
                dataGridView1.Columns[2].HeaderText = "Количество мест в ряде";
                dataGridView1.Columns[3].Visible = false;
                DataGridViewTextBoxColumn comboBoxColumn = new DataGridViewTextBoxColumn();
                comboBoxColumn.Name = "Hall";
                comboBoxColumn.HeaderText = "Зал";
                dataGridView1.Columns.Insert(4, comboBoxColumn);
            }
            dataLoad = false;
            LoadComboBoxes(false);
        }
        private void LoadComboBoxes(bool savePos)
        {
            int pos = 0;
            if (savePos)
            {
                pos = comboBox1.SelectedIndex;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(Program.connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SELECT Hall_id, Name FROM Hall", connection))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable table = new DataTable();
                            adapter.Fill(table);

                            comboBox1.DataSource = table;
                            comboBox1.DisplayMember = "Name";
                            comboBox1.ValueMember = "Hall_id";

                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                string hallIdString = row.Cells[3].Value?.ToString();

                                if (int.TryParse(hallIdString, out int hallId))
                                {
                                    DataRow[] result = table.Select($"Hall_id = {hallId}");

                                    if (result.Length > 0)
                                    {
                                        row.Cells[4].Value = result[0]["Name"].ToString();
                                    }
                                }
                            }
                        }
                    }
                }
                comboBox1.SelectedIndex = pos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при загрузке залов: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RowPlaceTable_Load(object sender, EventArgs e)
        {
            LoadTable();
            dataGridView1.ClearSelection();
            dataGridView1.Rows[0].Selected = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DataRow newRow = dataTable.NewRow();

            // Задать значения для новой строки
            newRow[1] = "0";
            newRow[2] = "0";

            // Добавить новую строку в DataTable
            dataTable.Rows.Add(newRow);

            button5_Click(null, new EventArgs());

            bindingSource1.MoveLast();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bindingSource1.RemoveCurrent();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                bindingSource1.EndEdit();
                dataAdapter.Update((DataTable)bindingSource1.DataSource);
                isSaveNeeded = false;
                LoadTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении изменений: " + ex.Message);
            }
        }

        bool isSaveNeeded = false;
        private void RowPlaceTable_FormClosing(object sender, FormClosingEventArgs e)
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

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            button2.ForeColor = Color.White;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.ForeColor = Color.Gray;
        }

        private void button5_MouseEnter(object sender, EventArgs e)
        {
            button5.ForeColor = Color.White;

        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.ForeColor = Color.White;

        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.ForeColor = Color.Gray;

        }

        private void button5_MouseLeave(object sender, EventArgs e)
        {
            button5.ForeColor = Color.Gray;

        }
        private void UpdateRowById(int id, string a, string b, string c)
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
                rowToUpdate.Cells[1].Value = a;
                rowToUpdate.Cells[2].Value = b;
                rowToUpdate.Cells[3].Value = c;

                // Опционально: обновить отображение данных в DataGridView
                dataGridView1.Refresh();
                LoadComboBoxes(true);
            }
            else
            {
                MessageBox.Show("Строка с указанным идентификатором не найдена.");
            }
        }

        int currentId = 0;
        private void button2_Click(object sender, EventArgs e)
        {
            UpdateRowById((int)currentId, textBox1.Text, textBox2.Text, comboBox1.SelectedValue.ToString());

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataLoad) return;
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                object cellValue = selectedRow.Cells[0].Value;

                if (cellValue != DBNull.Value)
                {
                    Int32.TryParse(cellValue.ToString(), out currentId);
                    textBox1.Text = selectedRow.Cells[1].Value.ToString();
                    textBox2.Text = selectedRow.Cells[2].Value.ToString();
                    comboBox1.SelectedIndex = comboBox1.FindStringExact(selectedRow.Cells["Hall"].Value?.ToString());
                }
                else
                {
                    currentId = 0;
                    textBox1.Text = "";
                    comboBox1.SelectedIndex = -1;
                }
            }
        }
    }
}
