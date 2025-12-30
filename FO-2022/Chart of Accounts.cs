//using System.Drawing.Printing;  
//using System.Drawing.Imaging;  
//using System.IO;                
using Microsoft.Reporting.WinForms;  
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace FO_2022
{
    public partial class Chart_of_Accounts : Form
    {
        //private string connectionString = "Data Source=YOUR_SERVER;Initial Catalog=UltraBiz20232;Integrated Security=True"
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


        public Chart_of_Accounts()
        {
            InitializeComponent();
            LoadAccountTypes();
        }

        private void LoadAccountTypes()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT DISTINCT AccountType FROM ChartOfAccounts ORDER BY AccountType";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    accountcombobox.Items.Clear();
                    accountcombobox.Items.Add("All Accounts");

                    while (reader.Read())
                    {
                        accountcombobox.Items.Add(reader["AccountType"].ToString());
                    }

                    reader.Close();

                    // Default selection
                    accountcombobox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading account types: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            if (accountcombobox.SelectedItem == null)
            {
                MessageBox.Show("Please select an account type first.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedAccountType = accountcombobox.SelectedItem.ToString();

            // Pass selected account type to report form
            Reports.Charts_of_Accounts_rv reportForm = new Reports.Charts_of_Accounts_rv();
            reportForm.SelectedAccountType = selectedAccountType;
            reportForm.Show();
        }

        private void prntbtn_Click(object sender, EventArgs e)
        {
            if (accountcombobox.SelectedItem == null)
            {
                MessageBox.Show("Please select an account type first.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedAccountType = accountcombobox.SelectedItem.ToString();

            // Simply open the report viewer
            Reports.Charts_of_Accounts_rv reportForm = new Reports.Charts_of_Accounts_rv();
            reportForm.SelectedAccountType = selectedAccountType;
            reportForm.Show();

            // User can then use the PRINT button in ReportViewer toolbar
        }

        private DataSet GetChartOfAccountsData(string accountType)
        {
            DataSet ds = new DataSet();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query;

                    if (accountType == "All Accounts")
                    {
                        query = @"SELECT AccountNo, AccountName, AccountType, Narration, 
                                 OpeningDebit, OpeningCredit 
                                 FROM ChartOfAccounts 
                                 ORDER BY AccountNo";
                    }
                    else
                    {
                        query = @"SELECT AccountNo, AccountName, AccountType, Narration, 
                                 OpeningDebit, OpeningCredit 
                                 FROM ChartOfAccounts 
                                 WHERE AccountType = @AccountType 
                                 ORDER BY AccountNo";
                    }

                    SqlCommand cmd = new SqlCommand(query, conn);

                    if (accountType != "All Accounts")
                    {
                        cmd.Parameters.AddWithValue("@AccountType", accountType);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds, "ChartOfAccounts");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching data: " + ex.Message);
            }

            return ds;
        }

        //private void PrintReport(Microsoft.Reporting.WinForms.LocalReport report)
        //{
        //    try
        //    {
        //        string deviceInfo =
        //        @"<DeviceInfo>
        //            <OutputFormat>EMF</OutputFormat>
        //            <PageWidth>8.5in</PageWidth>
        //            <PageHeight>11in</PageHeight>
        //            <MarginTop>0.25in</MarginTop>
        //            <MarginLeft>0.25in</MarginLeft>
        //            <MarginRight>0.25in</MarginRight>
        //            <MarginBottom>0.25in</MarginBottom>
        //        </DeviceInfo>";

        //        Warning[] warnings;
        //        string[] streams;
        //        byte[] renderedBytes;

        //        // Render the report
        //        renderedBytes = report.Render(
        //            "Image",
        //            deviceInfo,
        //            out string mimeType,
        //            out string encoding,
        //            out string fileNameExtension,
        //            out streams,
        //            out warnings);

        //        // Create a print dialog
        //        PrintDialog printDialog = new PrintDialog();
        //        if (printDialog.ShowDialog() == DialogResult.OK)
        //        {
        //            // Print the document
        //            System.Drawing.Printing.PrintDocument printDoc = new System.Drawing.Printing.PrintDocument();
        //            printDoc.PrinterSettings = printDialog.PrinterSettings;

        //            printDoc.PrintPage += (sender, e) =>
        //            {
        //                using (System.Drawing.Imaging.Metafile metafile = new System.Drawing.Imaging.Metafile(
        //                    new System.IO.MemoryStream(renderedBytes)))
        //                {
        //                    e.Graphics.DrawImage(metafile, e.PageBounds);
        //                }
        //                e.HasMorePages = false;
        //            };

        //            printDoc.Print();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Printing error: " + ex.Message, "Error",
        //            MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
            Mainform1 mainform1 = new Mainform1();
            mainform1.Show();
        }

        private void accountcombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // You can add code here if you want to do something when selection changes
        }
    }
}