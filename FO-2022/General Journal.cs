using System;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class General_Journal : Form
    {
        public General_Journal()
        {
            InitializeComponent();
        }

        private void General_Journal_Load(object sender, EventArgs e)
        {
            // Set default dates
            fromdatepcker.Value = DateTime.Today.AddDays(-30);
            todatepcker.Value = DateTime.Today;
            fromvouchernotxtbox.Text = "0";
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();

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
            try
            {
                // Get input values
                DateTime fromDate = fromdatepcker.Value;
                DateTime toDate = todatepcker.Value;

                if (fromDate > toDate)
                {
                    MessageBox.Show("From Date cannot be greater than To Date.", "Invalid Date");
                    return;
                }

                int fromVoucher = 0;
                int toVoucher = 0;

                // Parse voucher numbers
                if (!string.IsNullOrEmpty(fromvouchernotxtbox.Text))
                    int.TryParse(fromvouchernotxtbox.Text, out fromVoucher);

                if (!string.IsNullOrEmpty(tovouchernotxtbox.Text))
                    int.TryParse(tovouchernotxtbox.Text, out toVoucher);
                else
                    toVoucher = int.MaxValue; // If empty, show all

                // Open report viewer
                Reports.General_Journal_rv reportForm = new Reports.General_Journal_rv();
                reportForm.FromDate = fromDate;
                reportForm.ToDate = toDate;
                reportForm.FromVoucherNo = fromVoucher;
                reportForm.ToVoucherNo = toVoucher;
                reportForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error");
            }
        }

        private void fromdatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Optional validation
        }

        private void todatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Optional validation
        }

        private void fromvouchernotxtbox_TextChanged(object sender, EventArgs e)
        {
            // Allow only numbers
            if (!string.IsNullOrEmpty(fromvouchernotxtbox.Text) &&
                !int.TryParse(fromvouchernotxtbox.Text, out _))
            {
                MessageBox.Show("Please enter numbers only.", "Invalid Input");
                fromvouchernotxtbox.Text = "";
            }
        }

        private void tovouchernotxtbox_TextChanged(object sender, EventArgs e)
        {
            // Allow only numbers
            if (!string.IsNullOrEmpty(tovouchernotxtbox.Text) &&
                !int.TryParse(tovouchernotxtbox.Text, out _))
            {
                MessageBox.Show("Please enter numbers only.", "Invalid Input");
                tovouchernotxtbox.Text = "";
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
//using System;
//using System.Windows.Forms;

//namespace FO_2022
//{
//    public partial class General_Journal : Form
//    {
//        public General_Journal()
//        {
//            InitializeComponent();
//        }

//        private void General_Journal_Load(object sender, EventArgs e)
//        {
//            fromdatepcker.Value = DateTime.Today.AddDays(-30);
//            todatepcker.Value = DateTime.Today;
//            fromvouchernotxtbox.Text = "0";
//        }

//        private void closebtn_Click(object sender, EventArgs e) => GoToMain();

//        private void pictureBox4_Click(object sender, EventArgs e) => GoToMain();

//        private void GoToMain()
//        {
//            new Mainform1().Show();
//            this.Hide();
//        }

//        private void previewbtn_Click(object sender, EventArgs e)
//            => OpenReport();
//        private void printbtn_Click(object sender, EventArgs e)
//            => OpenReport();

//        private void OpenReport()
//        {
//            if (fromdatepcker.Value > todatepcker.Value)
//            {
//                MessageBox.Show("From Date cannot be greater than To Date.");
//                return;
//            }

//            var reportForm = new Reports.General_Journal_rv
//            {
//                FromDate = fromdatepcker.Value,
//                ToDate = todatepcker.Value,
//                FromVoucherNo = GetInt(fromvouchernotxtbox.Text),
//                ToVoucherNo = GetInt(tovouchernotxtbox.Text, int.MaxValue)
//            };
//            reportForm.Show();
//        }

//        private int GetInt(string text, int defaultValue = 0)
//        {
//            return int.TryParse(text, out int result) ? result : defaultValue;
//        }

//        private void fromvouchernotxtbox_TextChanged(object sender, EventArgs e)
//        {
//            ValidateNumberInput(fromvouchernotxtbox);
//        }

//        private void tovouchernotxtbox_TextChanged(object sender, EventArgs e)
//        {
//            ValidateNumberInput(tovouchernotxtbox);
//        }

//        private void ValidateNumberInput(TextBox textBox)
//        {
//            if (!string.IsNullOrEmpty(textBox.Text) && !int.TryParse(textBox.Text, out _))
//            {
//                MessageBox.Show("Numbers only.");
//                textBox.Text = "";
//            }
//        }
//    }
//}