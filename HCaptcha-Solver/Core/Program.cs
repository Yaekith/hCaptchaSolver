using HCaptcha_Solver.API;
using System;
using System.Threading.Tasks;

namespace HCaptcha_Solver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "HCaptcha Solver";
            Console.WriteLine("[!] Retrieving new challenge [!]");
            HCaptcha captcha = new HCaptcha("51829642-2cda-4b09-896c-594f89d700cc", "democaptcha.com");
            var solved = captcha.SolveCaptcha();
            Console.ReadKey();
        }
    }
}
