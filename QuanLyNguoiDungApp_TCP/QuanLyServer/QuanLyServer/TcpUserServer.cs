using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace QuanLyServer
{
    public class TcpUserServer
    {
        private static TcpListener listener;
        private static string connectionString =
            ConfigurationManager.ConnectionStrings["UserDb"].ConnectionString;
        // Bộ nhớ lưu token đang hoạt động
        private static ConcurrentDictionary<string, (string email, DateTime expires)> activeTokens =
            new ConcurrentDictionary<string, (string, DateTime)>();
        public static void Main()
        {
            int port = 5050;
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            Console.WriteLine($"Server started on port {port}");
            Console.WriteLine("Waiting for clients...\n");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(HandleClient, client);
            }
        }

        private static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[4096];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            Console.WriteLine($"Request: {request}");

            try
            {
                var req = JsonConvert.DeserializeObject<Dictionary<string, string>>(request);
                string action = req["action"];
                string response = "";

                switch (action)
                {
                    case "login":
                        response = HandleLogin(req);
                        break;
                    case "register":
                        response = HandleRegister(req);
                        break;
                    case "getinfo":
                        response = HandleGetInfo(req, new SqlConnection(connectionString));
                        break;
                    default:
                        response = JsonConvert.SerializeObject(new { status = "error", message = "Unknown action" });
                        break;
                }

                byte[] respBytes = Encoding.UTF8.GetBytes(response);
                stream.Write(respBytes, 0, respBytes.Length);
                Console.WriteLine($"Response: {response}");
            }
            catch (Exception ex)
            {
                string errorResp = JsonConvert.SerializeObject(new { status = "error", message = ex.Message });
                stream.Write(Encoding.UTF8.GetBytes(errorResp), 0, errorResp.Length);
            }
            finally
            {
                client.Close();
            }
        }

        private static string HandleLogin(Dictionary<string, string> req)
        {
            string email = req["email"];
            string passwordHash = req["passwordHash"];

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT COUNT(1) FROM USERS WHERE Email=@e AND MatKhauHash=@p";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@e", email);
                    cmd.Parameters.AddWithValue("@p", passwordHash);
                    int count = (int)cmd.ExecuteScalar();
                    if (count == 1)
                    {
                        // Sinh token ngẫu nhiên, hiệu lực 1 phút
                        string token = Guid.NewGuid().ToString("N");
                        DateTime expires = DateTime.Now.AddMinutes(1);
                        activeTokens[token] = (email, expires);

                        return JsonConvert.SerializeObject(new
                        {
                            status = "success",
                            message = "Đăng nhập thành công!",
                            token,
                            expires = expires.ToString("yyyy-MM-dd HH:mm:ss")
                        });
                    }
                    else
                        return JsonConvert.SerializeObject(new { status = "fail", message = "Sai email hoặc mật khẩu!" });
                }
            }
        }

        private static string HandleRegister(Dictionary<string, string> req)
        {
            string email = req["email"];
            string username = req["username"];
            string sdt = req["sdt"];
            string passwordHash = req["passwordHash"];
            DateTime ngaySinh = DateTime.Parse(req["ngaySinh"]);
            string loaiUser = req["loaiUser"];

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Kiểm tra trùng email
                string checkEmail = "SELECT COUNT(1) FROM USERS WHERE Email=@e";
                using (SqlCommand cmd = new SqlCommand(checkEmail, conn))
                {
                    cmd.Parameters.AddWithValue("@e", email);
                    if ((int)cmd.ExecuteScalar() > 0)
                        return JsonConvert.SerializeObject(new { status = "fail", message = "Email đã tồn tại!" });
                }

                // Kiểm tra trùng username
                string checkUser = "SELECT COUNT(1) FROM USERS WHERE UserName=@u";
                using (SqlCommand cmd = new SqlCommand(checkUser, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    if ((int)cmd.ExecuteScalar() > 0)
                        return JsonConvert.SerializeObject(new { status = "fail", message = "Username đã tồn tại!" });
                }

                // Thêm người dùng mới
                string sql = @"INSERT INTO USERS (LoaiUser, Email, UserName, SDT, MatKhauHash, NgaySinh)
                               VALUES (@loai, @e, @u, @sdt, @p, @ngay)";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@loai", loaiUser);
                    cmd.Parameters.AddWithValue("@e", email);
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@sdt", sdt);
                    cmd.Parameters.AddWithValue("@p", passwordHash);
                    cmd.Parameters.AddWithValue("@ngay", ngaySinh);
                    cmd.ExecuteNonQuery();
                    // Sinh token ngẫu nhiên, hiệu lực 1 phút
                    string token = Guid.NewGuid().ToString("N");
                    DateTime expires = DateTime.Now.AddMinutes(1);
                    activeTokens[token] = (email, expires);

                    return JsonConvert.SerializeObject(new
                    {
                        status = "success",
                        message = "Đăng ký thành công!",
                        token,
                        expires = expires.ToString("yyyy-MM-dd HH:mm:ss")
                    });
                }
            }
        }

        private static string HandleGetInfo(Dictionary<string, string> req, SqlConnection sqlConnection)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                if (!req.ContainsKey("token"))
                    return JsonConvert.SerializeObject(new { status = "fail", message = "Thiếu token!" });

                string token = req["token"];

                // Kiểm tra token có hợp lệ hay không
                if (!activeTokens.ContainsKey(token))
                    return JsonConvert.SerializeObject(new { status = "fail", message = "Token không hợp lệ!" });

                var (email, expires) = activeTokens[token];
                if (expires < DateTime.Now)
                {
                    activeTokens.TryRemove(token, out _);
                    return JsonConvert.SerializeObject(new { status = "fail", message = "Token đã hết hạn, vui lòng đăng nhập lại." });
                }

                {
                    conn.Open();
                    string sql = "SELECT Email, UserName, SDT, NgaySinh, LoaiUser FROM USERS WHERE Email=@e";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@e", email);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var userInfo = new
                                {
                                    Email = reader["Email"].ToString(),
                                    UserName = reader["UserName"].ToString(),
                                    SDT = reader["SDT"].ToString(),
                                    NgaySinh = Convert.ToDateTime(reader["NgaySinh"]).ToString("dd/MM/yyyy"),
                                    LoaiUser = reader["LoaiUser"].ToString()
                                };
                                return JsonConvert.SerializeObject(new { status = "success", user = userInfo });
                            }
                            else
                                return JsonConvert.SerializeObject(new { status = "fail", message = "Không tìm thấy người dùng" });
                        }
                    }
                }
            }
        }

        internal void Start()
        {
            throw new NotImplementedException();
        }
    }
}
