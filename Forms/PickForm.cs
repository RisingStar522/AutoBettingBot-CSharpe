using System;
using System.Windows.Forms;

namespace BustabitCrash.Forms
{
    public partial class PickForm : Form
    {
        public PickForm()
        {
            InitializeComponent();
            Core.Log("Now pick your game mode");
            this.FormClosing += (o, e) =>
            {
                try { Program.bot.Quit(); } catch { }

                /* CLEANUP CHROMEDRIVER PROCESS */
                System.Diagnostics.Process.Start("cmd.exe", "/c taskkill /F /IM chromedriver.exe /T");

                Environment.Exit(0);
            };
        }

        private void PickForm_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Stake.ApplyGame(Program.bot);
        }

        private void pictureBox1_MouseHover(object sender, System.EventArgs e)
        {
            this.pictureBox1.BackgroundImage = global::BustabitCrash.Properties.Resources.playbtn2;
        }

        private void pictureBox1_MouseLeave(object sender, System.EventArgs e)
        {
            this.pictureBox1.BackgroundImage = global::BustabitCrash.Properties.Resources.playbtn1;
        }

    }
}
