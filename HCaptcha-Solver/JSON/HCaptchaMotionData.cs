using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCaptcha_Solver.JSON
{
    public class HCaptchaMotionData
    {
        public long st { get; set; }
        public long[][] mm { get; set; }
        public float mmmp { get; set; }
        public long[][] md { get; set; }
        public int mdmp { get; set; }
        public long[][] mu { get; set; }
        public int mump { get; set; }
        public int v { get; set; }
        public Toplevel topLevel { get; set; }
        public object[] session { get; set; }
        public string[] widgetList { get; set; }
        public string widgetId { get; set; }
        public string href { get; set; }
        public Prev prev { get; set; }
    }

    public class Toplevel
    {
        public bool inv { get; set; }
        public long st { get; set; }
        public Sc sc { get; set; }
        public Nv nv { get; set; }
        public string dr { get; set; }
        public bool exec { get; set; }
        public long[][] wn { get; set; }
        public int wnmp { get; set; }
        public long[][] xy { get; set; }
        public int xymp { get; set; }
        public long[][] mm { get; set; }
        public float mmmp { get; set; }
    }

    public class Sc
    {
        public int availWidth { get; set; }
        public int availHeight { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int colorDepth { get; set; }
        public int pixelDepth { get; set; }
        public int availLeft { get; set; }
        public int availTop { get; set; }
    }

    public class Nv
    {
        public string vendorSub { get; set; }
        public string productSub { get; set; }
        public string vendor { get; set; }
        public int maxTouchPoints { get; set; }
        public Useractivation userActivation { get; set; }
        public object doNotTrack { get; set; }
        public Geolocation geolocation { get; set; }
        public Connection connection { get; set; }
        public Webkittemporarystorage webkitTemporaryStorage { get; set; }
        public Webkitpersistentstorage webkitPersistentStorage { get; set; }
        public int hardwareConcurrency { get; set; }
        public bool cookieEnabled { get; set; }
        public string appCodeName { get; set; }
        public string appName { get; set; }
        public string appVersion { get; set; }
        public string platform { get; set; }
        public string product { get; set; }
        public string userAgent { get; set; }
        public string language { get; set; }
        public string[] languages { get; set; }
        public bool onLine { get; set; }
        public bool webdriver { get; set; }
        public Scheduling scheduling { get; set; }
        public Mediacapabilities mediaCapabilities { get; set; }
        public Permissions permissions { get; set; }
        public Mediasession mediaSession { get; set; }
        public string[] plugins { get; set; }
    }

    public class Useractivation
    {
    }

    public class Geolocation
    {
    }

    public class Connection
    {
    }

    public class Webkittemporarystorage
    {
    }

    public class Webkitpersistentstorage
    {
    }

    public class Scheduling
    {
    }

    public class Mediacapabilities
    {
    }

    public class Permissions
    {
    }

    public class Mediasession
    {
    }

    public class Prev
    {
        public bool escaped { get; set; }
        public bool passed { get; set; }
        public bool expiredChallenge { get; set; }
        public bool expiredResponse { get; set; }
    }
}
