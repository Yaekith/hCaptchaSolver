using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCaptcha_Solver.JSON
{
    public class HCaptchaSolvedCaptchaResponse
    {
        public bool pass { get; set; }

        public string token { get; set; }
    }
}
