using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Day_Book : Form
    {
        //string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Day_Book()
        {
            InitializeComponent();
            // Default date selection (Last 7 days)
            dateTimePicker1.Value = DateTime.Today.AddDays(-7);
            dateTimePicker2.Value = DateTime.Today;
        }

        // Close icon
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Back to main form
        private void closebtn_Click(object sender, EventArgs e)
        {
            //Mainform1 mf = new Mainform1();
            //mf.Show();
            this.Close();
        }

        // Preview Button
        private void btnPreview_Click(object sender, EventArgs e)
        {
            DateTime fromDate = dateTimePicker1.Value.Date;
            DateTime toDate = dateTimePicker2.Value.Date;

            // Validation
            if (fromDate > toDate)
            {
                MessageBox.Show("From Date cannot be greater than To Date", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open Report Form
            Daybook_report rpt = new Daybook_report(fromDate, toDate);
            rpt.ShowDialog();

        }



        private void printbtn_Click(object sender, EventArgs e)
        {
            DateTime fromDate = dateTimePicker1.Value.Date;
            DateTime toDate = dateTimePicker2.Value.Date;

            // Validation
            if (fromDate > toDate)
            {
                MessageBox.Show("From Date cannot be greater than To Date", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open Report Form
            Daybook_report rpt = new Daybook_report(fromDate, toDate);
            rpt.ShowDialog();
        }
    }
}
