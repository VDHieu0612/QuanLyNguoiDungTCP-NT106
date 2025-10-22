using QuanLyNguoiDungApp.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyNguoiDungApp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Kiểm tra kết nối đến db
            // Application.Run(new Form1());
            // Chạy chương trình
            Application.Run(new SignupForm());
        }
    }
}
