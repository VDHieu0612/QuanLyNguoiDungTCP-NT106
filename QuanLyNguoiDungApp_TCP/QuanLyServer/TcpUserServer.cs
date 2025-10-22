// TcpUserServer.cs
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.IO;

namespace QuanLyServer
{
    public class TcpUserServer
    {
        private TcpListener listener;
        private string connectionString = "Server=localhost;Database=YourDbName;User Id=sa;Password=yourPassword;"; // <-- sửa đây
        private ConcurrentDictionary<string, (int userId, DateTime expires)> tokens = new ConcurrentDictionary<string, (int, DateTime)>();

        public TcpUserServer(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            listener.Start();
            Console.WriteLine("Server listening on port " + ((IPEndPoint)listener.LocalEndpoint).Port);
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Thread t = new Thread(() => HandleClient(client));
                t.IsBackground = true;
                t.Start();
            }
        }

        private void HandleClient(TcpClient client)
        {
            using (client)
            using (NetworkStream ns = client.GetStream())
            using (StreamReader reader = new StreamReader(ns, Encoding.UTF8))
            using (StreamWriter writer = new StreamWriter(ns, Encoding.UTF8) { AutoFlush = true })
            {
                try
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Console.WriteLine("REQ: " + line);
                        try
                        {
                            var doc = JsonDocument.Parse(line);
                            string action = doc.RootElement.GetProperty("action").GetString();
                            var data = doc.RootElement.GetProperty("data");

                            string response = action switch
                            {
                                "register" => HandleRegister(data),
                                "login" => HandleLogin(data),
                                "userinfo" => HandleUserInfo(data),
                                _ => JsonSerializer.Serialize(new { status = "error", action = action, message = "Unknown action" })
                            };

                            writer.WriteLine(response);
                            Console.WriteLine("RESP: " + response);
                        }
                        catch (Exception ex)
                        {
                            var err = JsonSerializer.Serialize(new { status = "error", message = "Invalid request: " + ex.Message });
                            writer.WriteLine(err);
                        }
                    }
                }
                catch (IOException) { /* connection closed */ }
                catch (Exception ex) { Console.WriteLine("Client handler exception: " + ex.Message); }
            }
            Console.WriteLine("Client disconnected");
        }

        private string HandleRegister(JsonElement data)
        {
            string username = data.GetProperty("username").GetString();
            string passwordHash = data.GetProperty("password").GetString();
            string email = data.TryGetProperty("email", out var e) ? e.GetString() : null;
            string fullname = data.TryGetProperty("fullname", out var f) ? f.GetString() : null;
            string birthday = data.TryGetProperty("birthday", out var b) ? b.GetString() : null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand check = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username=@u", conn))
                    {
                        check.Parameters.AddWithValue("@u", username);
                        int cnt = (int)check.ExecuteScalar();
                        if (cnt > 0)
                            return JsonSerializer.Serialize(new { status = "error", action = "register", message = "Username already exists" });
                    }

                    using (SqlCommand insert = new SqlCommand("INSERT INTO Users (Username, Password, Email, FullName, Birthday) VALUES (@u,@p,@e,@f,@b); SELECT SCOPE_IDENTITY();", conn))
                    {
                        insert.Parameters.AddWithValue("@u", username);
                        insert.Parameters.AddWithValue("@p", passwordHash);
                        insert.Parameters.AddWithValue("@e", (object)email ?? DBNull.Value);
                        insert.Parameters.AddWithValue("@f", (object)fullname ?? DBNull.Value);
                        if (!string.IsNullOrEmpty(birthday) && DateTime.TryParse(birthday, out DateTime bd))
                            insert.Parameters.AddWithValue("@b", bd);
                        else
                            insert.Parameters.AddWithValue("@b", DBNull.Value);

                        var idObj = insert.ExecuteScalar();
                        int newId = Convert.ToInt32(idObj);
                        return JsonSerializer.Serialize(new { status = "ok", action = "register", message = "Register successful", data = new { userid = newId, username } });
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { status = "error", action = "register", message = "Server error: " + ex.Message });
            }
        }

        private string HandleLogin(JsonElement data)
        {
            string username = data.GetProperty("username").GetString();
            string passwordHash = data.GetProperty("password").GetString();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT UserId, Username, FullName FROM Users WHERE Username=@u AND Password=@p", conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@p", passwordHash);
                        using (var r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                int uid = r.GetInt32(0);
                                string uname = r.GetString(1);
                                string fullname = r.IsDBNull(2) ? "" : r.GetString(2);
                                string token = Guid.NewGuid().ToString();
                                tokens[token] = (uid, DateTime.UtcNow.AddHours(2));
                                return JsonSerializer.Serialize(new { status = "ok", action = "login", message = "Login successful", data = new { userid = uid, username = uname, fullname, token } });
                            }
                            else
                            {
                                return JsonSerializer.Serialize(new { status = "error", action = "login", message = "Invalid username or password" });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { status = "error", action = "login", message = "Server error: " + ex.Message });
            }
        }

        private string HandleUserInfo(JsonElement data)
        {
            string token = data.GetProperty("token").GetString();
            if (!tokens.TryGetValue(token, out var t)) return JsonSerializer.Serialize(new { status = "error", action = "userinfo", message = "Invalid token" });
            if (t.expires < DateTime.UtcNow) { tokens.TryRemove(token, out _); return JsonSerializer.Serialize(new { status = "error", action = "userinfo", message = "Token expired" }); }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT UserId, Username, Email, FullName, Birthday FROM Users WHERE UserId=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", t.userId);
                        using (var r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                return JsonSerializer.Serialize(new
                                {
                                    status = "ok",
                                    action = "userinfo",
                                    data = new
                                    {
                                        userid = r.GetInt32(0),
                                        username = r.GetString(1),
                                        email = r.IsDBNull(2) ? "" : r.GetString(2),
                                        fullname = r.IsDBNull(3) ? "" : r.GetString(3),
                                        birthday = r.IsDBNull(4) ? "" : (r.GetDateTime(4)).ToString("yyyy-MM-dd")
                                    }
                                });
                            }
                            else
                                return JsonSerializer.Serialize(new { status = "error", action = "userinfo", message = "User not found" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { status = "error", action = "userinfo", message = "Server error: " + ex.Message });
            }
        }
    }
}
