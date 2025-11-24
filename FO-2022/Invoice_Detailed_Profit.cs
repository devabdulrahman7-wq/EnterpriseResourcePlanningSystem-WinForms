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
    public partial class Invoice_Detailed_Profit : Form
    {
        public Invoice_Detailed_Profit()
        {
            InitializeComponent();
        }
        private void Invoice_Detailed_Profit_Load(object sender, EventArgs e)
        {

        }
        private void closebtn_Click(object sender, EventArgs e)
        {
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
        }
    }
}
