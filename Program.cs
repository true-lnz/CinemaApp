using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CINEMA_APP
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CinemaMainForm());

        }
        //public static string connectionString = @"Data Source=LAPTOP-PCU2VFP9\SQLEXPRESS01;Initial Catalog=Cinema_1;Integrated Security=True";
        public static string connectionString = @"Data Source=DESKTOP-MK9RMFI\KAZAKKULOV_URAL;Initial Catalog=2;Integrated Security=True";

    }
}
