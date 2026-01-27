using FO_2022.Reports;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Account_Receivable : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Account_Receivable()
        {
            InitializeComponent();

            // Set dates
            fromdatepicker.Value = DateTime.Now.AddMonths(-1);
            todatepcker.Value = DateTime.Now;

            // Load categories
            LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                ctgrycombobox.Items.Clear();
                ctgrycombobox.Items.Add("All Categories");

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT CategoryName FROM Category ORDER BY CategoryName";
                    SqlCommand cmd = new SqlCommand(query, con);
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ctgrycombobox.Items.Add(dr["CategoryName"].ToString());
                        }
                    }
                }
                ctgrycombobox.SelectedIndex = 0;
            }
            catch
            {
                ctgrycombobox.Items.Add("All Categories");
                ctgrycombobox.SelectedIndex = 0;
            }
        }

        // ⚡⚡⚡ MAIN METHOD - SP KO EK BAAR MEIN CALL KARO ⚡⚡⚡
        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                lblStatus.Text = "Loading report...";
                lblStatus.Visible = true;
                Application.DoEvents();

                // Step 1: SP ko call karo sab accounts ke liye
                DataTable allData = CallSPForAllAccounts();

                if (allData == null || allData.Rows.Count == 0)
                {
                    MessageBox.Show("No data found", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Step 2: Balance filter apply karo
                decimal minBalance = GetMinBalance();
                if (minBalance > 0)
                {
                    allData = ApplyBalanceFilter(allData, minBalance);
                }

                // Step 3: Report show karo
                ShowReport(allData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                lblStatus.Visible = false;
            }
        }

        // ⭐⭐⭐ YE WALA METHOD USE KARO - SP KO FAST CALL KAREGA ⭐⭐⭐
        private DataTable CallSPForAllAccounts()
        {
            DataTable result = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // ⚡ TRICK: SP ko temp table mein result dalo
                    string query = @"
                        -- Create temp table with SP ka structure
                        CREATE TABLE #TempResults (
                            ACCOUNTNO VARCHAR(10),
                            ACCOUNTNAME VARCHAR(100),
                            CLOSINGBAL DECIMAL(18,2),
                            CLOSINGBALTYPE VARCHAR(2)
                        );
                        
                        -- Get all party accounts
                        DECLARE @AccountList TABLE (AccountNo VARCHAR(10));
                        INSERT INTO @AccountList
                        SELECT AccountNo 
                        FROM ChartOfAccounts 
                        WHERE IsDetailed = 1 
                            AND AccountType LIKE '%Parties%';
                        
                        -- Loop through accounts and call SP
                        DECLARE @AccountNo VARCHAR(10);
                        DECLARE account_cursor CURSOR FOR 
                        SELECT AccountNo FROM @AccountList;
                        
                        OPEN account_cursor;
                        FETCH NEXT FROM account_cursor INTO @AccountNo;
                        
                        WHILE @@FETCH_STATUS = 0
                        BEGIN
                            -- Insert SP result into temp table
                            INSERT INTO #TempResults (ACCOUNTNO, ACCOUNTNAME, CLOSINGBAL, CLOSINGBALTYPE)
                            EXEC SPACCACCOUNTSBALANCESFINAL 
                                @FROMDATE = @FromDate,
                                @TODATE = @ToDate,
                                @accountno = @AccountNo;
                            
                            FETCH NEXT FROM account_cursor INTO @AccountNo;
                        END;
                        
                        CLOSE account_cursor;
                        DEALLOCATE account_cursor;
                        
                        -- Return results
                        SELECT * FROM #TempResults;
                        
                        -- Cleanup
                        DROP TABLE #TempResults;";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandTimeout = 120; // 2 minutes
                        cmd.Parameters.AddWithValue("@FromDate", fromdatepicker.Value);
                        cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // ⚡ FALLBACK: Direct query use karo
                MessageBox.Show($"SP failed, using direct query...\n{ex.Message}",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                result = GetDataDirect();
            }

            return result;
        }

        // ⚡ FALLBACK METHOD - Agar SP slow hai toh ye use karo
        private DataTable GetDataDirect()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    // Direct query jo SP jaisa result de
                    string query = @"
                        SELECT 
                            AccountNo AS ACCOUNTNO,
                            AccountName AS ACCOUNTNAME,
                            ABS(AdjustedDebit - AdjustedCredit) AS CLOSINGBAL,
                            CASE 
                                WHEN (AdjustedDebit - AdjustedCredit) >= 0 
                                THEN 'Dr' 
                                ELSE 'Cr' 
                            END AS CLOSINGBALTYPE
                        FROM ChartOfAccounts 
                        WHERE IsDetailed = 1 
                            AND AccountType LIKE '%Parties%'
                            AND ABS(AdjustedDebit - AdjustedCredit) > 0
                        ORDER BY ABS(AdjustedDebit - AdjustedCredit) DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandTimeout = 30;
                        con.Open();

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Direct query failed: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private decimal GetMinBalance()
        {
            if (string.IsNullOrWhiteSpace(balancegreaterthantxtbox.Text))
                return 0;

            return decimal.TryParse(balancegreaterthantxtbox.Text, out decimal balance) ? Math.Abs(balance) : 0;
        }

        private DataTable ApplyBalanceFilter(DataTable data, decimal minBalance)
        {
            DataView dv = data.DefaultView;
            dv.RowFilter = $"CLOSINGBAL >= {minBalance}";
            return dv.ToTable();
        }

        private void ShowReport(DataTable data)
        {
            try
            {
                if (data.Rows.Count == 0)
                {
                    MessageBox.Show("No accounts found with the given criteria", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare report data
                Dictionary<string, DataTable> sources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", data }
                };

                // Prepare parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "ReportTitle", "Accounts Receivable Report" },
                    { "FromDate", fromdatepicker.Value.ToString("dd/MM/yyyy") },
                    { "ToDate", todatepcker.Value.ToString("dd/MM/yyyy") },
                    { "MinBalance", GetMinBalance().ToString("N2") },
                    { "Category", ctgrycombobox.SelectedItem?.ToString() ?? "All Categories" },
                    { "TotalAccounts", data.Rows.Count.ToString() }
                };

                // Show report
                using (Report_viewer viewer = new Report_viewer())
                {
                    viewer.ShowReport("Account_Receivable", sources, parameters);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Report error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ⚡ TEST BUTTON - SP ko test karne ke liye
        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SPACCACCOUNTSBALANCESFINAL", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FROMDATE", fromdatepicker.Value);
                        cmd.Parameters.AddWithValue("@TODATE", todatepcker.Value);
                        cmd.Parameters.AddWithValue("@accountno", "622174");

                        con.Open();

                        DataTable dt = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }

                        if (dt.Rows.Count > 0)
                        {
                            string msg = "";
                            foreach (DataColumn col in dt.Columns)
                            {
                                msg += $"{col.ColumnName}: {dt.Rows[0][col]}\n";
                            }
                            MessageBox.Show(msg, "SP Test - First Row",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"SP Test Failed: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //
        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void baltxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                e.Handled = true;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closebtn_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}