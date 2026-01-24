using System;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Customer_Wise_Profits : Form
    {
        public Customer_Wise_Profits()
        {
            InitializeComponent();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            //Mainform1 mf = new Mainform1();
            //mf.Show();
            //this.Hide();
            this.Close();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            OpenReportViewer();
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            OpenReportViewer(); // Same as preview
        }

        private void OpenReportViewer()
        {
            DateTime fromDate = fromdatepcker.Value;
            DateTime toDate = todatepcker.Value;

            if (fromDate > toDate)
            {
                MessageBox.Show("From Date cannot be greater than To Date.", "Invalid Date",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Open report viewer form - Use EITHER Properties OR Method
            Reports.Customer_Wise_Profits_rv reportForm = new Reports.Customer_Wise_Profits_rv();

            // Option 1: Using Properties (if you added them)
            reportForm.FromDate = fromDate;
            reportForm.ToDate = toDate;

            // Option 2: Using SetParameters method (if you prefer)
            // reportForm.SetParameters(fromDate, toDate);

            reportForm.Show();
        }

        private void Customer_Wise_Profits_Load(object sender, EventArgs e)
        {
            // Set default dates
            fromdatepcker.Value = DateTime.Today.AddDays(-30);
            todatepcker.Value = DateTime.Today;
        }

        private void fromdatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Optional
        }

        private void todatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Optional
        }
    }
}