using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using System.Drawing.Imaging;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace CINEMA_APP
{
    public partial class TicketConfirmationPrint : Form
    {
        string[] ticketDetails;
        public TicketConfirmationPrint(string[] temp)
        {
            InitializeComponent();
            this.ticketDetails = temp;
        }

        private void TicketConfirmationPrint_Load(object sender, EventArgs e)
        {
            string[] nums = ticketDetails[6].Split(',');

            for (int i = 0; i < nums.Length; i++)
            {
                StringBuilder ticketInfo = new StringBuilder();
                ticketInfo.AppendLine($"---------------------------------------------------------------------------------------------");
                ticketInfo.AppendLine(nums[i]);
                ticketInfo.AppendLine();
                ticketInfo.AppendLine($"Фильм: {ticketDetails[1]} {ticketDetails[7]} {ticketDetails[8]}+");
                ticketInfo.AppendLine($"Кинотеатр: {ticketDetails[2]}, зал: {ticketDetails[3]}");
                ticketInfo.AppendLine($"Адрес: {ticketDetails[4]}");
                ticketInfo.AppendLine($"Время: {ticketDetails[5]}");
                ticketInfo.AppendLine($"Цена: {ticketDetails[9]}" + "₽");
                ticketInfo.AppendLine($"---------------------------------------------------------------------------------------------");
                textBox1.Text += ticketInfo.ToString();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDocument1;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
            }

        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            using (Font font = new Font("Arial", 14))
            {
                e.Graphics.DrawString(textBox1.Text, font, Brushes.Black, new PointF(100, 100));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Генерируем уникальное имя файла с использованием текущей даты и времени
            string fileName = $"Билет_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            // Создаем новый PDF-документ
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();

            // Устанавливаем размеры и поля страницы
            page.Size = PdfSharp.PageSize.A4;
            page.Orientation = PdfSharp.PageOrientation.Portrait;

            // Создаем кисть для рисования
            XSolidBrush brush = new XSolidBrush(XColor.FromArgb(0, 0, 0));
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Arial", 12, XFontStyle.Bold);

            // Рисуем текст на странице
            string[] nums = ticketDetails[6].Split(',');

            int ticketsPerPage = 4;
            int ticketsDrawn = 0;

            for (int i = 0; i < nums.Length; i++)
            {
                if (ticketsDrawn % ticketsPerPage == 0 && ticketsDrawn != 0)
                {
                    // Создаем новую страницу
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                }

                int yOffset = (ticketsDrawn % ticketsPerPage) * 160; // Поднимаем каждую новую секцию на 160 пикселей

                gfx.DrawString("---------------------------------------------------------------------------------------------", font, brush, 100, 80 + yOffset);
                gfx.DrawString(nums[i], font, brush, 100, 100 + yOffset);
                gfx.DrawString($"Фильм: {ticketDetails[1]} {ticketDetails[7]} {ticketDetails[8]}+", font, brush, 100, 140 + yOffset);
                gfx.DrawString($"Кинотеатр: {ticketDetails[2]}, зал: {ticketDetails[3]}", font, brush, 100, 160 + yOffset);
                gfx.DrawString($"Адрес: {ticketDetails[4]}", font, brush, 100, 180 + yOffset);
                gfx.DrawString($"Время: {ticketDetails[5]}", font, brush, 100, 200 + yOffset);
                gfx.DrawString($"Цена: {ticketDetails[9]}" + "₽", font, brush, 100, 220 + yOffset);
                gfx.DrawString("---------------------------------------------------------------------------------------------", font, brush, 100, 240 + yOffset);

                ticketsDrawn++;
            }

            // Сохраняем PDF-документ на диск с уникальным именем
            document.Save(fileName);

            // Открываем PDF-файл в программе по умолчанию
            System.Diagnostics.Process.Start(fileName);
        }
        private void button2_Click(object sender, EventArgs e)
        {

            DialogResult = DialogResult.OK;
        }
    }
}
