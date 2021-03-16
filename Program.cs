using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace BustabitCrash
{
    class Program
    {
        public static ChromeDriver bot;
        [STAThread]
        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            Console.SetWindowSize(150, 40);
            DisableConsoleQuickEdit.Go();
            bot = Core.StartBot();
            Stake.GoToLogin(bot);
            Core.OpenPicker();            
            System.Console.ReadKey();
        }

        public static int time()
        {
            return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }
}

namespace BustabitCrash
{
    static class DisableConsoleQuickEdit
    {
        const uint ENABLE_QUICK_EDIT = 0x0040;
        const int STD_INPUT_HANDLE = -10;
        [DllImport("kernel32.dll")]
        static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);
        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        internal static void Go()
        {
            var handle = GetConsoleWindow();
            // Hide

            

            IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);
            uint consoleMode;
            GetConsoleMode(consoleHandle, out consoleMode);
            consoleMode &= ~ENABLE_QUICK_EDIT;
            SetConsoleMode(consoleHandle, consoleMode);

            ShowWindow(handle, 0);
        }
    }

    static class License
    {

        private static int mseconds;

        public static long QuickCheck()
        {
            if (!File.Exists("license.txt")) Exit("Missing license");
            var dt = File.ReadAllText("license.txt");
            var serial = long.Parse(new String(Encoding.ASCII.GetString(Convert.FromBase64String(dt).Reverse().ToArray()).Replace(" ", "").Where(x => Char.IsDigit(x)).ToArray()));
            if (new WebClient().DownloadString("https://hostyouenterprise.com/license/?id=" + GetHWID() + "&l=" + dt) != "ok") Exit("Invalid license");
            if (serial < Program.time()) Exit("License expired");
            return serial;
        }

        public static void Check()
        {
            var serial = License.QuickCheck();
            Core.Log("BustabitCrash 1");
            Core.Log("CREDITS: Cashkill");
            Core.Log("Enjoy My BustabitCrash v1.0.1");
            Core.Log("BustabitCrash - We search for Crypto Treasures");

            //  Core.log("Deep on the bottom of sea we find the treasure");
            var days = (int)((serial - Program.time()) / 86400);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Core.Log("LICENSE EXPIRES: " + (days > 0 ? days + " Days " : "Today"));
            Console.ForegroundColor = ConsoleColor.White;
            Random random = new Random();
            mseconds = random.Next(6, 10) * 1000;
            System.Threading.Thread.Sleep(mseconds);
        }

        public static void Exit(string message)
        {
            Core.Log(message, true);
            Environment.Exit(0);
        }

        public static string GetHWID()
        {
            var mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
            ManagementObjectCollection mbsList = mbs.Get();
            foreach (ManagementObject mo in mbsList) return mo["ProcessorId"].ToString();
            return null;
        }
    }

    internal class res
    {
    }

    static class License2
    {
        public static string Check()
        {
            try
            {
                if (!File.Exists("license_mines.txt")) return "Missing license";
                var dt = File.ReadAllText("license_mines.txt");
                var serial = long.Parse(new String(Encoding.ASCII.GetString(Convert.FromBase64String(dt).Reverse().ToArray()).Replace(" ", "").Where(x => Char.IsDigit(x)).ToArray()));
                if (new WebClient().DownloadString("https://hostyouenterprise.com/license/mines/?id=" + License.GetHWID() + "&l=" + dt) != "ok") return "Invalid license";
                if (serial < Program.time()) return "License expired";
                return "Valid";
            }
            catch { return "Invalid license"; }
        }
    }

}