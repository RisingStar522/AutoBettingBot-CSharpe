using BustabitCrash.Games;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace BustabitCrash.Forms
{
    public partial class CrashForm : Form
    {
        public CrashForm(string title)
        {
            InitializeComponent();
            this.Text = title + " Settings";
            this.FormClosing += (o, e) => { Core.OpenPicker(); };
        }

        bool running = false;
        public void button1_Click(object sender, EventArgs e)
        {
            if (running)
            {
                
                running = false;
                if (this.Text.Contains("Crash"))
                {
                    if (button1.InvokeRequired)
                    {
                        button1.Invoke((MethodInvoker)delegate ()
                        {
                            this.button1.Text = "Start(&S)";
                            this.button1.Enabled = false;
                            this.label7.Text = "1min delay";
                        });
                    }
                    new Thread(() =>
                    {
                        Thread.Sleep(1000 * 20);
                        this.Invoke((MethodInvoker)delegate ()
                        {
                            button1.Text = "Start(&S)";
                            button1.Enabled = true;
                            this.label7.Text = "Ready";
                            this.label7.ForeColor = Color.Black;
                        });
                    }).Start();

                }
                if (label7.InvokeRequired)
                {
                    label7.Invoke((MethodInvoker)delegate ()
                    {
                        label7.ForeColor = Color.Red;
                        label7.Text = "Stopped";
                    });
                }
                if (sender != null) Crash.Stop();
            }
            else
            {
                running = true;
                profits = 0;
                button1.Text = "Stop(&P)";
                label7.ForeColor = Color.Gold;
                label7.Text = "Running";
                try
                {
                    Crash.Start(double.Parse(textBox1.Text), double.Parse(textBox2.Text), double.Parse(textBox3.Text), double.Parse(profit_value.Text), double.Parse(losses_value.Text), checkBox2.Checked, numup.Value, gameHistory_chk.Checked, chatHistory_chk.Checked, bitRadiio.Checked);
                }
                catch
                {
                    Core.Log("Invalid settings");
                    MessageBox.Show("Invalid settings", "Bustabit", MessageBoxButtons.OK);
                    //button1_Click(sender, e);
                    running = false;
                    profits = 0;
                    button1.Text = "Start(&S)";
                    label7.ForeColor = Color.Red;
                    label7.Text = "Stopped";
                }
            }
        }

        public double profits = 0;
        public void UpdateProfits(double val)
        {
            profits += val;
            this.Invoke((MethodInvoker)delegate ()
            {
                label5.ForeColor = profits >= 0 ? Color.LimeGreen : Color.Red;
                label5.Text = profits.ToString("0.00000000");
            });
        }

        private void CrashForm_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to quit this program? All game data is automatically saved before exiting this program.", "Bustabit",
                MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                this.Close();
            }
        }

    }
}
