using QuanLyNguoiDungApp.Helpers;
using Newtonsoft.Json;
using System;
using System.Windows.Forms;

namespace QuanLyNguoiDungApp.Forms
{
    public partial class ProfileForm : Form
    {
        private readonly string _email;
        private System.Windows.Forms.Timer tokenTimer; // Timer kiểm tra token định kỳ

        public ProfileForm(string email)
        {
            InitializeComponent();
            _email = email;

            // Khởi tạo sự kiện form
            this.Load += ProfileForm_Load;
            this.FormClosed += ProfileForm_FormClosed;
        }

        // Load form
        private void ProfileForm_Load(object sender, EventArgs e)
        {
            StartTokenMonitor();
            LoadUserInfo();
        }

        // Kiểm tra token định kỳ
        private void StartTokenMonitor()
        {
            tokenTimer = new System.Windows.Forms.Timer();
            tokenTimer.Interval = 10000; // 10 giây kiểm tra 1 lần
            tokenTimer.Tick += (s, e) =>
            {
                if (!TokenManager.HasValidToken())
                {
                    tokenTimer.Stop();
                    MessageBox.Show("Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại.",
                        "Token hết hạn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LogoutAndReturn();
                }
            };
            tokenTimer.Start();
        }

        // Tải thông tin người dùng
        private void LoadUserInfo()
        {
            try
            {
                // Kiểm tra token còn hạn không
                if (!TokenManager.HasValidToken())
                {
                    MessageBox.Show("Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại.");
                    LogoutAndReturn();
                    return;
                }

                string token = TokenManager.GetToken();
                string response = TcpUserClient.GetUserInfo(token);
                dynamic result = JsonConvert.DeserializeObject(response);

                if (result.status == "success")
                {
                    var user = result.user;
                    EmailField.Text = user.Email;
                    UsernameField.Text = user.UserName;
                    PhonenumField.Text = user.SDT;
                    DoBField.Text = user.NgaySinh;
                    RoleField.Text = user.LoaiUser;
                }
                else
                {
                    MessageBox.Show((string)result.message);
                    LogoutAndReturn();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải thông tin người dùng: " + ex.Message);
            }
        }

        // Đăng xuất
        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            // Xoá token
            TokenManager.ClearToken();
            // Quay về form đăng nhập
            LogoutAndReturn();
        }

        // Đăng xuất và quay lại form Đăng nhập
        private void LogoutAndReturn()
        {
            try
            {
                tokenTimer?.Stop();
                TokenManager.ClearToken();

                this.Hide();
                LoginForm login = new LoginForm();
                login.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đăng xuất: " + ex.Message);
            }
        }

        // 
        private void ProfileForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            tokenTimer?.Stop();
            Application.Exit();
        }
    }
}
