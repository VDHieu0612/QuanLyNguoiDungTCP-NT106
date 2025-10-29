using QuanLyNguoiDungApp.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyNguoiDungApp.Forms
{
    public partial class SignupForm : Form
    {
        public SignupForm()
        {
            InitializeComponent();
        }

        // Kiểm tra ngày tháng hợp lệ
        private bool KiemTraNgayThang(int ngay, int thang, int nam)
        {
            // Kiểm tra tháng hợp lệ
            if (thang < 1 || thang > 12) return false;

            // Kiểm tra năm nhuận
            bool laNamNhuan = (nam % 400 == 0) || (nam % 4 == 0 && nam % 100 != 0);

            // Số ngày tối đa của từng tháng
            int[] soNgayTrongThang = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            // Nếu là năm nhuận thì tháng 2 có 29 ngày
            if (laNamNhuan) soNgayTrongThang[1] = 29;

            // Kiểm tra ngày hợp lệ
            return (ngay >= 1 && ngay <= soNgayTrongThang[thang - 1]);
        }
        // Đăng ký
        private void btnDangKy_Click(object sender, EventArgs e)
        {
            string email = EmailField.Text.Trim();
            string username = UsernameField.Text.Trim();
            string phone = PhonenumField.Text.Trim();
            string dateofbirth = DoBField.Text.Trim(); // dạng dd/MM/yyyy
            string password = PasswordField.Text;
            string confirmpassword = CPField.Text;

            // Kiểm tra nhập đủ thông tin bắt buộc
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(dateofbirth) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(confirmpassword))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ các thông tin bắt buộc.");
                return;
            }

            // Kiểm tra email hợp lệ
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Email không hợp lệ.");
                return;
            }

            // Username chỉ chứa chữ + số
            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9]+$"))
            {
                MessageBox.Show("Username chỉ được chứa chữ và số.");
                return;
            }

            // Kiểm tra password
            if (password.Length < 8 ||
                !Regex.IsMatch(password, @"[a-z]") ||
                !Regex.IsMatch(password, @"[A-Z]") ||
                !Regex.IsMatch(password, @"[0-9]") ||
                !Regex.IsMatch(password, @"[!@#$%^&*(),.?:{}|<>]"))
            {
                MessageBox.Show("Password phải chứa ít nhất 8 ký tự, bao gồm 1 ký tự thường, 1 ký tự in hoa, 1 ký tự số, 1 ký tự đặc biệt.");
                return;
            }

            if (password != confirmpassword)
            {
                MessageBox.Show("Confirm password không khớp.");
                return;
            }

            // Kiểm tra số điện thoại (nếu nhập)
            if (!string.IsNullOrEmpty(phone))
            {
                if (!Regex.IsMatch(phone, @"^0\d{9}$"))
                {
                    MessageBox.Show("Số điện thoại phải có 10 số và bắt đầu bằng 0.");
                    return;
                }
            }

            // Kiểm tra ngày sinh
            if (!DateTime.TryParseExact(dateofbirth, "dd/MM/yyyy", null,
                System.Globalization.DateTimeStyles.None, out DateTime dob))
            {
                MessageBox.Show("Ngày sinh không đúng định dạng dd/MM/yyyy.");
                return;
            }

            if (!KiemTraNgayThang(dob.Day, dob.Month, dob.Year))
            {
                MessageBox.Show("Ngày sinh không hợp lệ.");
                return;
            }

            int age = DateTime.Now.Year - dob.Year;
            if (dob > DateTime.Now.AddYears(-age)) age--;
            if (age < 12)
            {
                MessageBox.Show("Người dùng phải lớn hơn 12 tuổi.");
                return;
            }

            // Gửi request đăng ký TCP đến server
            try
            {
                string passwordHash = SecurityHelper.HashPassword(password);

                string response = TcpUserClient.Register(
                    "Nguoichoi",
                    email,
                    username,
                    phone,
                    passwordHash,
                    dob
                );

                dynamic result = JsonConvert.DeserializeObject(response);

                if (result.status == "success")
                {
                    // Lưu token và thời gian hết hạn xuống bộ nhớ (RAM hoặc file)
                    string token = result.token;
                    DateTime expires = DateTime.Now.AddMinutes(1); // 1 phút để kiểm tra

                    TokenManager.SaveToken(token, expires);

                    MessageBox.Show("Đăng ký thành công!");
                    ProfileForm profile = new ProfileForm(email);
                    this.Hide();
                    profile.Show();
                }
                else
                {
                    MessageBox.Show("Đăng ký thất bại: " + (string)result.message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đăng ký: " + ex.Message);
            }
        }


        private void linkDangNhap_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Mở form Login
            LoginForm login = new LoginForm();
            this.Hide();
            login.Show();
        }
        private void SignupForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
