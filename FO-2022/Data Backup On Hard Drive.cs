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
    public partial class Data_Backup_On_Hard_Drive : Form
    {
        public Data_Backup_On_Hard_Drive()
        {
            InitializeComponent();
        }

        private void Data_Backup_On_Hard_Drive_Load(object sender, EventArgs e)
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
