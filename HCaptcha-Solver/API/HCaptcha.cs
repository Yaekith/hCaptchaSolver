using Alturos.Yolo;
using HCaptcha_Solver.JSON;
using HCaptcha_Solver.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HCaptcha_Solver.API
{
    public class HCaptcha
    {
        private string SiteKey { get; set; }

        private string Host { get; set; }

        private HttpClient Client { get; set; }

        private Yolo YoloClient { get; set; }

        private HCaptchaSiteConfig Config { get; set; }

        public HCaptcha(string sitekey, string host)
        {
            SiteKey = sitekey;
            Host = host;
            Client = new HttpClient();
            YoloClient = new Yolo();
            Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.128 Safari/537.36");
        }

        private HCaptchaSiteConfig GetSiteConfig()
        {
            var request = Client.GetAsync($"https://hcaptcha.com/checksiteconfig?host={Host}&sitekey={SiteKey}&sc=1&swa=1").Result;
            var response = request.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<HCaptchaSiteConfig>(response);
        }

        private HCaptchaGetCaptcha GetCaptcha()
        {
            var config = GetSiteConfig();
            var n = GetN(config.c.req);

            if (n == null)
                return GetCaptcha();

            Config = config;
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("sitekey", SiteKey),
                new KeyValuePair<string, string>("host", Host),
                new KeyValuePair<string, string>("hl", "en"),
                new KeyValuePair<string, string>("motionData", "{}"),
                new KeyValuePair<string, string>("n", n),
                new KeyValuePair<string, string>("c", JsonConvert.SerializeObject(config.c))
            });
            var request = Client.PostAsync($"https://hcaptcha.com/getcaptcha?s={SiteKey}", data).Result;
            var response = request.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<HCaptchaGetCaptcha>(response);
        }

        private HCaptchaCheckCaptchaResponse CheckCaptcha(string check, string key)
        {
            var request = Client.PostAsync($"https://hcaptcha.com/checkcaptcha/{key}?s={SiteKey}", new StringContent(check, Encoding.UTF8, "application/json")).Result;
            var response = request.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<HCaptchaCheckCaptchaResponse>(response);
        }

        public string SolveCaptcha()
        {
            var captcha = GetCaptcha();

            if (captcha.requester_question == null)
                return SolveCaptcha();

            var query = captcha.requester_question.en.Split(' ').Last().Replace("motorbus", "bus");
            Console.WriteLine($"[!] We are looking for a {query} [!]");
            Dictionary<string, string> images = new Dictionary<string, string>();
            
            foreach(var task in captcha.tasklist)
                images.Add(task.datapoint_uri, task.task_key);

            var paths = new List<string>();

            Dictionary<string, string> Answers = new Dictionary<string, string>();

            foreach (var image in images)
                paths.Add(GeneralUtils.DownloadImage(image.Key, image.Value));

            YoloClient.Setup();
            foreach (var path in paths)
            {
                var objects = YoloClient.DetectObjects(path);
                foreach(var _object in objects)
                {
                    var taskkey = path.Split('_')[1].Replace(".png", "");
                    if (!Answers.ContainsKey(taskkey))
                    {
                        if (_object.Confidence > 0.5 && _object.Type == query)
                        {
                            Console.WriteLine("LETS GO");
                            Answers.Add(taskkey, "true");
                        } 
                        else
                            Answers.Add(taskkey, "false");
                    }
                }
            }

            var answer = CompileAnswer(Answers);
            Console.WriteLine(CheckCaptcha(answer, captcha.key).pass);
            return CheckCaptcha(answer, captcha.key).generated_pass_UUID;
        }

        private string GetN(string req)
        {
            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            chromeDriverService.SuppressInitialDiagnosticInformation = true;
            var options = new ChromeOptions();
            options.AddArguments("headless");
            options.AddArguments("silent");
            var script = File.ReadAllText("n.js");
            var payload = script + "\r\n" + $"return hsw('{req}');";
            var driver = new ChromeDriver(chromeDriverService, options);
            return driver.ExecuteScript(payload).ToString();
        }

        private string CompileAnswer(Dictionary<string, string> answers)
        {
            var n = GetN(Config.c.req);
            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            chromeDriverService.SuppressInitialDiagnosticInformation = true;
            var options = new ChromeOptions();
            options.AddArguments("headless");
            options.AddArguments("silent");
            var script = File.ReadAllText("compileanswer.js");
            var dateTimeOffset = DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000;
            var value1 = Math.Round((double)dateTimeOffset).ToString();
            var setanswers = "";

            foreach(var answer in answers)
                setanswers += $"answers['{answer.Key}'] = '{answer.Value}'\n";

            var payload = script + "\r\n" + setanswers + "\r\n" + $"return compile('{Config.c.req}', '{Config.c.type}', '{Host}', '{SiteKey}', '{n}', '{value1}', '{value1}')";
            var driver = new ChromeDriver(chromeDriverService, options);
            return driver.ExecuteScript(payload).ToString();
        }
    }
}
