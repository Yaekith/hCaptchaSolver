using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HCaptcha_Solver.Utils
{
    public static class GeneralUtils
    {
        private static Random random = new Random();

        private static WebClient Client = new WebClient();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string DownloadImage(string url, string extra)
        {
            var name = RandomString(20) + $"_{extra}.png";
            Client.DownloadFile(url, $"Images\\{name}");
            return $"Images\\{name}";
        }
    }
}
