using System;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            progressBar1.Style = ProgressBarStyle.Continuous;

            timer1.Start();   // Start progress automatically
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value < progressBar1.Maximum)
            {
                progressBar1.Value += 2;  // filling speed
            }
            else
            {
                timer1.Stop();  // stop when full
                SignUp f = new SignUp();
                f.Show();
                this.Hide();
            }
        }
    }
}
