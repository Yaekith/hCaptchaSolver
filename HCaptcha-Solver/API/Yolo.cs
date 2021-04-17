using Alturos.Yolo;
using Alturos.Yolo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCaptcha_Solver.API
{
    public class Yolo
    {
        private ConfigurationDetector Configuration { get; set; }

        private YoloWrapper Wrapper { get; set; }

        public void Setup()
        {
            Configuration = new ConfigurationDetector();
            Wrapper = new YoloWrapper(Configuration.Detect());
        }

        public IEnumerable<YoloItem> DetectObjects(string path)
        {
            return Wrapper.Detect(path);
        }
    }
}
