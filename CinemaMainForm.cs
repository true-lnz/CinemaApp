using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using static CINEMA_APP.CinemaMainForm;
using static CINEMA_APP.BookingSeats;
using System.Media;

namespace CINEMA_APP
{
    public partial class CinemaMainForm : Form
    {
        public CinemaMainForm()
        {
            InitializeComponent();
        }
        private List<FilmData> filmDataList = new List<FilmData>(); // Список для хранения данных о фильмах
        private int currentImageIndex = 0; // Индекс текущей отображаемой картинки
        private PictureBox[] pictureBoxes; // Массив PictureBox

        private void forward_Click(object sender, EventArgs e)
        {
            currentImageIndex = (currentImageIndex - 1 + filmDataList.Count) % filmDataList.Count;
            LoadImages();
        }

        private void backward_Click(object sender, EventArgs e)
        {
            currentImageIndex = (currentImageIndex + 1) % filmDataList.Count;
            LoadImages();
        }

        int filmId_To_Session = 0;
        private void ShowFilmData(FilmData filmData)
        {
            if (filmData != null)
            {
                filmId_To_Session = filmData.FilmId;
                labelFilmTitle.Text = filmData.Title;
                labelRating.Text = $"Рейтинг: {filmData.Rating}";
                genreLabel.Text = $"Жанр: {filmData.Genre}";
            }
        }

        private void LoadFilmData()
        {
            using (SqlConnection connection = new SqlConnection(Program.connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM GetMoviesInCurrentSessions();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            FilmData filmData = new FilmData
                            {
                                FilmId = reader.GetInt32(reader.GetOrdinal("Film_id")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                Rating = reader.IsDBNull(reader.GetOrdinal("Rating")) ? null : reader.GetDecimal(reader.GetOrdinal("Rating")).ToString(),
                                Genre = reader.IsDBNull(reader.GetOrdinal("Genre")) ? null : reader.GetString(reader.GetOrdinal("Genre")),
                                ImageBytes = reader["Film_image"] == DBNull.Value ? null : (byte[])reader["Film_image"],
                                AgeLimit = reader.IsDBNull(reader.GetOrdinal("AgeRestrictions")) ? 0 : reader.GetByte(reader.GetOrdinal("AgeRestrictions")),
                                Country = reader.IsDBNull(reader.GetOrdinal("Country")) ? null : reader.GetString(reader.GetOrdinal("Country")),
                                Duration = (short)(reader.IsDBNull(reader.GetOrdinal("Duration")) ? 0 : reader.GetInt16(reader.GetOrdinal("Duration"))),
                                Date_of_view = reader.IsDBNull(reader.GetOrdinal("PremierDate")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("PremierDate")),
                                LicenceBegin = reader.IsDBNull(reader.GetOrdinal("LicenseBegin")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("LicenseBegin")),
                                LicenceExp = reader.IsDBNull(reader.GetOrdinal("LicenseExp")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("LicenseExp"))
                            };
                            filmDataList.Add(filmData);
                        }
                    }
                }
            }
        }

        FilmData currentFilm = null;
        private bool LoadImages()
        {
            for (int i = 0; i < pictureBoxes.Length; i++)
            {
                if (filmDataList.Count == 0) return false;
                int filmIndex = (currentImageIndex + i) % filmDataList.Count; // Циклическое обращение к фильмам
                ShowImageInPictureBox(filmDataList[filmIndex].ImageBytes, pictureBoxes[i]);

                if (i == 2) // Если это центральное изображение, обновляем информацию о фильме
                {
                    ShowFilmData(filmDataList[filmIndex]);
                    currentFilm = filmDataList[filmIndex];
                }
            }
            return true;
        }

        private void ShowImageInPictureBox(byte[] imageBytes, PictureBox pictureBox)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                return;
            }

            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                System.Drawing.Image loadedImage = System.Drawing.Image.FromStream(ms);

                pictureBox.Image = loadedImage;
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom; // Масштабирование изображения
            }
        }

        public class FilmData
        {
            public int FilmId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Genre { get; set; }
            public byte[] ImageBytes { get; set; }
            public string Rating { get; set; }
            public short Duration { get; set; }
            public int AgeLimit { get; set; }
            public DateTime Date_of_view { get; set; }
            public DateTime LicenceBegin { get; set; }
            public DateTime LicenceExp { get; set; }
            public string Country { get; set; }

        }

        private void CinemaMainForm_Load(object sender, EventArgs e)
        {
            pictureBoxes = new PictureBox[] { pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5 };
            LoadFilmData(); // Загружаем данные о фильмах
            filmDataList = filmDataList.OrderByDescending(film => Convert.ToDouble(film.Rating)).ToList();

            if (!LoadImages()) // Загружаем изображения из базы данных в PictureBox
            {
                // Фильмов в прокате нет
                MessageBox.Show("К сожалению фильмов в прокате не найдено.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                button2.Enabled = false; // Нельзя купить билет
                button2.Text = "Недоступно";
                panel4.Visible = true;
                Label nothing = new Label();
                nothing.Text = "К сожалению фильмов в прокате не найдено";
                nothing.Font = new Font("Arial", 16);
                nothing.Dock = DockStyle.Fill;
                nothing.TextAlign = ContentAlignment.MiddleCenter;
                panel4.Controls.Add(nothing); 

            }
        }

        private void BookingSeats_newHandler()
        {
            panel3.Visible = true;
            bookingSeats.Hide();
        }

        BookingSeats bookingSeats = null;
        private void button2_Click(object sender, EventArgs e)
        {
            bookingSeats = new BookingSeats(filmId_To_Session);
            bookingSeats.newHandler += BookingSeats_newHandler;

            this.panel3.Visible = false;
            bookingSeats.TopLevel = false;
            bookingSeats.Dock = DockStyle.Fill;
            //panel3.Controls.Clear();
            this.Controls.Add(bookingSeats);
            this.Dock = DockStyle.Fill;
            bookingSeats.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Auth auth = new Auth(this);
            auth.Show();
            this.Hide();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FilmDescription filmDescription = new FilmDescription(currentFilm);
            filmDescription.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SearchDialog searchDialog = new SearchDialog();
            if (searchDialog.ShowDialog() == DialogResult.OK)
            {
                return;
            }
            else
            {
                SystemSounds.Exclamation.Play();
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FilmsList filmsList = new FilmsList(filmDataList);
            filmsList.Text = "Рейтинг актуальных фильмов";
            filmsList.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string searchTerm = textBox1.Text;
            FilmData foundFilm = null;

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                SystemSounds.Exclamation.Play();
                return;
            }

            foreach (FilmData film in filmDataList)
            {
                string temp = film.Title;
                if (temp.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    foundFilm = film;
                    break; // Выход из цикла, так как фильм найден
                }
            }

            if (foundFilm != null)
            {
                linkLabel3.Visible = true;
                button3.Visible = false;
                ShowFilmData(foundFilm);
                currentFilm = foundFilm;
                ShowImageInPictureBox(filmDataList[filmDataList.IndexOf(foundFilm)].ImageBytes, pictureBoxes[2]);
                panel4.Visible = true;
                FilmDescription film = new FilmDescription(foundFilm);
                film.SomeEvent += Film_SomeEvent; // Подпишитесь на событие (если у FilmDescription есть такое событие)
                film.TopLevel = false;
                film.FormBorderStyle = FormBorderStyle.None;
                film.button2.Visible = true;
                panel4.Controls.Add(film);
                film.Show();
            }
            else
            {
                // Фильм не найден, выведите сообщение об этом
                MessageBox.Show("Фильм не найден.", "Поиск", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


        }

        private void Film_SomeEvent(object sender, EventArgs e)
        {
            cancelSearch();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                // Вызываем метод, который выполняет действие при нажатии Enter (например, поиск)
                button3_Click(null, new EventArgs());

                // Предотвращаем дальнейшую обработку клавиши Enter
                e.Handled = true;
            }
        }
        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            cancelSearch();
        }
        private void cancelSearch()
        {
            panel4.Visible = false;
            panel4.Controls.Clear();
            button3.Visible = true;
            linkLabel3.Visible = false;
            textBox1.Text = "";
        }

        private void CinemaMainForm_Resize(object sender, EventArgs e)
        {
/*            if (WindowState == FormWindowState.Maximized)
            {
                bookingSeats.listBox1_SelectedIndexChanged(null, new EventArgs());
            }
            else if (WindowState == FormWindowState.Normal)
            {
                bookingSeats.listBox1_SelectedIndexChanged(null, new EventArgs());
            }*/
        }
    }

}
