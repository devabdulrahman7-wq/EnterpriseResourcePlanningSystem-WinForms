using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Company_Wise_Profits : Form
    {
        public Company_Wise_Profits()
        {
            
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
        }

        private void Company_Wise_Profits_Load(object sender, EventArgs e)
        {

        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {

        }

        private void printbtn_Click(object sender, EventArgs e)
        {

        }

        private void fromdatepcker_ValueChanged(object sender, EventArgs e)
        {

        }

        private void todatepcker_ValueChanged(object sender, EventArgs e)
        {

        }

        //private void InitializeComponent()
        //{
        //    this.SuspendLayout();
        //    // 
        //    // Company_Wise_Profits
        //    // 
        //    this.ClientSize = new System.Drawing.Size(429, 261);
        //    this.Name = "Company_Wise_Profits";
        //    this.ResumeLayout(false);

        //}
    }
}

