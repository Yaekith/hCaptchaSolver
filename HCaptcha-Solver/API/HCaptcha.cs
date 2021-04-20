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
using System.Web;

namespace HCaptcha_Solver.API
{
    public class HCaptcha
    {
        private string SiteKey { get; set; }

        private string Host { get; set; }

        private HttpClient Client { get; set; }

        private Yolo YoloClient { get; set; }

        private HCaptchaSiteConfig Config { get; set; }

        private bool _Debug { get; set; }

        public HCaptcha(string sitekey, string host, bool debug = false)
        {
            SiteKey = sitekey;
            Host = host;
            _Debug = debug;
            Client = new HttpClient();
            YoloClient = new Yolo();
            Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.128 Safari/537.36");

            if (!Directory.Exists("Images"))
                Directory.CreateDirectory("Images");
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
            var n = config.c.type == "hsl" ? GetNV2(config.c.req) : GetNV1(config.c.req);

            if (n == null)
                return GetCaptcha();

            Config = config;
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("v", "89f9b6a"),
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

        public HCaptchaSolvedCaptchaResponse SolveCaptcha()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var captcha = GetCaptcha();

            if (captcha.requester_question == null)
                return SolveCaptcha();

            Debug("Retrieved new captcha.");

            var query = captcha.requester_question.en.Split(' ').Last();
            var fixedword = GeneralUtils.GetFixedWord(query);
            DebugInfo($"We are looking for a {query} {(fixedword != query ? $"or {fixedword}" : "")}");
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
                foreach (var _object in objects)
                {
                    var taskkey = path.Split('_')[1].Replace(".png", "");
                    if (!Answers.ContainsKey(taskkey))
                    {
                        DebugSuccess($"Recognised {_object.Type} -> with {_object.Confidence} confidence");

                        if (_object.Confidence > 0.5 && (_object.Type == query || _object.Type == GeneralUtils.GetFixedWord(query)))
                        {
                            DebugInfo($"{taskkey} is probably correct -> {_object.Type} does match {query} {(fixedword != query ? $"or {fixedword}" : "")}");
                            Answers.Add(taskkey, "true");
                        }
                        else
                        {
                            DebugInfo($"{taskkey} is probably not correct -> {_object.Type} does not match {query} {(fixedword != query ? $"or {fixedword}" : "")}");
                            Answers.Add(taskkey, "false");
                        }

                        File.Delete(path);
                    }
                }
            }

            var answer = CompileAnswer(Config.c.type, Answers);
            var solved = CheckCaptcha(answer, captcha.key);

            watch.Stop();

            if (solved.pass)
                DebugSuccess($"Challenge Passed. UUID: {solved.generated_pass_UUID}");
            else
                DebugError($"Challenge not passed. Something went wrong with solving the captcha.");

            DebugInfo($"Finished with challenge. Took {watch.ElapsedMilliseconds / 1000} second(s)");

            return new HCaptchaSolvedCaptchaResponse()
            {
                token = solved.generated_pass_UUID,
                pass = solved.pass
            };
        }

        private string GetNV1(string req)
        {
            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            chromeDriverService.SuppressInitialDiagnosticInformation = true;
            var options = new ChromeOptions();
            options.AddArguments("headless");
            options.AddArguments("silent");
            var script = File.ReadAllText("hsw.js");
            var payload = script + "\r\n" + $"return hsw('{req}');";
            var driver = new ChromeDriver(chromeDriverService, options);
            var result = driver.ExecuteScript(payload).ToString();
            driver.Close();
            return result;
        }

        private string GetNV2(string req)
        {
            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            chromeDriverService.SuppressInitialDiagnosticInformation = true;
            var options = new ChromeOptions();
            options.AddArguments("headless");
            options.AddArguments("silent");
            var script = File.ReadAllText("hsl.js");
            var payload = script + "\r\n" + $"return hsl('{req}');";
            var driver = new ChromeDriver(chromeDriverService, options);
            var result = driver.ExecuteScript(payload).ToString();
            driver.Close();
            return result;
        }

        private string CompileAnswer(string type, Dictionary<string, string> answers)
        {
            var n = type == "hsl" ? GetNV2(Config.c.req) : GetNV1(Config.c.req);

            if (n == null)
                return CompileAnswer(type, answers);

            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            chromeDriverService.SuppressInitialDiagnosticInformation = true;
            var options = new ChromeOptions();
            options.AddArguments("headless");
            options.AddArguments("silent");
            var script = File.ReadAllText("compileanswer.js");
            var setanswers = "";

            foreach(var answer in answers)
                setanswers += $"answers['{answer.Key}'] = '{answer.Value}'\n";

            var payload = script + "\r\n" + setanswers + "\r\n" + $"return compile('{Config.c.req}', '{Config.c.type}', '{Host}', '{SiteKey}', '{n}')";
            var driver = new ChromeDriver(chromeDriverService, options);
            var result = driver.ExecuteScript(payload).ToString();
            driver.Close();
            return result;
        }

        private void Debug(string text)
        {
            if (_Debug)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"[!] {text} [!]");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private void DebugInfo(string text)
        {
            if (_Debug)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[!] {text} [!]");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private void DebugSuccess(string text)
        {
            if (_Debug)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[!] {text} [!]");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private void DebugError(string text)
        {
            if (_Debug)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[!] {text} [!]");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
