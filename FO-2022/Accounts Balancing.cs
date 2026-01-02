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
    public partial class Accounts_Balancing : Form
    {
        public Accounts_Balancing()
        {
            InitializeComponent();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
        }

        private void accountcombobox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void previewbtn_Click(object sender, EventArgs e)
        {

        }

        private void prntbtn_Click(object sender, EventArgs e)
        {

        }

        private void excludeaccountshavingzerobalancecheckbox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Accounts_Balancing_Load(object sender, EventArgs e)
        {

        }
    }
}
