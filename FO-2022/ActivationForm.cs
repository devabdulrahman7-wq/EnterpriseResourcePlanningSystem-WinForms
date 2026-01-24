using Delloweb.Licensing;
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
    public partial class ActivationForm : Form
    {
        public ActivationForm()
        {
            InitializeComponent();
        }

        private void btnActivate_Click(object sender, EventArgs e)
        {
            if (LicenseEngine.ValidateLicense(txtLicenseKey.Text.Trim(), out int year))
            {
                LicenseEngine.SaveLicense(year);
                MessageBox.Show("Activated for year " + year);
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid License Key");
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
