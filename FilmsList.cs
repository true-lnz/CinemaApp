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
    public partial class FilmsList : Form
    {
        List<FilmData> films = new List<FilmData>();
        public FilmsList(List<FilmData> films)
        {
            InitializeComponent();
            this.films = films; 
        }

        private void DisplayFilmsInTable(List<FilmData> filmsList)
        {
            tableLayoutPanel1.Controls.Clear();
            for (int i = 0; i < filmsList.Count; i++)
            {
                Panel filmPanel = CreateFilmPanel(i, filmsList[i]); 
                tableLayoutPanel1.Controls.Add(filmPanel);
            }
        }

        private Panel CreateFilmPanel(int filmIndex, FilmData film)
        {
            // Создаем новую панель
            Panel panel = new Panel();

            // Настраиваем внешний вид панели
            panel.BackColor = Color.WhiteSmoke;
            panel.Size = new Size(355, 150);

            panel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;


            // Добавляем нумерацию слева посередине
            Label indexLabel = new Label
            {
                Text = (filmIndex + 1).ToString(), // filmIndex начинается с 0, поэтому добавляем 1
                Location = new Point(5, (int)(panel.Height / 2.3)),
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold) // Устанавливаем жирный шрифт
            };
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            panel.Controls.Add(indexLabel);

            if (film.ImageBytes != null && film.ImageBytes.Length > 0)
            {
                PictureBox pictureBox = new PictureBox
                {
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Size = new Size(100, 133),
                    Location = new Point(40, 10),
                    Image = Image.FromStream(new MemoryStream(film.ImageBytes))
                };
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                panel.Controls.Add(pictureBox);
            }

            // Добавляем информацию о фильме
            Label titleLabel = new Label
            {
                Text = film.Title,
                Location = new Point(150, 10),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Bold)
            };
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            panel.Controls.Add(titleLabel);

            Label ratingLabel = new Label
            {
                Text = $"Рейтинг: {film.Rating}",
                Location = new Point(150, 40),
                AutoSize = true
            };
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            panel.Controls.Add(ratingLabel);

            Label countryLabel = new Label
            {
                Text = $"Страна: {film.Country}",
                Location = new Point(150, 60),
                AutoSize = true
            };
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            panel.Controls.Add(countryLabel);

            Label licenseLabel = new Label
            {
                Text = $"Прокат в кинотеатрах до {film.LicenceExp.ToString("d")}",
                Location = new Point(150, 80),
                AutoSize = true

            };
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            if (film.Title != "Ничего не найдено")
                panel.Controls.Add(licenseLabel);

            LinkLabel clickLabel = new LinkLabel
            {
                Text = $"Подробнее о фильме",
                LinkColor = SystemColors.HotTrack,
                ActiveLinkColor = SystemColors.Highlight,
                Location = new Point(150, 120),
                LinkBehavior = LinkBehavior.NeverUnderline,
                AutoSize = true,
            };
            clickLabel.Click += (sender, e) => FilmPanel_Click(film);
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            if (film.Title != "Ничего не найдено")
                panel.Controls.Add(clickLabel);

            return panel;
        }


        private void FilmPanel_Click(FilmData film)
        {
            FilmDescription filmDescription = new FilmDescription(film);
            filmDescription.Show();
        }

        private void FilmsList_Load(object sender, EventArgs e)
        {
            if (films.Count <= 0)
                films.Add(new FilmData() { Title = "Ничего не найдено", Country = "Пусто", Rating = "Пусто" });
            DisplayFilmsInTable(films);
        }
    }
}
