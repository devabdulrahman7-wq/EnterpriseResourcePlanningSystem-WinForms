//using FO_2022.Reports;
//using Microsoft.Reporting.WinForms;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Configuration;
//using System.Data;
//using System.Data.SqlClient;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace FO_2022
//{
//    public partial class Sales_VS_Recoveries : Form
//    {
//        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

//        public Sales_VS_Recoveries()
//        {
//            InitializeComponent();
//        }

//        private void Sales_VS_Recoveries_Load(object sender, EventArgs e)
//        {
//            // Set default dates
//            fromdatepcker.Value = DateTime.Now.AddDays(-30);
//            todatepcker.Value = DateTime.Now;

//            // Load NSM combobox
//            LoadNSMComboBox();
//        }

//        // 📌 LOAD NSM COMBOBOX (Using KeyValuePair)
//        private void LoadNSMComboBox()
//        {
//            try
//            {
//                nsmcombobox.Items.Clear();
//                nsmcombobox.Items.Add(new KeyValuePair<string, string>("All", "All"));

//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    string query = "SELECT NSMid, NSMName FROM NSM ORDER BY NSMName";
//                    using (SqlCommand cmd = new SqlCommand(query, conn))
//                    {
//                        conn.Open();
//                        using (SqlDataReader reader = cmd.ExecuteReader())
//                        {
//                            while (reader.Read())
//                            {
//                                nsmcombobox.Items.Add(new KeyValuePair<string, string>(
//                                    reader["NSMName"].ToString(),
//                                    reader["NSMid"].ToString()
//                                ));
//                            }
//                        }
//                    }
//                }
//                nsmcombobox.SelectedIndex = 0;
//                nsmcombobox.DisplayMember = "Key";
//                nsmcombobox.ValueMember = "Value";
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error loading NSM: {ex.Message}", "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        // 📌 LOAD ASM COMBOBOX BASED ON NSM
//        private void LoadASMComboBox(string nsmId)
//        {
//            try
//            {
//                asmcombobox.Items.Clear();
//                asmcombobox.Items.Add(new KeyValuePair<string, string>("All", "All"));

//                if (nsmId == "All")
//                {
//                    asmcombobox.SelectedIndex = 0;
//                    asmcombobox.DisplayMember = "Key";
//                    asmcombobox.ValueMember = "Value";
//                    return;
//                }

//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    string query = "SELECT ASMId, ASMName FROM ASMs WHERE NSMid = @NSMid ORDER BY ASMName";
//                    using (SqlCommand cmd = new SqlCommand(query, conn))
//                    {
//                        cmd.Parameters.AddWithValue("@NSMid", nsmId);
//                        conn.Open();
//                        using (SqlDataReader reader = cmd.ExecuteReader())
//                        {
//                            while (reader.Read())
//                            {
//                                asmcombobox.Items.Add(new KeyValuePair<string, string>(
//                                    reader["ASMName"].ToString(),
//                                    reader["ASMId"].ToString()
//                                ));
//                            }
//                        }
//                    }
//                }
//                asmcombobox.SelectedIndex = 0;
//                asmcombobox.DisplayMember = "Key";
//                asmcombobox.ValueMember = "Value";
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error loading ASM: {ex.Message}", "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        // 📌 LOAD SALE OFFICERS IN LISTVIEW
//        private void LoadSaleOfficers(string nsmId, string asmId)
//        {
//            try
//            {
//                listViewforsaleofficer.Items.Clear();

//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    string query = @"
//                        SELECT SPOId, SPOName 
//                        FROM SPOs 
//                        WHERE (@NSMid = 'All' OR NSMid = @NSMid)
//                        AND (@ASMId = 'All' OR CONVERT(nvarchar(10), ASMID) = @ASMId)
//                        AND IsActive = 1
//                        ORDER BY SPOName";

//                    using (SqlCommand cmd = new SqlCommand(query, conn))
//                    {
//                        cmd.Parameters.AddWithValue("@NSMid", nsmId);
//                        cmd.Parameters.AddWithValue("@ASMId", asmId);

//                        conn.Open();
//                        using (SqlDataReader reader = cmd.ExecuteReader())
//                        {
//                            while (reader.Read())
//                            {
//                                ListViewItem item = new ListViewItem();
//                                item.Text = reader["SPOName"].ToString();
//                                item.SubItems.Add(reader["SPOId"].ToString());
//                                item.Checked = true;
//                                listViewforsaleofficer.Items.Add(item);
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error loading Sale Officers: {ex.Message}", "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        // 📌 NSM COMBOBOX SELECTION CHANGED
//        private void nsmcombobox_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            if (nsmcombobox.SelectedItem == null) return;

//            string selectedNSMId = ((KeyValuePair<string, string>)nsmcombobox.SelectedItem).Value;

//            LoadASMComboBox(selectedNSMId);
//            LoadSaleOfficers(selectedNSMId, "All");
//        }

//        // 📌 ASM COMBOBOX SELECTION CHANGED
//        private void asmcombobox_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            if (nsmcombobox.SelectedItem == null || asmcombobox.SelectedItem == null) return;

//            string selectedNSMId = ((KeyValuePair<string, string>)nsmcombobox.SelectedItem).Value;
//            string selectedASMId = ((KeyValuePair<string, string>)asmcombobox.SelectedItem).Value;

//            LoadSaleOfficers(selectedNSMId, selectedASMId);
//        }

//        // 📌 SELECT ALL OFFICERS
//        private void slctallfromlistviewbtn_Click(object sender, EventArgs e)
//        {
//            foreach (ListViewItem item in listViewforsaleofficer.Items)
//            {
//                item.Checked = true;
//            }
//        }

//        // 📌 PREVIEW BUTTON
//        private void previewbtn_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                GenerateReport(false);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error: {ex.Message}", "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        // 📌 PRINT BUTTON
//        private void printbtn_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                GenerateReport(true);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error: {ex.Message}", "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        // 📌 GENERATE REPORT
//        private void GenerateReport(bool autoPrint)
//        {
//            // Validate dates
//            if (fromdatepcker.Value > todatepcker.Value)
//            {
//                MessageBox.Show("From date cannot be greater than To date", "Validation Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                return;
//            }

//            // Get selected officers
//            List<string> selectedOfficerIds = new List<string>();
//            foreach (ListViewItem item in listViewforsaleofficer.Items)
//            {
//                if (item.Checked && item.SubItems.Count > 1)
//                {
//                    selectedOfficerIds.Add(item.SubItems[1].Text);
//                }
//            }

//            if (selectedOfficerIds.Count == 0)
//            {
//                MessageBox.Show("Please select at least one Sale Officer", "Validation",
//                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                return;
//            }

//            // Get report type based on radio buttons
//            string reportType = GetSelectedReportType();

//            // Get data
//            DataTable dt = GetSalesVsRecoveryData(
//                fromdatepcker.Value,
//                todatepcker.Value,
//                selectedOfficerIds,
//                reportType);

//            if (dt.Rows.Count == 0)
//            {
//                MessageBox.Show("No data found for the selected criteria", "Information",
//                    MessageBoxButtons.OK, MessageBoxIcon.Information);
//                return;
//            }

//            // Prepare data sources
//            Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>
//            {
//                { "DataSet1", dt }
//            };

//            // Prepare parameters
//            Dictionary<string, string> parameters = new Dictionary<string, string>
//            {
//                //{ "FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy") },
//                //{ "ToDate", todatepcker.Value.ToString("dd-MMM-yyyy") },
//                //{ "ReportType", reportType },
//                //{ "TotalOfficers", selectedOfficerIds.Count.ToString() },
//                //{ "ReportDate", DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
//            };

//            // Show report
//            Report_viewer viewer = new Report_viewer();
//            viewer.ShowReport("Sales_VS_Recoveries", dataSources, parameters, null);
//        }

//        // 📌 GET REPORT TYPE FROM RADIO BUTTONS
//        private string GetSelectedReportType()
//        {
//            // Add this radio button to your form in designer
//            // Name: dailyreportradiobtn (Text: "Daily Report - Recovery Without Builty")

//            if (recoverywithbuiltyradiobtn.Checked)
//                return "Recovery With Builty";
//            else if (asmwitharearadiobtn.Checked)
//                return "ASM With Area - Recovery With Builty";
//            else if (asmwithoutarearadiobtn.Checked)
//                return "ASM Without Area - Recovery Without Builty";
//            else
//                return "Daily Report - Recovery Without Builty";
//        }

//        // 📌 GET SALES VS RECOVERY DATA
//        private DataTable GetSalesVsRecoveryData(DateTime fromDate, DateTime toDate,
//            List<string> officerIds, string reportType)
//        {
//            DataTable dt = new DataTable();

//            // Create comma-separated string of officer IDs
//            string officerIdsString = string.Join(",", officerIds);

//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                string query = @"
//                    SELECT 
//                        NSM.NSMName,
//                        ASM.ASMName,
//                        SPO.SPOName AS OfficerName,
//                        S.SaleDate,
//                        S.BillNo,
//                        S.CustomerId,
//                        P.PartyName AS CustomerName,
//                        SUM(SB.TTLValue) AS TotalSalesValue,
//                        SUM(SB.TTLSTax) AS TotalSalesTax,
//                        SUM(SB.TTLFTax) AS TotalFTax,
//                        (SUM(SB.TTLValue) + SUM(SB.TTLSTax) + SUM(SB.TTLFTax)) AS NetSalesAmount,
//                        R.RecoveryDate,
//                        SUM(ISNULL(RB.Amount, 0)) AS RecoveryAmount,
//                        S.BiltyNo,
//                        S.TransportName
//                    FROM Sales S
//                    INNER JOIN SalesBody SB ON S.SaleId = SB.SaleId
//                    INNER JOIN Parties P ON S.CustomerId = P.PartyID
//                    LEFT JOIN RecoveryBody RB ON S.CustomerId = RB.CustomerID 
//                        AND RB.IsDeleted = 0
//                    LEFT JOIN Recovery R ON RB.RecoveryID = R.RecoveryID 
//                        AND R.RecoveryDate BETWEEN @FromDate AND @ToDate
//                    LEFT JOIN Salesmen SM ON S.SalesmanId = SM.SalesmanID
//                    LEFT JOIN SPOs SPO ON SM.SalesmanID = SPO.ExternalID
//                    LEFT JOIN ASMs ASM ON SPO.ASMID = ASM.ASMId AND SPO.NSMid = ASM.NSMid
//                    LEFT JOIN NSM ON ASM.NSMid = NSM.NSMid
//                    WHERE S.SaleDate BETWEEN @FromDate AND @ToDate
//                    AND S.IsPosted = 1
//                    AND SPO.SPOId IN (SELECT value FROM STRING_SPLIT(@OfficerIds, ','))
//                    GROUP BY 
//                        NSM.NSMName, ASM.ASMName, SPO.SPOName,
//                        S.SaleDate, S.BillNo, S.CustomerId, P.PartyName,
//                        R.RecoveryDate, S.BiltyNo, S.TransportName
//                    ORDER BY S.SaleDate, S.BillNo";

//                using (SqlCommand cmd = new SqlCommand(query, conn))
//                {
//                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
//                    cmd.Parameters.AddWithValue("@ToDate", toDate);
//                    cmd.Parameters.AddWithValue("@OfficerIds", officerIdsString);

//                    conn.Open();
//                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
//                    {
//                        da.Fill(dt);
//                    }
//                }
//            }

//            return dt;
//        }

//        // 📌 DATE CHANGED EVENTS
//        private void fromdatepcker_ValueChanged(object sender, EventArgs e) { }
//        private void todatepcker_ValueChanged(object sender, EventArgs e) { }

//        // 📌 RADIO BUTTON EVENTS
//        private void asmwitharearadiobtn_CheckedChanged(object sender, EventArgs e) { }
//        private void asmwithoutarearadiobtn_CheckedChanged(object sender, EventArgs e) { }
//        private void recoverywithbuiltyradiobtn_CheckedChanged(object sender, EventArgs e) { }

//        private void pictureBox1_Click(object sender, EventArgs e)
//        {
//            this.Close();
//        }

//        private void closebtn_Click(object sender, EventArgs e)
//        {
//            this.Close();
//        }

//        private void listViewforsaleofficer_SelectedIndexChanged(object sender, EventArgs e) { }
//    }
//}
using FO_2022.Reports;
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
    public partial class Sales_VS_Recoveries : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Sales_VS_Recoveries()
        {
            InitializeComponent();
        }

        private void Sales_VS_Recoveries_Load(object sender, EventArgs e)
        {
            // Set default dates
            fromdatepcker.Value = DateTime.Now.AddDays(-30);
            todatepcker.Value = DateTime.Now;

            // Load NSM combobox
            LoadNSMComboBox();
        }

        // 📌 LOAD NSM COMBOBOX
        private void LoadNSMComboBox()
        {
            try
            {
                nsmcombobox.Items.Clear();
                nsmcombobox.Items.Add("All");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT NSMid, NSMName FROM NSM ORDER BY NSMName";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                nsmcombobox.Items.Add(reader["NSMName"].ToString());
                            }
                        }
                    }
                }
                nsmcombobox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading NSM: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 📌 LOAD ASM COMBOBOX BASED ON NSM
        private void LoadASMComboBox(string nsmName)
        {
            try
            {
                asmcombobox.Items.Clear();
                asmcombobox.Items.Add("All");

                if (nsmName == "All")
                {
                    asmcombobox.SelectedIndex = 0;
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT A.ASMId, A.ASMName 
                        FROM ASMs A
                        INNER JOIN NSM N ON A.NSMid = N.NSMid
                        WHERE N.NSMName = @NSMName 
                        ORDER BY A.ASMName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NSMName", nsmName);
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                asmcombobox.Items.Add(reader["ASMName"].ToString());
                            }
                        }
                    }
                }
                asmcombobox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading ASM: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 📌 LOAD SALE OFFICERS IN LISTVIEW
        private void LoadSaleOfficers(string nsmName, string asmName)
        {
            try
            {
                listViewforsaleofficer.Items.Clear();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    StringBuilder query = new StringBuilder(@"
                        SELECT SPO.SPOId, SPO.SPOName 
                        FROM SPOs SPO
                        LEFT JOIN ASMs A ON SPO.ASMID = A.ASMId AND SPO.NSMid = A.NSMid
                        LEFT JOIN NSM N ON SPO.NSMid = N.NSMid
                        WHERE 1=1");

                    List<SqlParameter> parameters = new List<SqlParameter>();

                    if (nsmName != "All")
                    {
                        query.Append(" AND N.NSMName = @NSMName");
                        parameters.Add(new SqlParameter("@NSMName", nsmName));
                    }

                    if (asmName != "All")
                    {
                        query.Append(" AND A.ASMName = @ASMName");
                        parameters.Add(new SqlParameter("@ASMName", asmName));
                    }

                    query.Append(" AND SPO.IsActive = 1 ORDER BY SPO.SPOName");

                    using (SqlCommand cmd = new SqlCommand(query.ToString(), conn))
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.Add(param);
                        }

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListViewItem item = new ListViewItem();
                                item.Text = reader["SPOName"].ToString();
                                item.SubItems.Add(reader["SPOId"].ToString());
                                item.Checked = true;
                                listViewforsaleofficer.Items.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Sale Officers: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 📌 NSM COMBOBOX SELECTION CHANGED
        private void nsmcombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (nsmcombobox.SelectedItem == null) return;

            string selectedNSMName = nsmcombobox.SelectedItem.ToString();
            LoadASMComboBox(selectedNSMName);
            LoadSaleOfficers(selectedNSMName, "All");
        }

        // 📌 ASM COMBOBOX SELECTION CHANGED
        private void asmcombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (nsmcombobox.SelectedItem == null || asmcombobox.SelectedItem == null) return;

            string selectedNSMName = nsmcombobox.SelectedItem.ToString();
            string selectedASMName = asmcombobox.SelectedItem.ToString();

            LoadSaleOfficers(selectedNSMName, selectedASMName);
        }

        // 📌 SELECT ALL OFFICERS
        private void slctallfromlistviewbtn_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listViewforsaleofficer.Items)
            {
                item.Checked = true;
            }
        }

        // 📌 PREVIEW BUTTON
        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                GenerateReport(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 📌 PRINT BUTTON
        private void printbtn_Click(object sender, EventArgs e)
        {
            try
            {
                GenerateReport(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 📌 GENERATE REPORT
        private void GenerateReport(bool autoPrint)
        {
            // Validate dates
            if (fromdatepcker.Value > todatepcker.Value)
            {
                MessageBox.Show("From date cannot be greater than To date", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get selected officers
            List<string> selectedOfficerIds = new List<string>();
            foreach (ListViewItem item in listViewforsaleofficer.Items)
            {
                if (item.Checked && item.SubItems.Count > 1)
                {
                    selectedOfficerIds.Add(item.SubItems[1].Text);
                }
            }

            if (selectedOfficerIds.Count == 0)
            {
                MessageBox.Show("Please select at least one Sale Officer", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get report type based on radio buttons
            string reportType = GetSelectedReportType();

            // Get data
            DataTable dt = GetSalesVsRecoveryData(
                fromdatepcker.Value,
                todatepcker.Value,
                selectedOfficerIds,
                reportType);

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("No data found for the selected criteria", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Prepare data sources
            Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>
            {
                { "DataSet1", dt }
            };

            // Prepare parameters
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                //{ "FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy") },
                //{ "ToDate", todatepcker.Value.ToString("dd-MMM-yyyy") },
                //{ "ReportType", reportType },
                //{ "TotalOfficers", selectedOfficerIds.Count.ToString() },
                //{ "ReportDate", DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
            };

            // Show report
            Report_viewer viewer = new Report_viewer();
            viewer.ShowReport("Sales_VS_Recoveries", dataSources, parameters, null);
        }

        // 📌 GET REPORT TYPE FROM RADIO BUTTONS
        private string GetSelectedReportType()
        {
            if (recoverywithbuiltyradiobtn.Checked)
                return "Recovery With Builty";
            else if (asmwitharearadiobtn.Checked)
                return "ASM With Area - Recovery With Builty";
            else if (asmwithoutarearadiobtn.Checked)
                return "ASM Without Area - Recovery Without Builty";
            else
                return "Daily Report - Recovery Without Builty";
        }

        // 📌 GET SALES VS RECOVERY DATA (FIXED - NO STRING_SPLIT)
        private DataTable GetSalesVsRecoveryData(DateTime fromDate, DateTime toDate,
            List<string> officerIds, string reportType)
        {
            DataTable dt = new DataTable();

            // Create comma-separated string of officer IDs for LIKE query
            string officerIdsList = string.Join(",", officerIds);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        NSM.NSMName,
                        ASM.ASMName,
                        SPO.SPOName AS OfficerName,
                        S.SaleDate,
                        S.BillNo,
                        S.CustomerId,
                        P.PartyName AS CustomerName,
                        SUM(SB.TTLValue) AS TotalSalesValue,
                        SUM(SB.TTLSTax) AS TotalSalesTax,
                        SUM(SB.TTLFTax) AS TotalFTax,
                        (SUM(SB.TTLValue) + SUM(SB.TTLSTax) + SUM(SB.TTLFTax)) AS NetSalesAmount,
                        R.RecoveryDate,
                        SUM(ISNULL(RB.Amount, 0)) AS RecoveryAmount,
                        S.BiltyNo,
                        S.TransportName
                    FROM Sales S
                    INNER JOIN SalesBody SB ON S.SaleId = SB.SaleId
                    INNER JOIN Parties P ON S.CustomerId = P.PartyID
                    LEFT JOIN RecoveryBody RB ON S.CustomerId = RB.CustomerID 
                        AND RB.IsDeleted = 0
                    LEFT JOIN Recovery R ON RB.RecoveryID = R.RecoveryID 
                        AND R.RecoveryDate BETWEEN @FromDate AND @ToDate
                    LEFT JOIN Salesmen SM ON S.SalesmanId = SM.SalesmanID
                    LEFT JOIN SPOs SPO ON SM.SalesmanID = SPO.ExternalID
                    LEFT JOIN ASMs ASM ON SPO.ASMID = ASM.ASMId AND SPO.NSMid = ASM.NSMid
                    LEFT JOIN NSM ON ASM.NSMid = NSM.NSMid
                    WHERE S.SaleDate BETWEEN @FromDate AND @ToDate
                    AND S.IsPosted = 1
                    AND ',' + @OfficerIds + ',' LIKE '%,' + CAST(SPO.SPOId AS NVARCHAR(10)) + ',%'
                    GROUP BY 
                        NSM.NSMName, ASM.ASMName, SPO.SPOName,
                        S.SaleDate, S.BillNo, S.CustomerId, P.PartyName,
                        R.RecoveryDate, S.BiltyNo, S.TransportName
                    ORDER BY S.SaleDate, S.BillNo";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);
                    cmd.Parameters.AddWithValue("@OfficerIds", officerIdsList);

                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        // 📌 DATE CHANGED EVENTS
        private void fromdatepcker_ValueChanged(object sender, EventArgs e) { }
        private void todatepcker_ValueChanged(object sender, EventArgs e) { }
        // 📌 RADIO BUTTON EVENTS
        private void asmwitharearadiobtn_CheckedChanged(object sender, EventArgs e) { }
        private void asmwithoutarearadiobtn_CheckedChanged(object sender, EventArgs e) { }
        private void recoverywithbuiltyradiobtn_CheckedChanged(object sender, EventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void listViewforsaleofficer_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}