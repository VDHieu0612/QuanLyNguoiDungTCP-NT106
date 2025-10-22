using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace QuanLyNguoiDungApp.Helpers
{
    public static class TcpUserClient
    {
        private const string SERVER_IP = "127.0.0.1";
        private const int SERVER_PORT = 5050;
        // Token lưu tạm trong client
        public static string Token { get; private set; }

        private static string SendRequest(Dictionary<string, string> request)
        {
            using (TcpClient client = new TcpClient(SERVER_IP, SERVER_PORT))
            using (NetworkStream stream = client.GetStream())
            {
                string json = JsonConvert.SerializeObject(request);
                byte[] data = Encoding.UTF8.GetBytes(json);
                stream.Write(data, 0, data.Length);

                byte[] buffer = new byte[4096];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
        }

        public static string Login(string email, string passwordHash)
        {
            var req = new Dictionary<string, string>
            {
                { "action", "login" },
                { "email", email },
                { "passwordHash", passwordHash }
            };
            string resp = SendRequest(req);
            dynamic result = JsonConvert.DeserializeObject(resp);
            if (result.status == "success")
                Token = result.token;
            return resp;
        }

        public static string Register(string loaiUser, string email, string username, string sdt, string passwordHash, DateTime ngaySinh)
        {
            var req = new Dictionary<string, string>
            {
                { "action", "register" },
                { "loaiUser", loaiUser },
                { "email", email },
                { "username", username },
                { "sdt", sdt },
                { "passwordHash", passwordHash },
                { "ngaySinh", ngaySinh.ToString("yyyy-MM-dd") }
            };
            return SendRequest(req);
        }

        public static string GetUserInfo(string email)
        {
            var req = new Dictionary<string, string>
            {
                { "action", "getinfo" },
                { "token", Token ?? "" }
            };
            return SendRequest(req);
        }
        public static void Logout()
        {
            Token = null;
        }
    }
}
