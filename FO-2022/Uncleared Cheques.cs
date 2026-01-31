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
    public partial class Uncleared_Cheques : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Uncleared_Cheques()
        {
            InitializeComponent();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Uncleared_Cheques_Load(object sender, EventArgs e)
        {
            // Form load - nothing special needed
        }

        private DataTable GetUnclearedChequesData()
        {
            DataTable dt = new DataTable();

            try
            {
                string query = @"
                -- UNCLEARED CHEQUES (Issued Cheques not reconciled)
                SELECT 
                    bc.ChequeNo,
                    bc.ChequeDate,
                    bc.VoucherDate,
                    bc.Amount,
                    bc.BankAccountNo,
                    bcoa.AccountName AS BankName,
                    bc.AccountNo,
                    pcoa.AccountName AS PartyName,
                    bc.Narration,
                    bc.ReconcileDate,
                    bc.IsLost,
                    DATEDIFF(DAY, bc.ChequeDate, GETDATE()) AS DaysOutstanding,
                    CASE 
                        WHEN bc.ReconcileDate IS NULL THEN 'Uncleared'
                        ELSE 'Cleared'
                    END AS Status
                FROM [dbo].[BankCheques] bc
                LEFT JOIN [dbo].[ChartOfAccounts] bcoa ON bc.BankAccountNo = bcoa.AccountNo
                LEFT JOIN [dbo].[ChartOfAccounts] pcoa ON bc.AccountNo = pcoa.AccountNo
                WHERE bc.ReconcileDate IS NULL 
                    AND bc.IsPosted = 1
                    AND bc.IsLost = 0
                
                UNION ALL
                
                -- UNCLEARED DEPOSIT CHEQUES (Cheques deposited but not reconciled)
                SELECT 
                    bd.ChequeNo,
                    bd.ChequeDate,
                    bd.VoucherDate,
                    bd.Amount,
                    bd.BankAccountNo,
                    bcoa.AccountName AS BankName,
                    bd.AccountNo,
                    pcoa.AccountName AS PartyName,
                    bd.Narration,
                    bd.ReconcileDate,
                    0 AS IsLost,
                    DATEDIFF(DAY, bd.ChequeDate, GETDATE()) AS DaysOutstanding,
                    CASE 
                        WHEN bd.ReconcileDate IS NULL THEN 'Uncleared'
                        ELSE 'Cleared'
                    END AS Status
                FROM [dbo].[BankDeposit] bd
                LEFT JOIN [dbo].[ChartOfAccounts] bcoa ON bd.BankAccountNo = bcoa.AccountNo
                LEFT JOIN [dbo].[ChartOfAccounts] pcoa ON bd.AccountNo = pcoa.AccountNo
                WHERE bd.IsCheque = 1 
                    AND bd.ReconcileDate IS NULL 
                    AND bd.IsPosted = 1
                    AND bd.IsBounced = 0
                
                ORDER BY ChequeDate, BankAccountNo";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching uncleared cheques: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private void ShowReport(bool isPrintMode = false)
        {
            try
            {
                // Get data
                DataTable dt = GetUnclearedChequesData();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No uncleared cheques found", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare data sources
                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", dt);

                // Prepare parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                //parameters.Add("ReportTitle", "Uncleared Cheques Report");
                //parameters.Add("GeneratedDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                //parameters.Add("TotalUncleared", dt.Rows.Count.ToString());

                // Calculate total amount
                decimal totalAmount = 0;
                foreach (DataRow row in dt.Rows)
                {
                    totalAmount += Convert.ToDecimal(row["Amount"]);
                }
                parameters.Add("TotalAmount", totalAmount.ToString("N2"));

                // Create report viewer instance
                Reports.Report_viewer reportViewer = new Reports.Report_viewer();
                reportViewer.ShowReport("Uncleared_Cheques", dataSources );

                // Print mode specific logic (if needed)
                if (isPrintMode)
                {
                    // Add print-specific logic here
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Report Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            ShowReport(false); // Preview mode
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            ShowReport(true); // Print mode
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}