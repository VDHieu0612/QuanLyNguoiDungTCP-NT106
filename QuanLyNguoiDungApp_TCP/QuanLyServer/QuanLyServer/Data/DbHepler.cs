using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyServer.Data
{
    internal class DbHepler
    {
        public static class DbHelper
        {
            // Lấy connection string từ App.config
            private static string connectionString =>
                ConfigurationManager.ConnectionStrings["Userdb"].ConnectionString;
            //private static readonly string connectionString = "Server=LENOVO-YOGA-PRO;Database=QUANLYNGUOIDUNG;Trusted_Connection=True;User Id=tester;Password=hieu06122005;";

            //
            // Kiểm tra email đã tồn tại hay chưa
            //
            public static bool EmailExists(string Email)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(1) FROM USERS WHERE Email = @e", connection))
                {
                    cmd.Parameters.AddWithValue("@e", Email);
                    connection.Open();
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }

            //
            // Kiểm tra username đã tồn tại hay chưa
            //
            public static bool UsernameExists(string Username)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(1) FROM USERS WHERE Username = @u", connection))
                {
                    cmd.Parameters.AddWithValue("@u", Username);
                    connection.Open();
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }

            //
            // Đăng ký user mới
            // 
            public static void RegisterUser(
                string loaiUser,
                string email,
                string username,
                string sdt,
                string passwordHash,
                DateTime ngaySinh)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO USERS (LoaiUser, Email, Username, SDT, MatKhauHash, NgaySinh)
                VALUES (@loai, @e, @u, @sdt, @p, @ngay)", conn))
                {
                    cmd.Parameters.AddWithValue("@loai", loaiUser);
                    cmd.Parameters.AddWithValue("@e", email);
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@sdt", sdt);
                    cmd.Parameters.AddWithValue("@p", passwordHash);
                    cmd.Parameters.AddWithValue("@ngay", ngaySinh);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            //
            // Lấy mật khẩu hash từ DB theo Email
            //
            public static string GetPasswordHash(string Email)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("SELECT MatKhauHash FROM USERS WHERE Email = @e", conn))
                {
                    cmd.Parameters.AddWithValue("@e", Email);
                    conn.Open();
                    return cmd.ExecuteScalar() as string;
                }
            }

            //
            // Tìm thông tin người dùng
            public static Dictionary<string, object> GetUserInfo(string email)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT Email, UserName, SDT, NgaySinh, LoaiUser " +
                    "FROM USERS WHERE Email = @e", conn))
                {
                    cmd.Parameters.AddWithValue("@e", email);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Dictionary<string, object>
                {
                    { "Email", reader["Email"] },
                    { "UserName", reader["UserName"] },
                    { "SDT", reader["SDT"] },
                    { "NgaySinh", reader["NgaySinh"] },
                    { "LoaiUser", reader["LoaiUser"] }
                };
                        }
                    }
                }
                return null;
            }
        }
    }
}

