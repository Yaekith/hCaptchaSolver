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
            Console.ForegroundColor = ConsoleColor.White;
            SolveCaptcha();
            Console.ReadKey();
        }

        private static string SolveCaptcha()
        {
            HCaptcha captcha = new HCaptcha("f5561ba9-8f1e-40ca-9b5b-a0b3f719ef34", "discord.com", true);
            var solved = captcha.SolveCaptcha();
            if (solved.pass)
                return solved.token;
            else
                return SolveCaptcha();
        }
    }
}
