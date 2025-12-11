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
    public partial class Customers : Form
    {
        public Customers()
        {
            InitializeComponent();
        }

        private void Customers_Load(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
        }

        private void sectoramecombobox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void sectornamecombobox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void nametxtbox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
