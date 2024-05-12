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
using static CINEMA_APP.BookingSeats;
using static CINEMA_APP.CinemaMainForm;

namespace CINEMA_APP
{
    public partial class SearchDialog : Form
    {
        public SearchDialog()
        {
            InitializeComponent();
        }

        private void LoadComboBoxes()
        {
            for (int i = 0; i < 46; i++)
            {
                comboBox2.Items.Add((2025 - i).ToString());
            }
            for (int i = 0; i < 46; i++)
            {
                comboBox3.Items.Add((2025 - i).ToString());
            }

            comboBox1.Items.Clear();
            using (SqlConnection connection = new SqlConnection(Program.connectionString))
            {
                connection.Open();

                string query = "EXEC getCountries;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboBox1.Items.Add(reader[0].ToString());
                        }
                    }
                }
            }
            listBox1.Items.Clear();
            using (SqlConnection connection = new SqlConnection(Program.connectionString))
            {
                connection.Open();

                string query = "EXEC getGenres;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listBox1.Items.Add(reader[0].ToString());
                        }
                    }
                }
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<FilmData> searchResults = new List<FilmData>();

            string searchTitle = textBox1.Text;
            int? searchMinYear = null;
            int? searchMaxYear = null;

            if (comboBox2.SelectedIndex != -1 && comboBox3.SelectedIndex != -1)
            {
                Int32.TryParse(comboBox3.SelectedItem.ToString(), out int tempMinYear);
                Int32.TryParse(comboBox2.SelectedItem.ToString(), out int tempMaxYear);
                 searchMinYear = tempMinYear;
                searchMaxYear = tempMaxYear;
            }
            else if (comboBox2.SelectedIndex == -1 && comboBox3.SelectedIndex != -1)
            {
                Int32.TryParse(comboBox3.SelectedItem.ToString(), out int tempMinYear);
                searchMinYear = searchMaxYear = tempMinYear;
            }
            else if (comboBox2.SelectedIndex != -1 && comboBox3.SelectedIndex == -1)
            {
                Int32.TryParse(comboBox2.SelectedItem.ToString(), out int tempMinYear);
                searchMinYear = searchMaxYear = tempMinYear;
            }
            else if (!string.IsNullOrWhiteSpace(textBox2.Text))
            {
                Int32.TryParse(textBox2.Text, out int tempMinYear);
                searchMinYear = searchMaxYear = tempMinYear;
            }
            string searchCountry = comboBox1.SelectedIndex != -1 ? comboBox1.SelectedItem.ToString() : null;
            string searchGenre = "";
            if (listBox1.SelectedItems.Count > 0)
            {
                for (int i = 0; i < listBox1.SelectedItems.Count; i++)
                    searchGenre += $"{listBox1.SelectedItems[i]} ";
            }
            else
                searchGenre = null;

            string sqlQuery = "SELECT * FROM dbo.SearchFilms(@SearchTitle, @SearchGenre, @SearchCountry, @SearchMinYear, @SearchMaxYear);";

            using (SqlConnection connection = new SqlConnection(Program.connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    // Добавляем параметры к команде
                    command.Parameters.AddWithValue("@SearchTitle", searchTitle);
                    command.Parameters.AddWithValue("@SearchMinYear", searchMinYear ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@SearchMaxYear", searchMaxYear ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@SearchCountry", searchCountry ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@SearchGenre", searchGenre ?? (object)DBNull.Value);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            FilmData film = new FilmData
                            {
                                FilmId = Convert.ToInt32(reader["Film_id"]),
                                Title = reader["Title"].ToString(),
                                Description = reader["Description"].ToString(),
                                Rating = reader["Rating"].ToString(),
                                Genre = reader["Genre"].ToString(),
                                Duration = Convert.ToInt16(reader["Duration"]),
                                Country = reader["Country"].ToString(),
                                Date_of_view = Convert.ToDateTime(reader["PremierDate"]),
                                AgeLimit = Convert.ToByte(reader["AgeRestrictions"]),
                                ImageBytes = (byte[])reader["Film_image"],
                                LicenceBegin = Convert.ToDateTime(reader["LicenseBegin"]),
                                LicenceExp = Convert.ToDateTime(reader["LicenseExp"]),
                            };

                            searchResults.Add(film);
                        }
                    }
                }
            }

            FilmsList filmsList = new FilmsList(searchResults);
            filmsList.Show();
            DialogResult = DialogResult.OK;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex != -1 && comboBox2.SelectedIndex != -1)
            {
                if (Convert.ToInt32(comboBox3.SelectedItem.ToString()) > Convert.ToInt32(comboBox2.SelectedItem.ToString()))
                {
                    comboBox2.SelectedIndex = -1;
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex != -1 && comboBox2.SelectedIndex != -1)
            {
                if (Convert.ToInt32(comboBox2.SelectedItem.ToString()) < Convert.ToInt32(comboBox3.SelectedItem.ToString()))
                {
                    comboBox2.SelectedIndex = -1;
                }
            }

        }

        private void SearchDialog_Load(object sender, EventArgs e)
        {
            LoadComboBoxes();
        }
    }
}
