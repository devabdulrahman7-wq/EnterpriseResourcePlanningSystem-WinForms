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
//    public partial class Salesman_Sector_List : Form
//    {
//        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

//        private DataTable dtSalesman;
//        private DataTable dtParties;

//        public Salesman_Sector_List()
//        {
//            InitializeComponent();
//        }

//        private void Salesman_Sector_List_Load(object sender, EventArgs e)
//        {
//            // Initialize components or load data if needed
//        }

//        // Search button click event
//        private void salesmanidsearch_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                string salesmanID = salesmanidtxtbox.Text.Trim();

//                if (string.IsNullOrEmpty(salesmanID))
//                {
//                    MessageBox.Show("Please enter Salesman ID", "Warning", 
//                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                    return;
//                }

//                // Get salesman data
//                dtSalesman = GetSalesmanData(salesmanID);

//                if (dtSalesman.Rows.Count == 0)
//                {
//                    MessageBox.Show("Salesman not found!", "Information", 
//                        MessageBoxButtons.OK, MessageBoxIcon.Information);
//                    salesmanidtxtbox.Clear();
//                    salesmannametxtbox.Clear();
//                    return;
//                }

//                // Fill salesman name in textbox
//                salesmannametxtbox.Text = dtSalesman.Rows[0]["SalesmanName"].ToString();

//                // Get parties data if checkbox is checked
//                if (includepartiesforthereasoncheckbox.Checked)
//                {
//                    dtParties = GetPartiesData(salesmanID);
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error: {ex.Message}", "Error", 
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        // Get salesman data from database
//        private DataTable GetSalesmanData(string salesmanID)
//        {
//            DataTable dt = new DataTable();

//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                string query = @"
//                    SELECT 
//                        SalesmanID, 
//                        SalesmanName, 
//                        Address, 
//                        City, 
//                        Phone, 
//                        Mobile,
//                        CommissionRatio,
//                        OfficeID
//                    FROM Salesmen 
//                    WHERE SalesmanID = @SalesmanID";

//                using (SqlCommand cmd = new SqlCommand(query, conn))
//                {
//                    cmd.Parameters.AddWithValue("@SalesmanID", salesmanID);

//                    conn.Open();
//                    SqlDataAdapter da = new SqlDataAdapter(cmd);
//                    da.Fill(dt);
//                }
//            }

//            return dt;
//        }

//        // Get parties data for the salesman
//        private DataTable GetPartiesData(string salesmanID)
//        {
//            DataTable dt = new DataTable();

//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                string query = @"
//                    SELECT 
//                        p.PartyID,
//                        p.PartyName,
//                        p.Address as PartyAddress,
//                        p.City as PartyCity,
//                        p.Phone1 as PartyPhone,
//                        p.PartyType,
//                        s.SectorName,
//                        p.CreditLimit,
//                        p.CreditDays
//                    FROM SalesmenParties sp
//                    INNER JOIN Parties p ON sp.AccountNo = p.PartyID
//                    LEFT JOIN Sectors s ON p.SectorID = s.SectorID
//                    WHERE sp.SalesmanID = @SalesmanID
//                    ORDER BY p.PartyName";

//                using (SqlCommand cmd = new SqlCommand(query, conn))
//                {
//                    cmd.Parameters.AddWithValue("@SalesmanID", salesmanID);

//                    conn.Open();
//                    SqlDataAdapter da = new SqlDataAdapter(cmd);
//                    da.Fill(dt);
//                }
//            }

//            return dt;
//        }

//        // Preview button click even
//        // Salesman ID text changed
//        private void salesmanidtxtbox_TextChanged(object sender, EventArgs e)
//        {
//            // Clear name when ID changes
//            salesmannametxtbox.Clear();
//        }

//        // Include parties checkbox changed
//        private void includepartiesforthereasoncheckbox_CheckedChanged(object sender, EventArgs e)
//        {
//            // If unchecked, clear parties data
//            if (!includepartiesforthereasoncheckbox.Checked)
//            {
//                dtParties = null;
//            }
//            else if (dtSalesman != null && dtSalesman.Rows.Count > 0)
//            {
//                // If checked and salesman exists, load parties
//                dtParties = GetPartiesData(dtSalesman.Rows[0]["SalesmanID"].ToString());
//            }
//        }

//        private void salesmannametxtbox_TextChanged(object sender, EventArgs e)
//        {
//            // No action needed
//        }

//        private void label3_Click(object sender, EventArgs e)
//        {
//            // No action needed
//        }

//        private void printbtn_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                if (dtSalesman == null || dtSalesman.Rows.Count == 0)
//                {
//                    MessageBox.Show("Please search for a salesman first", "Warning",
//                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                    return;
//                }

//                FO_2022.Reports.ReportViewer reportViewer = new FO_2022.Reports.ReportViewer();
//                reportViewer.LoadReport("SalesmanSectorList", dtSalesman, dtParties);
//                reportViewer.PrintReport(); // Direct print without preview
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error: {ex.Message}", "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void previewbtn_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                if (dtSalesman == null || dtSalesman.Rows.Count == 0)
//                {
//                    MessageBox.Show("Please search for a salesman first", "Warning",
//                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                    return;
//                }

//                // Open Report Viewer form with data
//                FO_2022.Reports.ReportViewer reportViewer = new FO_2022.Reports.ReportViewer();

//                // Pass data and report type to viewer
//                reportViewer.LoadReport("SalesmanSectorList", dtSalesman, dtParties);
//                reportViewer.ShowDialog();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error: {ex.Message}", "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }
//    }
//}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Salesman_Sector_List : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Salesman_Sector_List()
        {
            InitializeComponent();
        }

        // 📌 SEARCH BUTTON
        private void salesmanidsearch_Click(object sender, EventArgs e)
        {
            string salesmanID = salesmanidtxtbox.Text.Trim();

            if (string.IsNullOrEmpty(salesmanID))
            {
                MessageBox.Show("Please enter Salesman ID", "Input Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Get data
                DataTable dtSalesman = GetSalesmanData(salesmanID);

                if (dtSalesman.Rows.Count == 0)
                {
                    MessageBox.Show("Salesman not found!", "Not Found",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Show salesman name
                salesmannametxtbox.Text = dtSalesman.Rows[0]["SalesmanName"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 📌 PREVIEW BUTTON
        private void previewbtn_Click(object sender, EventArgs e)
        {
            string salesmanID = salesmanidtxtbox.Text.Trim();

            if (string.IsNullOrEmpty(salesmanID))
            {
                MessageBox.Show("Please search for a salesman first", "Input Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 📌 STEP 1: Get Salesman Data
                DataTable dtSalesman = GetSalesmanData(salesmanID);

                // 📌 STEP 2: Prepare DataSources Dictionary
                var dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("dsSalesman", dtSalesman); // Must match RDLC DataSet name

                // 📌 STEP 3: Add Parties if checkbox checked
                if (includepartiesforthereasoncheckbox.Checked)
                {
                    DataTable dtParties = GetPartiesData(salesmanID);
                    dataSources.Add("dsParties", dtParties); // Must match RDLC DataSet name
                }

                // 📌 STEP 4: Show Report
                FO_2022.Reports.ReportViewer reportViewer = new FO_2022.Reports.ReportViewer();
                reportViewer.ShowReport("SalesmanSectorList", dataSources);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Report Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 📌 GET SALESMAN DATA
        private DataTable GetSalesmanData(string salesmanID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT SalesmanID, SalesmanName, Address, City, Phone, Mobile
                    FROM Salesmen 
                    WHERE SalesmanID = @SalesmanID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SalesmanID", salesmanID);

                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        // 📌 GET PARTIES DATA
        private DataTable GetPartiesData(string salesmanID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT p.PartyID, p.PartyName, p.Address as PartyAddress, 
                           p.City as PartyCity, p.Phone1 as PartyPhone, p.PartyType
                    FROM SalesmenParties sp
                    INNER JOIN Parties p ON sp.AccountNo = p.PartyID
                    WHERE sp.SalesmanID = @SalesmanID
                    ORDER BY p.PartyName";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SalesmanID", salesmanID);

                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        // 📌 PRINT BUTTON (Direct Print without preview)
        private void printbtn_Click(object sender, EventArgs e)
        {
            previewbtn_Click(sender, e); // Same as preview
        }

        // 📌 CHECKBOX CHANGED
        private void includepartiesforthereasoncheckbox_CheckedChanged(object sender, EventArgs e)
        {
            // No action needed - data loaded at preview time
        }

        private void salesmanidtxtbox_TextChanged(object sender, EventArgs e)
        {
            salesmannametxtbox.Clear();
        }
    }
}