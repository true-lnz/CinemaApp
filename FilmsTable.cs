using Google.Protobuf.WellKnownTypes;
using PdfSharp.Charting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CINEMA_APP
{
    public partial class FilmsTable : Form
    {
        public FilmsTable()
        {
            InitializeComponent();
        }

        SqlDataAdapter dataAdapter = new SqlDataAdapter();
        DataTable dataTable = new DataTable();

        private void FilmsTable_Load(object sender, EventArgs e)
        {
            FillDataGridViewWithFilms();
            dataGridView1.ClearSelection();
            dataGridView1.Rows[0].Selected = true;
        }
        bool dataLoad = true;
        public void FillDataGridViewWithFilms()
        {
            dataLoad = true;
            string query = "SELECT * FROM Film"; // Пример запроса на выборку всех фильмов

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

                    comboBox1.DataSource = dataTable;
                    comboBox1.DisplayMember = "Title";
                    comboBox1.ValueMember = "Film_id";

                    bindingSource1.DataSource = dataTable;
                    dataGridView1.DataSource = bindingSource1;

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        string filmIdString = row.Cells[3].Value?.ToString();

                        if (int.TryParse(filmIdString, out int hallId))
                        {
                            DataRow[] result = dataTable.Select($"Film_id = {hallId}");

                            if (result.Length > 0)
                            {
                                row.Cells[5].Value = result[0]["Title"].ToString();
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Произошла ошибка при получении данных: " + ex.Message);
                }
            }
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[9].Visible = false;
            dataGridView1.Columns[1].HeaderText = "Название";
            dataGridView1.Columns[2].HeaderText = "Описание";
            dataGridView1.Columns[3].HeaderText = "Рейтинг";
            dataGridView1.Columns[4].HeaderText = "Жанр";
            dataGridView1.Columns[5].HeaderText = "Длительность";
            dataGridView1.Columns[6].HeaderText = "Страна";
            dataGridView1.Columns[7].HeaderText = "Дата премьеры";
            dataGridView1.Columns[8].HeaderText = "Возрастные ограничения";
            dataGridView1.Columns[10].HeaderText = "Начало лицензии";
            dataGridView1.Columns[11].HeaderText = "Истечение лицензии";
            dataLoad = false;
        }

        void ShowImageInPictureBox(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                return;
            }

            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                Image loadedImage = Image.FromStream(ms);
                pictureBox1.Image = loadedImage;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }
        static byte[] ConvertImageToBytes(string imagePath)
        {
            using (Image image = Image.FromFile(imagePath))
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg; *.png; *.bmp)|*.jpg; *.png; *.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                byte[] imageBytes = ConvertImageToBytes(openFileDialog.FileName);
                UpdateRowById(currentId, imageBytes);
                ShowImageInPictureBox(imageBytes);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bindingSource1.RemoveCurrent();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DataRow newRow = dataTable.NewRow();

            // Задать значения для новой строки
            newRow[1] = "Название";
            newRow[2] = "Описание";
            newRow[3] = 1;
            newRow[4] = "Жанр";
            newRow[5] = 0;
            newRow[6] = "Без страны";
            newRow[7] = DateTime.Now;
            newRow[8] = 0;
            newRow[10] = DateTime.Now;
            newRow[11] = DateTime.Now;

            // Добавить новую строку в DataTable
            dataTable.Rows.Add(newRow);

            button5_Click(null, new EventArgs());

            bindingSource1.MoveLast();

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

        int currentId = 0;
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
                    comboBox1.SelectedIndex = comboBox1.FindStringExact(selectedRow.Cells[1].Value?.ToString());

                    int rowIndex = dataGridView1.SelectedCells[0].RowIndex;
                    var value = dataGridView1.Rows[rowIndex].Cells[9].Value;
                    byte[] bytes = (byte[])(value != DBNull.Value ? value : null);
                    if (bytes != null)
                        ShowImageInPictureBox(bytes);
                    else
                        pictureBox1.Image = null;
                }
                else
                {
                    currentId = 0;
                    comboBox1.SelectedIndex = -1;
                }
            }
        }

        bool isSaveNeeded = false;
        private void FilmsTable_FormClosing(object sender, FormClosingEventArgs e)
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
        private void UpdateRowById(int id, byte[] d)
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
                rowToUpdate.Cells[9].Value = d;

                // Опционально: обновить отображение данных в DataGridView
                dataGridView1.Refresh();
            }
            else
            {
                MessageBox.Show("Ошибка при обновлении афиши.");
            }
        }
        private void button5_MouseEnter(object sender, EventArgs e)
        {
            button5.ForeColor = Color.White;
        }

        private void button4_MouseEnter(object sender, EventArgs e)
        {
            button4.ForeColor = Color.White;

        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.ForeColor = Color.White;

        }

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            button2.ForeColor = Color.White;

        }

        private void button3_MouseEnter(object sender, EventArgs e)
        {
            button3.ForeColor = Color.White;

        }

        private void button5_MouseLeave(object sender, EventArgs e)
        {
            button5.ForeColor = Color.Gray;

        }

        private void button4_MouseLeave(object sender, EventArgs e)
        {
            button4.ForeColor = Color.Gray;

        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.ForeColor = Color.Gray;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ViewReports view = new ViewReports(3);
            view.Text = "Топ фильмов по выручке";
            view.Show();
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.ForeColor = Color.Gray;

        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            button3.ForeColor = Color.Gray;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ViewReports view = new ViewReports(2);
            view.Text = "Топ фильмов по рейтингу";
            view.Show();
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int.TryParse(comboBox1.SelectedValue.ToString(), out int currentId);

        }
    }
}
