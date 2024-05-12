namespace CINEMA_APP
{
    partial class FilmDescription
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ratingLabel = new System.Windows.Forms.Label();
            this.labelFilmTitle = new System.Windows.Forms.Label();
            this.filmPictureBox = new System.Windows.Forms.PictureBox();
            this.ageLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.durationLabel = new System.Windows.Forms.Label();
            this.countryLabel = new System.Windows.Forms.Label();
            this.genreLabel = new System.Windows.Forms.Label();
            this.dateLabel = new System.Windows.Forms.Label();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.filmPictureBox)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ratingLabel
            // 
            this.ratingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ratingLabel.Location = new System.Drawing.Point(9, 213);
            this.ratingLabel.Margin = new System.Windows.Forms.Padding(0);
            this.ratingLabel.Name = "ratingLabel";
            this.ratingLabel.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.ratingLabel.Size = new System.Drawing.Size(105, 26);
            this.ratingLabel.TabIndex = 14;
            this.ratingLabel.Text = "Рейтинг:";
            this.ratingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelFilmTitle
            // 
            this.labelFilmTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelFilmTitle.Location = new System.Drawing.Point(165, 12);
            this.labelFilmTitle.Margin = new System.Windows.Forms.Padding(0);
            this.labelFilmTitle.Name = "labelFilmTitle";
            this.labelFilmTitle.Size = new System.Drawing.Size(410, 41);
            this.labelFilmTitle.TabIndex = 11;
            this.labelFilmTitle.Text = "Название";
            this.labelFilmTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // filmPictureBox
            // 
            this.filmPictureBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.filmPictureBox.Location = new System.Drawing.Point(12, 12);
            this.filmPictureBox.Name = "filmPictureBox";
            this.filmPictureBox.Size = new System.Drawing.Size(150, 200);
            this.filmPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.filmPictureBox.TabIndex = 5;
            this.filmPictureBox.TabStop = false;
            // 
            // ageLabel
            // 
            this.ageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ageLabel.Location = new System.Drawing.Point(114, 213);
            this.ageLabel.Margin = new System.Windows.Forms.Padding(0);
            this.ageLabel.Name = "ageLabel";
            this.ageLabel.Size = new System.Drawing.Size(48, 26);
            this.ageLabel.TabIndex = 20;
            this.ageLabel.Text = "+";
            this.ageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.durationLabel);
            this.panel1.Controls.Add(this.countryLabel);
            this.panel1.Controls.Add(this.genreLabel);
            this.panel1.Controls.Add(this.dateLabel);
            this.panel1.Controls.Add(this.descriptionLabel);
            this.panel1.Location = new System.Drawing.Point(168, 47);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(404, 222);
            this.panel1.TabIndex = 21;
            // 
            // durationLabel
            // 
            this.durationLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.durationLabel.Location = new System.Drawing.Point(0, 170);
            this.durationLabel.Margin = new System.Windows.Forms.Padding(0);
            this.durationLabel.Name = "durationLabel";
            this.durationLabel.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.durationLabel.Size = new System.Drawing.Size(404, 20);
            this.durationLabel.TabIndex = 24;
            this.durationLabel.Text = "Длительность:";
            this.durationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // countryLabel
            // 
            this.countryLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.countryLabel.Location = new System.Drawing.Point(0, 150);
            this.countryLabel.Margin = new System.Windows.Forms.Padding(0);
            this.countryLabel.Name = "countryLabel";
            this.countryLabel.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.countryLabel.Size = new System.Drawing.Size(404, 20);
            this.countryLabel.TabIndex = 23;
            this.countryLabel.Text = "Страна:";
            this.countryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // genreLabel
            // 
            this.genreLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.genreLabel.Location = new System.Drawing.Point(0, 130);
            this.genreLabel.Margin = new System.Windows.Forms.Padding(0);
            this.genreLabel.Name = "genreLabel";
            this.genreLabel.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.genreLabel.Size = new System.Drawing.Size(404, 20);
            this.genreLabel.TabIndex = 22;
            this.genreLabel.Text = "Жанр:";
            this.genreLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dateLabel
            // 
            this.dateLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.dateLabel.Location = new System.Drawing.Point(0, 110);
            this.dateLabel.Margin = new System.Windows.Forms.Padding(0);
            this.dateLabel.Name = "dateLabel";
            this.dateLabel.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.dateLabel.Size = new System.Drawing.Size(404, 20);
            this.dateLabel.TabIndex = 21;
            this.dateLabel.Text = "Дата выхода:";
            this.dateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.descriptionLabel.Location = new System.Drawing.Point(0, 0);
            this.descriptionLabel.Margin = new System.Windows.Forms.Padding(0);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Padding = new System.Windows.Forms.Padding(10, 0, 0, 30);
            this.descriptionLabel.Size = new System.Drawing.Size(404, 110);
            this.descriptionLabel.TabIndex = 20;
            this.descriptionLabel.Text = "Описание:";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.Highlight;
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(12, 247);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(150, 22);
            this.button2.TabIndex = 22;
            this.button2.Text = "Назад";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // FilmDescription
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(584, 281);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ageLabel);
            this.Controls.Add(this.ratingLabel);
            this.Controls.Add(this.filmPictureBox);
            this.Controls.Add(this.labelFilmTitle);
            this.Icon = global::CINEMA_APP.Properties.Resources.kino;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(600, 350);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 300);
            this.Name = "FilmDescription";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "О фильме";
            this.Load += new System.EventHandler(this.FilmDescription_Load);
            ((System.ComponentModel.ISupportInitialize)(this.filmPictureBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox filmPictureBox;
        private System.Windows.Forms.Label ratingLabel;
        private System.Windows.Forms.Label labelFilmTitle;
        private System.Windows.Forms.Label ageLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label durationLabel;
        private System.Windows.Forms.Label countryLabel;
        private System.Windows.Forms.Label genreLabel;
        private System.Windows.Forms.Label dateLabel;
        private System.Windows.Forms.Label descriptionLabel;
        public System.Windows.Forms.Button button2;
    }
}