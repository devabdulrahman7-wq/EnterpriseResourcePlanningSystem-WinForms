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
    public partial class Account_Balances : Form
    {
        public Account_Balances()
        {
            InitializeComponent();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            Mainform1 mf= new Mainform1();
            mf.Show();
            this.Hide();
        }

        private void Account_Balances_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void previewbtn_Click(object sender, EventArgs e)
        {

        }

        private void printbtn_Click(object sender, EventArgs e)
        {

        }
    }
}
