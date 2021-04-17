using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCaptcha_Solver.JSON
{
    public class HCaptchaCheckCaptchaResponse
    {
        public C c { get; set; }
        public bool pass { get; set; }
        public string generated_pass_UUID { get; set; }
        public int expiration { get; set; }
    }
}
