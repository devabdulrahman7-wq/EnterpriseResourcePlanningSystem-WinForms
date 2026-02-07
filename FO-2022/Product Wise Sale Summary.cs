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
    public partial class Product_Wise_Sale_Summary : Form
    {
        public Product_Wise_Sale_Summary()
        {
            InitializeComponent();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void fromsaletxtbox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
