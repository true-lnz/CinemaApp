using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CINEMA_APP.CinemaMainForm;

namespace CINEMA_APP
{
    public partial class FilmDescription : Form
    {
        public FilmDescription(FilmData filmData)
        {
            InitializeComponent();
            labelFilmTitle.Text = filmData.Title;
            genreLabel.Text = "Жанр: " + filmData.Genre;
            descriptionLabel.Text = "Описание:\n\n" + filmData.Description;
            durationLabel.Text = "Длительность: " + filmData.Duration.ToString()+" минут";
            ratingLabel.Text = "Рейтинг: "+ filmData.Rating.ToString();
            countryLabel.Text = "Страна производства: " + filmData.Country;
            ageLabel.Text = filmData.AgeLimit.ToString()+"+";
            dateLabel.Text = "Дата выхода: " + filmData.Date_of_view.ToString("D");

            byte[] imagesBytes = filmData.ImageBytes;
            if (imagesBytes == null || imagesBytes.Length == 0)
            {
                return;
            }

            using (MemoryStream ms = new MemoryStream(imagesBytes))
            {
                System.Drawing.Image loadedImage = System.Drawing.Image.FromStream(ms);
                filmPictureBox.Image = loadedImage;
                filmPictureBox.SizeMode = PictureBoxSizeMode.Zoom; // Масштабирование изображения
            }

        }

        private void FilmDescription_Load(object sender, EventArgs e)
        {

        }
        public event EventHandler SomeEvent;
        private void button2_Click(object sender, EventArgs e)
        {
            SomeEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
