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
    public partial class Daily_Business_Activity : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Daily_Business_Activity()
        {
            InitializeComponent();
            SetDefaultDates();
        }

        private void SetDefaultDates()
        {
            fromdatepcker.Value = new DateTime(2023,01,01);
            todatepcker.Value =new DateTime(2023,02,02);
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Daily_Business_Activity_Load(object sender, EventArgs e)
        {
            // Load any additional data if needed
        }

        private void fromdatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Optional: Add any validation logic
        }

        private void todatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Optional: Add any validation logic
        }

        private DataTable GetDailyBusinessActivityData()
        {
            DataTable dt = new DataTable();

            try
            {
                string query = @"
                -- DAILY BUSINESS ACTIVITY REPORT
                -- Shows all business transactions for selected date range
                
                -- 1. SALES ACTIVITY
                SELECT 
                    'SALE' AS ActivityType,
                    s.SaleDate AS TransactionDate,
                    s.BillNo AS ReferenceNo,
                    p.PartyName AS PartyName,
                    'Customer' AS PartyType,
                    'INVOICE' AS TransactionType,
                    SUM(sb.TTLValue + sb.TTLSTax + sb.TTLFTax) AS Amount,
                    'DEBIT' AS EntryType,
                    s.SalesmanID AS Salesman,
                    s.Narration AS Remarks
                FROM [dbo].[Sales] s
                INNER JOIN [dbo].[SalesBody] sb ON s.SaleId = sb.SaleId
                INNER JOIN [dbo].[Parties] p ON s.CustomerId = p.PartyID
                WHERE s.SaleDate BETWEEN @FromDate AND @ToDate
                    AND s.IsPosted = 1
                GROUP BY s.SaleId, s.SaleDate, s.BillNo, p.PartyName, s.SalesmanID, s.Narration
                
                UNION ALL
                
                -- 2. PURCHASE ACTIVITY
                SELECT 
                    'PURCHASE' AS ActivityType,
                    pur.PurchaseDate AS TransactionDate,
                    pur.BillNo AS ReferenceNo,
                    p.PartyName AS PartyName,
                    'Vendor' AS PartyType,
                    'BILL' AS TransactionType,
                    SUM(pb.TTLValue + pb.TTLSTax + pb.TTLFTax) AS Amount,
                    'CREDIT' AS EntryType,
                    NULL AS Salesman,
                    pur.Narration AS Remarks
                FROM [dbo].[Purchase] pur
                INNER JOIN [dbo].[PurchasesBody] pb ON pur.PurchaseID = pb.PurchaseId
                INNER JOIN [dbo].[Parties] p ON pur.VendorID = p.PartyID
                WHERE pur.PurchaseDate BETWEEN @FromDate AND @ToDate
                GROUP BY pur.PurchaseID, pur.PurchaseDate, pur.BillNo, p.PartyName, pur.Narration
                
                UNION ALL
                
                -- 3. SALES RETURNS
                SELECT 
                    'SALE RETURN' AS ActivityType,
                    sr.ReturnDate AS TransactionDate,
                    sr.BillNo AS ReferenceNo,
                    p.PartyName AS PartyName,
                    'Customer' AS PartyType,
                    'CREDIT NOTE' AS TransactionType,
                    SUM(srb.TTLValue + srb.TTLSTax + srb.TTLFTax) AS Amount,
                    'CREDIT' AS EntryType,
                    sr.SalesmanId AS Salesman,
                    sr.Narration AS Remarks
                FROM [dbo].[SaleReturns] sr
                INNER JOIN [dbo].[SaleReturnsBody] srb ON sr.ReturnId = srb.ReturnId
                INNER JOIN [dbo].[Parties] p ON sr.CustomerId = p.PartyID
                WHERE sr.ReturnDate BETWEEN @FromDate AND @ToDate
                    AND sr.IsPosted = 1
                GROUP BY sr.ReturnId, sr.ReturnDate, sr.BillNo, p.PartyName, sr.SalesmanId, sr.Narration
                
                UNION ALL
                
                -- 4. PURCHASE RETURNS
                SELECT 
                    'PURCHASE RETURN' AS ActivityType,
                    pr.ReturnDate AS TransactionDate,
                    pr.BillNo AS ReferenceNo,
                    p.PartyName AS PartyName,
                    'Vendor' AS PartyType,
                    'DEBIT NOTE' AS TransactionType,
                    SUM(prb.TTLValue + prb.TTLSTax + prb.TTLFTax) AS Amount,
                    'DEBIT' AS EntryType,
                    NULL AS Salesman,
                    pr.Narration AS Remarks
                FROM [dbo].[PurReturns] pr
                INNER JOIN [dbo].[PurReturnsBody] prb ON pr.PurReturnId = prb.PurReturnId
                INNER JOIN [dbo].[Parties] p ON pr.VendorId = p.PartyID
                WHERE pr.ReturnDate BETWEEN @FromDate AND @ToDate
                    AND pr.IsPosted = 1
                GROUP BY pr.PurReturnId, pr.ReturnDate, pr.BillNo, p.PartyName, pr.Narration
                
                UNION ALL
                
                -- 5. RECEIPTS (CASH/BANK)
                SELECT 
                    'RECEIPT' AS ActivityType,
                    av.VoucherDate AS TransactionDate,
                    CAST(av.VoucherNo AS VARCHAR(20)) AS ReferenceNo,
                    coa.AccountName AS PartyName,
                    'Account' AS PartyType,
                    av.VoucherType AS TransactionType,
                    av.Debit AS Amount,
                    'DEBIT' AS EntryType,
                    NULL AS Salesman,
                    av.Narration AS Remarks
                FROM [dbo].[AllVouchers] av
                INNER JOIN [dbo].[ChartOfAccounts] coa ON av.AccountNo = coa.AccountNo
                WHERE av.VoucherDate BETWEEN @FromDate AND @ToDate
                    AND av.VoucherType IN ('RV', 'CR') -- Receipt/Cash Receipt
                    AND av.Debit > 0
                
                UNION ALL
                
                -- 6. PAYMENTS (CASH/BANK)
                SELECT 
                    'PAYMENT' AS ActivityType,
                    av.VoucherDate AS TransactionDate,
                    CAST(av.VoucherNo AS VARCHAR(20)) AS ReferenceNo,
                    coa.AccountName AS PartyName,
                    'Account' AS PartyType,
                    av.VoucherType AS TransactionType,
                    av.Credit AS Amount,
                    'CREDIT' AS EntryType,
                    NULL AS Salesman,
                    av.Narration AS Remarks
                FROM [dbo].[AllVouchers] av
                INNER JOIN [dbo].[ChartOfAccounts] coa ON av.AccountNo = coa.AccountNo
                WHERE av.VoucherDate BETWEEN @FromDate AND @ToDate
                    AND av.VoucherType IN ('PV', 'CP') -- Payment/Cash Payment
                    AND av.Credit > 0
                
                UNION ALL
                
                -- 7. JOURNAL ENTRIES
                SELECT 
                    'JOURNAL' AS ActivityType,
                    jv.VoucherDate AS TransactionDate,
                    CAST(jv.VoucherNo AS VARCHAR(20)) AS ReferenceNo,
                    coa.AccountName AS PartyName,
                    'Account' AS PartyType,
                    'JV' AS TransactionType,
                    CASE 
                        WHEN jvb.Debit > 0 THEN jvb.Debit
                        ELSE jvb.Credit
                    END AS Amount,
                    CASE 
                        WHEN jvb.Debit > 0 THEN 'DEBIT'
                        ELSE 'CREDIT'
                    END AS EntryType,
                    NULL AS Salesman,
                    jvb.Narration AS Remarks
                FROM [dbo].[JournalVouchers] jv
                INNER JOIN [dbo].[JournalVouchersBody] jvb ON jv.VoucherNo = jvb.VoucherNo
                INNER JOIN [dbo].[ChartOfAccounts] coa ON jvb.AccountNo = coa.AccountNo
                WHERE jv.VoucherDate BETWEEN @FromDate AND @ToDate
                    AND jv.IsPosted = 1
                
                ORDER BY TransactionDate, ActivityType";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromdatepcker.Value.Date);
                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value.Date);

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching business activity data: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private void ShowReport(bool isPrintMode = false)
        {
            try
            {
                // Validate dates
                if (fromdatepcker.Value > todatepcker.Value)
                {
                    MessageBox.Show("From date cannot be greater than To date", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get data
                DataTable dt = GetDailyBusinessActivityData();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No business activity found for selected date range", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare data sources
                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", dt);

                // Prepare parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                //parameters.Add("ReportTitle", "Daily Business Activity Report");
                //parameters.Add("FromDate", fromdatepcker.Value.ToString("dd/MM/yyyy"));
                //parameters.Add("ToDate", todatepcker.Value.ToString("dd/MM/yyyy"));
                //parameters.Add("GeneratedDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                // Create report viewer instance
                Reports.Report_viewer reportViewer = new Reports.Report_viewer();
                reportViewer.ShowReport("Daily_Business_Activity", dataSources);

                // Print mode specific logic (if needed)
                if (isPrintMode)
                {
                    // You can add print-specific logic here
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report: " + ex.Message, "Report Error",
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
    }
}