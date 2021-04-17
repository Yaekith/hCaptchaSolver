using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCaptcha_Solver.JSON
{
    public class HCaptchaSiteConfig
    {
        public bool pass { get; set; }
        public C c { get; set; }
    }

    public class C
    {
        public string type { get; set; }
        public string req { get; set; }
    }
}
