using System;
using System.Globalization;
using System.IO;

namespace QuanLyNguoiDungApp.Helpers
{
    public static class TokenManager
    {
        private static readonly string tokenFile = "user_token.txt";

        public static void SaveToken(string token, DateTime expires)
        {
            File.WriteAllText(tokenFile, $"{token}|{expires:o}");
        }

        public static bool HasValidToken()
        {
            if (!File.Exists(tokenFile)) return false;

            string[] parts = File.ReadAllText(tokenFile).Split('|');
            if (parts.Length != 2) return false;

            if (!DateTime.TryParseExact(parts[1], "O", null,
                DateTimeStyles.RoundtripKind, out DateTime expires))
                return false;

            return DateTime.Now < expires;
        }

        public static string GetToken()
        {
            if (!File.Exists(tokenFile)) return null;
            string[] parts = File.ReadAllText(tokenFile).Split('|');
            return parts[0];
        }

        public static void ClearToken()
        {
            if (File.Exists(tokenFile))
                File.Delete(tokenFile);
        }
    }
}
