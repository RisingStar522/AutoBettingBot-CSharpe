using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using BustabitCrash.Forms;
using BustabitCrash.Games;
using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace BustabitCrash
{
    class Core
    {
        public static string logname = DateTime.Now.ToString("dd-MM-yyyy");
        public static Random rand = new Random();
        public static bool running = true;
        public static string XPJS = "function getElementByXpath(path) { return document.evaluate(path, document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;}";

        public static ChromeDriver StartBot()
        {
            Log("Starting bot...");
            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            
            var options = new ChromeOptions();
            options.AddArgument("mute-audio");
            options.AddArgument("--window-size=1000,700");
            options.AddArgument("ignore-certificate-errors");
            options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36");
            var driver = new ChromeDriver(chromeDriverService, options);            
            return driver;
        }

        public static void WaitAction(Action action, int timeout = 4)
        {
            int t = 0;
            while (running)
            {
                t++;
                Thread.Sleep(500);
                try
                {
                    action.Invoke();
                    break;
                }
                catch { }
                if (t >= timeout) break;
            }
        }

        public static void Log(string text, bool save = false)
        {
            var log = "[" + DateTime.Now.ToShortTimeString() + "] " + text;
            Console.WriteLine(log);
            if (save) File.AppendAllLines("logs/" + logname + ".txt", new[] { log });
        }

        public static void Stop(ChromeDriver driver)
        {
            Log("Stopping bot...");
            driver.Quit();
        }

        static PickForm form = null;
        public static void OpenPicker()
        {
            if (form != null)
                form.Visible = true;
            else
            {
                Application.EnableVisualStyles();
                Application.Run(form = new PickForm());
            }
        }
        public static void ClosePicker()
        {
            form.Visible = false;
        }

        internal static void log(string v)
        {
            throw new NotImplementedException();
        }
    }

    class Stake
    {
        public static void GoToLogin(ChromeDriver driver)
        {
            Core.Log("Going to stake...");
            driver.Navigate().GoToUrl("https://www.bustabit.com/login");
            Console.ForegroundColor = ConsoleColor.Red;
            Core.Log("Manual login required");
            Console.ForegroundColor = ConsoleColor.White;
           
        }
       
        public static void ApplyGame(ChromeDriver driver)
        {
            String game;
            game = "crash";
            Core.ClosePicker();
            Core.Log("Opening " + game);
            try
            {
                driver.Navigate().GoToUrl("https://bustabit.com/play/");
            }
            catch {}
            Crash.Open(driver, true);
        }

        public static double GetBalance(ChromeDriver driver)
        {
            double balance = -1;
            Core.WaitAction(() =>
            {
                By by = By.TagName("nav");
                var element = driver.FindElement(by);
                var text = element.FindElement(By.CssSelector("._3x9Vhplwvs0D9hkoB9O0v8 > a:nth - child(2) > span")).Text;
                text = text.Replace("Bits:", "").Trim();
                balance = double.Parse(text);
            });            
            return balance;
        }
    }
}
