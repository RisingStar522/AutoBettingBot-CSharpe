using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using BustabitCrash.Forms;
using System;
using System.Threading;

namespace BustabitCrash.Games
{
    class Crash
    {
        public static ChromeDriver driver;
        public static CrashForm form;
        public static bool running, save_logs, is_crash, game_history, chat_history, bitsRadio, increase_profit;
        public static double base_amount, initProfits, max_bet, probit_value, losses_value;
        public static Decimal numup;
        static int on_loss, skipping, losses;
        static double stop_at;
        static int[] skip_after;
        static int[][] skip_range;
        static double amount;


        public static void Open(ChromeDriver driver, bool is_crash = false)
        {
            Crash.is_crash = is_crash;
            Crash.driver = driver;
            form = new CrashForm("Bustabit-Crash");
            form.Show();
        }

        public static void Start(double max_bet, double initProfits, double base_bet, double probit_value, double losses_value, bool save_logs, Decimal numup, bool game_history, bool chat_history, bool bitsRadio, bool increase_profit)
        {
            Crash.base_amount = Crash.amount = (base_bet);
            Crash.max_bet = max_bet;
            Crash.initProfits = initProfits;
            Crash.save_logs = save_logs;
            Crash.probit_value = probit_value;
            Crash.losses_value = losses_value;
            Crash.game_history = game_history;
            Crash.chat_history = chat_history;
            Crash.bitsRadio = bitsRadio;
            Crash.numup = numup;
            Crash.increase_profit = increase_profit;
            skipping = losses = 0;
            StartMethod();
        }

        public static void StartMethod()
        {
            Core.Log("Starting Crash 1.0.1 "+ increase_profit +" game", save_logs);
            running = true;
            on_loss = Convert.ToInt32(numup);
            amount = base_amount;
            stop_at = initProfits;
            Skip_after = numup == 1 ? new[] { 13 } : new[] { 5 };
            skip_range = numup == 1 ? new int[][] { new[] { 15, 25 } } : new int[][] { new[] { 15, 34 } };
            //new Thread(() =>
            //{
                while (running)
                {
                    if (skipping > 0)
                    {
                        DoBet(0);
                        skipping--;
                        Core.Log("Skipping game, remaining " + skipping, save_logs);
                    }
                    else
                    {
                        var game_stop_at = numup > 1 && losses > 4 ? stop_at : stop_at; //StopAt_random(stop_at);
                        SetPayout(game_stop_at.ToString("0.00"));
                        if (Stake.GetBalance(driver) < amount)
                        {
                            Core.Log("Not enough balance ", save_logs);
                            
                            Stop();
                            form.button1_Click(null, null);
                            break;
                        }
                        var bet = DoBet(amount);
                        if ( is_crash ) // crash 1.3
                        {
                            Core.Log("Result: " + game_stop_at + " | Bet: " + amount.ToString("0.00"), save_logs);
                            if (bet < 0)
                            {
                                losses++;
                                Core.Log("Losses: " + losses, save_logs);
                                form.UpdateProfits(-amount);
                                amount *= on_loss;
                                if (amount > max_bet)
                                {
                                    Core.Log("Max bet reached", save_logs);
                                    Stop();
                                    form.button1_Click(null, null);
                                }
                                form.UpdateState("Stoped by Profit Breaker");
                        }
                            else
                            {
                                losses = 0;
                                var bet_profits = bet - amount;
                                Core.Log("Won: " + bet.ToString("0.00000000"), save_logs);
                                form.UpdateProfits(bet);
                                amount = base_amount;
                                if (form.profits >= probit_value)
                                {
                                    Core.Log("Max profits reached", save_logs);
                                    form.UpdateState("Stoped by Profit Breaker");
                                    Stop();
                                    form.button1_Click(null, null);
                                }
                            }
                        }
                    }
                }
            //}).Start();
        }

        static double last_amount = 0;

        public static double StopAt_random(double rand_val)
        {
            double generated;
            Random RandGenerator = new Random();
            generated = 0;
            generated = RandGenerator.NextDouble() + rand_val;
            return generated;
        }

        public static int[] Skip_after { get => Skip_after1; set => Skip_after1 = value; }
        public static int[] Skip_after1 { get => skip_after; set => skip_after = value; }

        public static double DoBet(double amount)
        {
            if (amount != last_amount)
            {
                SetBet(amount);
                last_amount = amount;
            }
            
            Core.WaitAction(() =>
            {
                driver.FindElementByXPath("//*[@id='root']/div/div/div[5]/div/div[2]/form/button").Click();
            });
            double balance = 0;

            var js = (IJavaScriptExecutor)driver;
            string cs = "x";
            while (cs != "BETTING")
            {
                try
                {
                    Thread.Sleep(100);
                    cs = (string)(js.ExecuteScript(Core.XPJS + "return getElementByXpath(\"//*[@id='root']/div/div/div[5]/div/div[2]/form/button\").innerText;"));
                    cs =cs.Trim();
                }
                catch { }
            }
            while (cs == "BETTING")
            {
                try
                {
                    Thread.Sleep(100);
                    cs = (string)(js.ExecuteScript(Core.XPJS + "return getElementByXpath(\"//*[@id='root']/div/div/div[5]/div/div[2]/form/button\").innerText;"));
                    cs=cs.Trim();
                }
                catch { }
            }
            double bc = Stake.GetBalance(driver);
            double bce = Stake.GetBalance(driver);
            while (bc == bce)
            {
                try
                {
                    bce = Stake.GetBalance(driver);
                    string st = (string)(js.ExecuteScript("var elem = document.getElementsByTagName('div');for (var i = 0; i < elem.length; i++) if (elem[i].getAttribute('class') && elem[i].getAttribute('class').indexOf('ProgressBar') > -1) return elem[i].parentNode.getElementsByTagName('span')[0].innerText;"));
                    if (st.IndexOf("BET") > -1)
                    {
                        return balance = -amount;
                    }
                }
                catch { }

            }
            balance = bce - bc;

            return balance;
        }

        public static void Stop()
        {
            Core.Log("Stopping " + "crash" + " game", save_logs);
            running = false;
        }

        public static void SetPayout(string payout)
        {
            Core.WaitAction(() =>
            {
                driver.FindElementByName("payout").Clear();
                driver.FindElementByName("payout").SendKeys(payout);                
            });
        }

        public static void SetBet(double amount)
        {
            Core.WaitAction(() =>
            {
                driver.FindElementByName("wager").Clear();
                driver.FindElementByName("wager").SendKeys(amount.ToString());
            });
        }
    }
}
