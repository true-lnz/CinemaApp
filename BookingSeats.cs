using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CINEMA_APP.BookingSeats;
using static CINEMA_APP.CinemaMainForm;
using System.Runtime.InteropServices;
using System.Transactions;
using System.Media;

namespace CINEMA_APP
{
    public partial class BookingSeats : Form
    {
        int filmId;
        public BookingSeats(int Option)
        {
            InitializeComponent();
            filmId = Option;
        }

        public delegate void BackEventHandler();
        public event BackEventHandler newHandler;
        TicketConfirmationPrint print = new TicketConfirmationPrint(new string[] { });
        private void BookingSeats_Load(object sender, EventArgs e)
        {
            sessionInfoList = GetSessionInfo(filmId);

            if (sessionInfoList.Count > 0)
            {
                label5.Text = "«"+sessionInfoList[0].FilmTitle + "»";
                label9.Text = $"{sessionInfoList[0].SessionTime.ToString("d MMMM", new CultureInfo("ru-RU"))}. Кинотеатр {sessionInfoList[0].CinemaName}";
            }
            else
            {
                label5.Text = "Нет данных";
                label9.Text = "Нет данных";
            }



            listBox1.Items.Clear();

            foreach (SessionInfo sessionInfo in sessionInfoList)
            {
                listBox1.Items.Add($"{sessionInfo.SessionTime}, {sessionInfo.CinemaName}");
                button74.Text = sessionInfo.SessionTime.ToString("HH:mm");
            }

            listBox1.SelectedIndex = 0;

            LoadSeatAvailabilityCache(sessionInfoList[0].SchedId);
            LoadSeats();
        }

        private Dictionary<Tuple<int, int, int>, bool> seatAvailabilityCache = new Dictionary<Tuple<int, int, int>, bool>();

        private void LoadSeatAvailabilityCache(int sessionId)
        {
            seatAvailabilityCache.Clear();
            using (SqlConnection connection = new SqlConnection(Program.connectionString))
            {
                connection.Open();

                string query = "EXEC dbo.GetSeatAvailability @sessionId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@sessionId", sessionId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int Row_num = reader.GetByte(reader.GetOrdinal("Row_number"));
                            int SeatNum = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Seat_num")));
                            int Status = reader.GetInt32(reader.GetOrdinal("IsReserved"));

                            seatAvailabilityCache.Add(Tuple.Create(Row_num, SeatNum, sessionId), Status == 0);
                        }
                    }
                }
            }
        }


        private bool IsSeatAvailable(int rowNumber, int seatNumber, int sessionId)
        {
            Tuple<int, int, int> key = Tuple.Create(rowNumber, seatNumber, sessionId);
            if (seatAvailabilityCache.ContainsKey(key))
            {
                return seatAvailabilityCache[key];
            }

            return true;
        }

        private List<SeatInfo> GetSeatInfo(int schedId)
        {
            List<SeatInfo> seatInfoList = new List<SeatInfo>();

            using (SqlConnection connection = new SqlConnection(Program.connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM GetRowCountInfo(@schedId);";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@schedId", schedId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SeatInfo seatInfo = new SeatInfo
                            {
                                SeatId = reader.GetInt32(reader.GetOrdinal("RowPlace_id")),
                                RowNumber = reader.GetByte(reader.GetOrdinal("Row_number")),
                                SeatsNumber = reader.GetByte(reader.GetOrdinal("Seats_inRow")),
                            };

                            seatInfoList.Add(seatInfo);
                        }
                    }
                }
            }

            return seatInfoList;
        }

        public void LoadSeats()
        {
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.AutoScroll = false;
            tableLayoutPanel1.Visible = false;

            tableLayoutPanel1.SuspendLayout();

            int maxSeats = 0;

            List<SeatInfo> seatInfoList = GetSeatInfo(sessionInfoList[listBox1.SelectedIndex].SchedId);

            // Находим максимальное количество мест в ряду.
            foreach (var seatInfo in seatInfoList)
            {
                if (seatInfo.SeatsNumber > maxSeats)
                    maxSeats = seatInfo.SeatsNumber;
            }

            // Устанавливаем количество столбцов и строк в TableLayoutPanel.
            tableLayoutPanel1.ColumnCount = maxSeats + 2; // Два дополнительных столбца для номеров рядов
            tableLayoutPanel1.RowCount = seatInfoList.Count;

            // Создаем кнопки и добавляем их в TableLayoutPanel.
            foreach (var seatInfo in seatInfoList)
            {
                int rowSeats = seatInfo.SeatsNumber;
                int offset = (maxSeats - rowSeats) / 2; // Определяем отступ для центрирования.

                // Добавляем боковые столбцы с номерами рядов слева.
                Label rowLabelLeft = new Label();
                rowLabelLeft.Text = $"{seatInfo.RowNumber}";
                rowLabelLeft.Dock = DockStyle.Right;
                rowLabelLeft.TextAlign = ContentAlignment.TopRight;
                tableLayoutPanel1.Controls.Add(rowLabelLeft, 0, seatInfo.RowNumber - 1);

                for (int i = 0; i < rowSeats; i++)
                {
                    Button seatButton = new Button();

                    bool isSeatAvailable = IsSeatAvailable(seatInfo.RowNumber, i+1, sessionInfoList[listBox1.SelectedIndex].SchedId);

                    if (!isSeatAvailable)
                    {
                        seatButton.BackColor = System.Drawing.Color.Gray;
                        seatButton.Enabled = false;
                    }
                    else
                    {
                        seatButton.BackColor = System.Drawing.Color.GreenYellow;
                    }

                    seatButton.FlatAppearance.BorderSize = 0;
                    int buttonSize = tableLayoutPanel1.Width / ((maxSeats + 2)+15); // Размер кнопки зависит от ширины tableLayoutPanel1
                    seatButton.Size = new Size(buttonSize, buttonSize);
                    seatButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                    seatButton.ForeColor = System.Drawing.Color.White;
                    seatButton.Click += SeatButton_Click;
                    seatButton.Tag = new Tuple<int, int>(seatInfo.RowNumber, i);

                    tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                    tableLayoutPanel1.Controls.Add(seatButton, i + 1 + offset, seatInfo.RowNumber - 1);
                }

                Label rowLabelRight = new Label();
                rowLabelRight.Dock = DockStyle.Left;
                rowLabelRight.Text = $"{seatInfo.RowNumber}";
                tableLayoutPanel1.Controls.Add(rowLabelRight, maxSeats + 1, seatInfo.RowNumber - 1);
            }

            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            for (int i = 0; i < maxSeats; i++)
            {
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            }
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            tableLayoutPanel1.ResumeLayout();
            tableLayoutPanel1.AutoScroll = true;
            tableLayoutPanel1.Visible = true;
        }

        int ticketId = 0; // Будет хранить идентификатор забронированного билета
        public int ReserveTicket(int filmId, int sessionId, int rowNumber, int seatNumber, string customerName, SqlConnection connection, SqlTransaction transaction, out decimal ticketPrice)
        {
            using (SqlCommand cmd = new SqlCommand("ReserveTicket", connection, transaction))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@filmId", filmId));
                cmd.Parameters.Add(new SqlParameter("@sessionId", sessionId));
                cmd.Parameters.Add(new SqlParameter("@rowNumber", rowNumber));
                cmd.Parameters.Add(new SqlParameter("@seatNumber", seatNumber + 1));
                cmd.Parameters.Add(new SqlParameter("@customerName", customerName));

                // Добавляем выходные параметры для идентификатора билета и цены
                SqlParameter ticketIdParam = new SqlParameter("@TicketId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(ticketIdParam);

                SqlParameter ticketPriceParam = new SqlParameter("@TicketPrice", SqlDbType.Decimal)
                {
                    Precision = 6,
                    Scale = 2,
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(ticketPriceParam);

                cmd.ExecuteNonQuery();

                // Получаем идентификатор и цену забронированного билета
                if (ticketIdParam.Value != DBNull.Value && ticketPriceParam.Value != DBNull.Value)
                {
                    ticketPrice = (decimal)ticketPriceParam.Value;
                    return (int)ticketIdParam.Value;
                }

                // Возвращаем 0, если бронирование не удалось
                ticketPrice = 0;
                return 0;
            }
        }

        private List<Tuple<int, int>> selectedSeats = new List<Tuple<int, int>>();

        private void SeatButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;

            var seatInfo = (Tuple<int, int>)clickedButton.Tag;
            int rowNumber = seatInfo.Item1;
            int seatNumber = seatInfo.Item2;

            // Проверяем, есть ли данное место в списке выбранных мест
            var selectedSeat = selectedSeats.Find(seat => seat.Item1 == rowNumber && seat.Item2 == seatNumber);

            if (selectedSeat == null)
            {
                // Место не выбрано - добавляем в список
                selectedSeats.Add(Tuple.Create(rowNumber, seatNumber));
            }
            else
            {
                // Место уже выбрано - удаляем из списка
                selectedSeats.Remove(selectedSeat);
            }

            // Обновляем информацию на label17
            UpdateLabel();

            // Дополнительный код по изменению внешнего вида кнопок
            UpdateButtonAppearance(clickedButton);
        }

        private void UpdateLabel()
        {
            string labelText = "Выбранные места: ";
            if (selectedSeats.Count > 0)
            {
                labelText += string.Join(", ", selectedSeats.Select(seat => $"(Ряд {seat.Item1} место {seat.Item2 + 1})"));
                button2.ForeColor = Color.White;
                button2.BackColor = SystemColors.Highlight;
                button2.Text = "Забронировать";
            }
            else
            {
                button2.ForeColor = Color.Gray;
                button2.BackColor = Color.Gainsboro;
                button2.Text = "Место не выбрано";
                labelText = "";
            }

            label18.Text = labelText;
        }

        private void UpdateButtonAppearance(Button clickedButton)
        {
            // Обновляем внешний вид кнопки в зависимости от того, выбрано место или нет
            var seatInfo = (Tuple<int, int>)clickedButton.Tag;
            int rowNumber = seatInfo.Item1;
            int seatNumber = seatInfo.Item2;

            var selectedSeat = selectedSeats.Find(seat => seat.Item1 == rowNumber && seat.Item2 == seatNumber);

            if (selectedSeat == null)
            {
                clickedButton.BackColor = Color.GreenYellow;
            }
            else
            {
                clickedButton.BackColor = SystemColors.Highlight;
            }
        }


        private void button10_Click(object sender, EventArgs e)
        {
            panel1.Focus();
            newHandler?.Invoke();
        }

        private void button74_Click(object sender, EventArgs e)
        {
            listBox1.Visible = true;
        }
        List<SessionInfo> sessionInfoList = new List<SessionInfo>();
        private List<SessionInfo> GetSessionInfo(int filmId)
        {

            using (SqlConnection connection = new SqlConnection(Program.connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM GetSessionInfo(@filmId)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@filmId", filmId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SessionInfo sessionInfo = new SessionInfo
                            {
                                SchedId = reader.GetInt32(reader.GetOrdinal("Session_id")),
                                FilmTitle = reader.GetString(reader.GetOrdinal("Title")),
                                SessionTime = reader.GetDateTime(reader.GetOrdinal("Session_time")),
                                CinemaName = reader.GetString(reader.GetOrdinal("CinemaName")),
                                CinemaAddress = reader.GetString(reader.GetOrdinal("Address")),
                                FilmFormat = reader.GetString(reader.GetOrdinal("Film_Format")),
                                AgeLimit = reader.GetByte(reader.GetOrdinal("AgeLimit")),
                                HallName = reader.GetString(reader.GetOrdinal("HallName"))
                            };
                            sessionInfoList.Add(sessionInfo);
                        }
                    }
                }
            }

            return sessionInfoList;
        }

        public class SessionInfo
        {
            public int SchedId { get; set; }
            public string FilmTitle { get; set; }
            public DateTime SessionTime { get; set; }
            public string CinemaName { get; set; }
            public string CinemaAddress { get; set; }
            public string FilmFormat { get; set; }
            public byte AgeLimit { get; set; }
            public string HallName { get; set; }
        }

        public class SeatInfo
        {
            public int SeatId { get; set; }
            public int SeatsNumber { get; set; }
            public int RowNumber { get; set; }
        }

        private void GetAvailableSeatsCount()
        {
            using (SqlConnection connection = new SqlConnection(Program.connectionString))
            {
                // Открытие подключения
                connection.Open();

                // Создание команды для вызова хранимой процедуры с использованием инлайн функции
                using (SqlCommand command = new SqlCommand("SELECT dbo.GetAvailableSeats(@sessionId) AS Seats", connection))
                {
                    command.Parameters.AddWithValue("@sessionId", sessionInfoList[listBox1.SelectedIndex].SchedId);

                    // Выполнение команды и получение результата
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Получение возраста из результата запроса
                            int count = Convert.ToInt32(reader["Seats"]);
                            label2.Text = $"Осталось  {count}  место";
                        }
                    }
                }
            }
        }

        static decimal GetTicketPrice(int sessionId)
        {
            decimal ticketPrice = 0;

            using (SqlConnection connection = new SqlConnection(Program.connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT dbo.CalculateTicketPrice(@SessionId)", connection))
                {
                    cmd.Parameters.AddWithValue("@SessionId", sessionId);

                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        ticketPrice = Convert.ToDecimal(result);
                    }
                }
            }

            return ticketPrice;
        }

        public void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var info = sessionInfoList[listBox1.SelectedIndex];
            if (listBox1.SelectedIndex != -1)
            {
                selectedSessionId = info.SchedId;
                label5.Text = "«" + info.FilmTitle + "»";
                label9.Text = $"{info.SessionTime.ToString("d MMMM", new CultureInfo("ru-RU"))}. Кинотеатр {info.CinemaName}";
                label21.Text = GetTicketPrice(info.SchedId).ToString() + "₽";
                label1.Text = $"{info.FilmFormat}   {info.AgeLimit}+   Зал {info.HallName}";
                GetAvailableSeatsCount();
            }

            button74.Text = sessionInfoList[listBox1.SelectedIndex].SessionTime.ToString("HH:mm");

            listBox1.Visible = false;
            // Сбросить выбранные места
            selectedSeats.Clear();
            UpdateLabel();
            LoadSeatAvailabilityCache(info.SchedId);
            LoadSeats();
        }

        int selectedSessionId = 0;
        private void button2_Click(object sender, EventArgs e)
        {
            if (label18.Text == "")
            {
                SystemSounds.Exclamation.Play();
                return;
            }
            string personDetails = "";
            var info = sessionInfoList[listBox1.SelectedIndex];
            decimal ticketPrice = 100; // Добавленная переменная для цены билета


            PersonDialog personDialog = new PersonDialog();
            if (personDialog.ShowDialog() == DialogResult.OK)
            {
                personDetails = personDialog.textBox1.Text;
            }

            // Начало транзакции
            using (SqlConnection connection = new SqlConnection(Program.connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Зарезервировать все выбранные места
                    foreach (var seat in selectedSeats)
                    {
                        ticketId = ReserveTicket(filmId, selectedSessionId, seat.Item1, seat.Item2, personDetails, connection, transaction, out ticketPrice);

                        // Если ticketId <= 0, то возникла ошибка при резервировании места
                        if (ticketId <= 0)
                        {
                            // Откатываем транзакцию
                            transaction.Rollback();
                            MessageBox.Show("Бронирование не удалось. Место возможно уже занято.");
                            return;
                        }
                    }

                    string[] ticketInfoLines = new string[]
                    {
                        $"{personDetails}",
                        $"{info.FilmTitle}",
                        $"{info.CinemaName}",
                        $"{info.HallName}",
                        $"{info.CinemaAddress}",
                        $"{info.SessionTime.ToString("d MMMM HH:mm")}",
                        $"{GetReservationDetails()}",
                        $"{info.FilmFormat}",
                        $"{info.AgeLimit}",
                        $"{ticketPrice}"
                    };

                    // Передача массива строк или его использование по мере необходимости
                    print = new TicketConfirmationPrint(ticketInfoLines);

                    print.label2.Focus();
                    if (print.ShowDialog() == DialogResult.OK)
                    {
                        transaction.Commit(); // Фиксируем транзакцию
                        print.button1.Visible = true;
                        print.button7.Visible = true;
                        print.label2.Visible = true;
                        print.button2.Visible = false;
                        print.Show();
                    }
                    else
                    {
                        transaction.Rollback();
                    }

                    // Сбросить выбранные места
                    selectedSeats.Clear();
                    UpdateLabel();
                    LoadSeatAvailabilityCache(info.SchedId);
                    GetAvailableSeatsCount();
                    LoadSeats();
                    }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Ошибка при бронировании билетов: {ex.Message}");
                }
        }
        }

        public string GetReservationDetails()
        {
            if (selectedSeats.Count > 0)
            {
                return string.Join(",", selectedSeats.Select(seat =>
                    $"Билет {GetTicketNumber(seat.Item1, seat.Item2)}: Ряд {seat.Item1} - место {seat.Item2 + 1}"));
            }
            else
            {
                return "Нет забронированных билетов";
            }
        }


        private string GetTicketNumber(int rowNumber, int seatNumber)
        {
            return $"{rowNumber}{seatNumber}";
        }

        private void label18_TextChanged(object sender, EventArgs e)
        {
            if (label18.Text.Length > 200)
            {
                label18.Font = new Font("Microsoft Sans Serif", 7f, FontStyle.Regular);
            }
        }

    }
}
