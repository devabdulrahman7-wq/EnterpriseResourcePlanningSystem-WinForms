//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using FO_2022.Reports;  // Add this if the report form is in Reports folder

//namespace FO_2022
//{
//    public partial class Customer_Wise_Profits : Form
//    {
//        public Customer_Wise_Profits()
//        {
//            InitializeComponent();  // Uncomment this line
//        }

//        private void pictureBox4_Click(object sender, EventArgs e)
//        {
//            Mainform1 mf = new Mainform1();
//            mf.Show();
//            this.Hide();
//        }

//        private void closebtn_Click(object sender, EventArgs e)
//        {
//            Mainform1 mf = new Mainform1();
//            mf.Show();
//            this.Hide();
//        }

//        // Preview button: open report viewer form
//        private void previewbtn_Click(object sender, EventArgs e)
//        {
//            DateTime fromDate = fromdatepcker.Value;
//            DateTime toDate = todatepcker.Value;

//            if (fromDate > toDate)
//            {
//                MessageBox.Show("From Date cannot be greater than To Date.", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                return;
//            }

//            // Open report viewer form for preview
//            Customer_Wise_Profits_rv reportViewerForm = new Customer_Wise_Profits_rv();
//            reportViewerForm.SetParameters(fromDate, toDate);
//            reportViewerForm.Show();  // Show the report viewer form
//        }

//        // Print button: open report viewer and print directly
//        private void printbtn_Click(object sender, EventArgs e)
//        {
//            //DateTime fromDate = fromdatepcker.Value;
//            //DateTime toDate = todatepcker.Value;

//            //if (fromDate > toDate)
//            //{
//            //    MessageBox.Show("From Date cannot be greater than To Date.", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//            //    return;
//            //}

//            // Open report viewer form and print
//            Customer_Wise_Profits_rv reportViewerForm = new Customer_Wise_Profits_rv();
//            //reportViewerForm.SetParameters(fromDate, toDate);
//            reportViewerForm.Show();  // Optional: show form before printing
//            reportViewerForm.PrintReport();  // Call print method
//        }

//        private void fromdatepcker_ValueChanged(object sender, EventArgs e)
//        {
//            // Optional: add validation logic here
//        }

//        private void todatepcker_ValueChanged(object sender, EventArgs e)
//        {
//            // Optional: add validation logic here
//        }

//        private void Customer_Wise_Profits_Load(object sender, EventArgs e)
//        {

//        }
//    }
//}using System;
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
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
        }

        // Preview button: open report viewer form
        private void previewbtn_Click(object sender, EventArgs e)
        {
            DateTime fromDate = fromdatepcker.Value;
            DateTime toDate = todatepcker.Value;

            if (fromDate > toDate)
            {
                MessageBox.Show("From Date cannot be greater than To Date.", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Open report viewer form for preview
            Reports.Customer_Wise_Profits_rv reportViewerForm = new Reports.Customer_Wise_Profits_rv();
            reportViewerForm.SetParameters(fromDate, toDate);
            reportViewerForm.Show();  // Show the report viewer form
        }

        // Print button: open report viewer and print directly
        private void printbtn_Click(object sender, EventArgs e)
        {
            DateTime fromDate = fromdatepcker.Value;
            DateTime toDate = todatepcker.Value;

            if (fromDate > toDate)
            {
                MessageBox.Show("From Date cannot be greater than To Date.", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Open report viewer form and print
            Reports.Customer_Wise_Profits_rv reportViewerForm = new Reports.Customer_Wise_Profits_rv();
            reportViewerForm.SetParameters(fromDate, toDate);
            reportViewerForm.Show();  // Optional: show form before printing
            reportViewerForm.PrintReport();  // Call print method
        }

        private void fromdatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Optional: add validation logic here
        }

        private void todatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Optional: add validation logic here
        }

        private void Customer_Wise_Profits_Load(object sender, EventArgs e)
        {
            // Set default dates
            fromdatepcker.Value = DateTime.Today.AddDays(-30);
            todatepcker.Value = DateTime.Today;
        }
    }
}