using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FO_2022.Reports
{
    public partial class Salesman_Sector : Form
    {
        public Salesman_Sector()
        {
            InitializeComponent();
        }

        private void Salesman_Sector_Load(object sender, EventArgs e)
        {

            this.reportViewer1.RefreshReport();
        }
    }
}
