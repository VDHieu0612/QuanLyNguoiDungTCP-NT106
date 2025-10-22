using QuanLyNguoiDungApp.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyNguoiDungApp.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }
        // Đăng nhập
        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string email = EmailField.Text.Trim();
            string password = PasswordField.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đủ thông tin!");
                return;
            }

            string passwordHash = SecurityHelper.HashPassword(password);
            string response = TcpUserClient.Login(email, passwordHash);

            dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(response);
            if (result.status == "success")
            {
                // Lưu token và thời gian hết hạn xuống bộ nhớ (RAM hoặc file)
                string token = result.token;
                DateTime expires = DateTime.Now.AddMinutes(1); // 1 phút để kiểm tra

                TokenManager.SaveToken(token, expires);

                MessageBox.Show("Đăng nhập thành công!");
                ProfileForm profile = new ProfileForm(email);
                this.Hide();
                profile.Show();
            }
            else
            {
                MessageBox.Show((string)result.message);
            }
        }

        private void linkDangKy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Mở form Signup
            SignupForm signup = new SignupForm();
            this.Hide();
            signup.Show();
        }
        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
}
}
