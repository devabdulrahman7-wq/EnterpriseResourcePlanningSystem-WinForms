using FO_2022.Reports;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace FO_2022
{
    public partial class Sale_Register_1_ : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Sale_Register_1_()
        {
            InitializeComponent();
        }

        private void Sale_Register_1__Load(object sender, EventArgs e)
        {
            fromdatepcker.Value = DateTime.Now.AddMonths(-24);
            todatepcker.Value = DateTime.Now;
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            this.Close();
        }
private void cmpnyidsearch_Click(object sender, EventArgs e)
{
    string companyID = cmpnyidtxtbox.Text.Trim();
    string companyName = cmpnynametxtbox.Text.Trim();

    if (string.IsNullOrEmpty(companyID) && string.IsNullOrEmpty(companyName))
    {
        MessageBox.Show("Please enter Company ID or Company Name",
            "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
    }

    try
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            string query = @"
                SELECT CompanyID, CompanyName
                FROM Companies
                WHERE CompanyID = @CompanyID
                   OR CompanyName = @CompanyName";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@CompanyID", companyID);
                cmd.Parameters.AddWithValue("@CompanyName", companyName);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cmpnyidtxtbox.Text = reader["CompanyID"].ToString();
                        cmpnynametxtbox.Text = reader["CompanyName"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("Company not found!",
                            "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        cmpnyidtxtbox.Clear();
                        cmpnynametxtbox.Clear();
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show("Error searching company: " + ex.Message);
    }
}

        private void cstmridsearch_Click(object sender, EventArgs e)
        {
            string customerID = cstmridtxtbox.Text.Trim();
            string customerName = cstmrnametxtbox.Text.Trim();

            if (string.IsNullOrEmpty(customerID) && string.IsNullOrEmpty(customerName))
            {
                MessageBox.Show("Please enter Customer ID or Customer Name",
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                SELECT PartyID, PartyName
                FROM Parties
                WHERE (PartyID = @PartyID OR PartyName = @PartyName)
                AND PartyType = 'C'";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PartyID", customerID);
                        cmd.Parameters.AddWithValue("@PartyName", customerName);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cstmridtxtbox.Text = reader["PartyID"].ToString();
                                cstmrnametxtbox.Text = reader["PartyName"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Customer not found!",
                                    "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                cstmridtxtbox.Clear();
                                cstmrnametxtbox.Clear();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching customer: " + ex.Message);
            }
        }

        private void sectoridsearch_Click(object sender, EventArgs e)
        {
            string sectorID = sectoridtxtbox.Text.Trim();
            string sectorName = sectornametxtbox.Text.Trim();

            if (string.IsNullOrEmpty(sectorID) && string.IsNullOrEmpty(sectorName))
            {
                MessageBox.Show("Please enter Sector ID or Sector Name",
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                SELECT SectorID, SectorName
                FROM Sectors
                WHERE SectorID = @SectorID
                   OR SectorName = @SectorName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SectorID", sectorID);
                        cmd.Parameters.AddWithValue("@SectorName", sectorName);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                sectoridtxtbox.Text = reader["SectorID"].ToString();
                                sectornametxtbox.Text = reader["SectorName"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Sector not found!",
                                    "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                sectoridtxtbox.Clear();
                                sectornametxtbox.Clear();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching sector: " + ex.Message);
            }
        }

        private void townidsearch_Click(object sender, EventArgs e)
        {
            string townID = townidtxtbox.Text.Trim();
            string townName = townnametxtbox.Text.Trim();

            if (string.IsNullOrEmpty(townID) && string.IsNullOrEmpty(townName))
            {
                MessageBox.Show("Please enter Town ID or Town Name",
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                SELECT TownID, TownName
                FROM Towns
                WHERE TownID = @TownID
                   OR TownName = @TownName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TownID", townID);
                        cmd.Parameters.AddWithValue("@TownName", townName);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                townidtxtbox.Text = reader["TownID"].ToString();
                                townnametxtbox.Text = reader["TownName"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Town not found!",
                                    "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                townidtxtbox.Clear();
                                townnametxtbox.Clear();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching town: " + ex.Message);
            }
        }


        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (fromdatepcker.Value > todatepcker.Value)
                {
                    MessageBox.Show("From date cannot be greater than To date", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Validate sale ID range
                if (!string.IsNullOrEmpty(fromsaletxtbox.Text) && !string.IsNullOrEmpty(tosaletxtbox.Text))
                {
                    if (!int.TryParse(fromsaletxtbox.Text, out int fromSale) || !int.TryParse(tosaletxtbox.Text, out int toSale))
                    {
                        MessageBox.Show("Please enter valid numeric Sale IDs", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (fromSale > toSale)
                    {
                        MessageBox.Show("From Sale ID cannot be greater than To Sale ID", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // Validate bill number range
                if (!string.IsNullOrEmpty(frombilltxtbox.Text) && !string.IsNullOrEmpty(tobilltxtbox.Text))
                {
                    if (!int.TryParse(frombilltxtbox.Text, out int fromBill) || !int.TryParse(tobilltxtbox.Text, out int toBill))
                    {
                        MessageBox.Show("Please enter valid numeric Bill numbers", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (fromBill > toBill)
                    {
                        MessageBox.Show("From Bill number cannot be greater than To Bill number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                DataTable salesData = GetSalesRegisterData();

                if (salesData.Rows.Count == 0)
                {
                    MessageBox.Show("No sales data found for selected criteria", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", salesData);

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                //parameters.Add("ReportTitle", "Sales Register Report");
                //parameters.Add("GeneratedDate", DateTime.Now.ToString("dd-MMM-yyyy HH:mm"));
                //parameters.Add("FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy"));
                //parameters.Add("ToDate", todatepcker.Value.ToString("dd-MMM-yyyy"));
                //parameters.Add("CompanyName", cmpnynametxtbox.Text);
                //parameters.Add("CustomerName", cstmrnametxtbox.Text);
                //parameters.Add("SectorName", sectornametxtbox.Text);
                //parameters.Add("TownName", townnametxtbox.Text);
                //parameters.Add("ShowPendingRates", checkBoxshowonlypendingrates.Checked ? "Yes" : "No");

                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Sale_Register", dataSources );
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            previewbtn_Click(sender, e);
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private DataTable GetSalesRegisterData()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("SELECT ");
                    query.AppendLine("    S.SaleId,");
                    query.AppendLine("    S.SaleDate,");
                    query.AppendLine("    S.BillNo,");
                    query.AppendLine("    P.PartyName AS CustomerName,");
                    query.AppendLine("    SEC.SectorName,");
                    query.AppendLine("    T.TownName,");
                    query.AppendLine("    C.CompanyName,");
                    query.AppendLine("    PR.ProductName,");
                    query.AppendLine("    PK.PackingName,");
                    query.AppendLine("    SB.QtyLose,");
                    query.AppendLine("    SB.PricePack,");
                    query.AppendLine("    SB.DiscValLose,");
                    query.AppendLine("    SB.STaxValPack,");
                    query.AppendLine("    SB.TTLValue,");
                    query.AppendLine("    SB.PerUnitCostLose");
                    query.AppendLine("FROM Sales S");
                    query.AppendLine("INNER JOIN SalesBody SB ON S.SaleId = SB.SaleId");
                    query.AppendLine("INNER JOIN Parties P ON S.CustomerId = P.PartyID");
                    query.AppendLine("LEFT JOIN Sectors SEC ON S.SectorID = SEC.SectorID");
                    query.AppendLine("LEFT JOIN Towns T ON SEC.TownID = T.TownID");
                    query.AppendLine("INNER JOIN Products PR ON SB.ProductId = PR.ProductID");
                    query.AppendLine("INNER JOIN Companies C ON PR.CompanyID = C.CompanyID");
                    query.AppendLine("LEFT JOIN Packings PK ON SB.PackingId = PK.PackingId");
                    query.AppendLine("WHERE S.SaleDate BETWEEN @FromDate AND @ToDate");

                    // Add filters
                    if (!string.IsNullOrEmpty(cmpnyidtxtbox.Text))
                        query.AppendLine("AND C.CompanyID = @CompanyID");

                    if (!string.IsNullOrEmpty(cstmridtxtbox.Text))
                        query.AppendLine("AND S.CustomerId = @CustomerID");

                    if (!string.IsNullOrEmpty(sectoridtxtbox.Text))
                        query.AppendLine("AND SEC.SectorID = @SectorID");

                    if (!string.IsNullOrEmpty(townidtxtbox.Text))
                        query.AppendLine("AND T.TownID = @TownID");

                    if (!string.IsNullOrEmpty(fromsaletxtbox.Text) && !string.IsNullOrEmpty(tosaletxtbox.Text))
                        query.AppendLine("AND S.SaleId BETWEEN @FromSaleID AND @ToSaleID");

                    if (!string.IsNullOrEmpty(frombilltxtbox.Text) && !string.IsNullOrEmpty(tobilltxtbox.Text))
                        query.AppendLine("AND S.BillNo BETWEEN @FromBillNo AND @ToBillNo");

                    if (checkBoxshowonlypendingrates.Checked)
                        query.AppendLine("AND S.IsPendingRate = 1");

                    query.AppendLine("ORDER BY S.SaleDate, S.SaleId, C.CompanyName, PR.ProductName");

                    SqlCommand cmd = new SqlCommand(query.ToString(), conn);
                    cmd.Parameters.AddWithValue("@FromDate", fromdatepcker.Value);
                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value);

                    // Add optional parameters only if they have values
                    if (!string.IsNullOrEmpty(cmpnyidtxtbox.Text))
                        cmd.Parameters.AddWithValue("@CompanyID", cmpnyidtxtbox.Text);

                    if (!string.IsNullOrEmpty(cstmridtxtbox.Text))
                        cmd.Parameters.AddWithValue("@CustomerID", cstmridtxtbox.Text);

                    if (!string.IsNullOrEmpty(sectoridtxtbox.Text))
                    {
                        if (int.TryParse(sectoridtxtbox.Text, out int sectorId))
                            cmd.Parameters.AddWithValue("@SectorID", sectorId);
                    }

                    if (!string.IsNullOrEmpty(townidtxtbox.Text))
                    {
                        if (int.TryParse(townidtxtbox.Text, out int townId))
                            cmd.Parameters.AddWithValue("@TownID", townId);
                    }

                    if (!string.IsNullOrEmpty(fromsaletxtbox.Text) && !string.IsNullOrEmpty(tosaletxtbox.Text))
                    {
                        cmd.Parameters.AddWithValue("@FromSaleID", int.Parse(fromsaletxtbox.Text));
                        cmd.Parameters.AddWithValue("@ToSaleID", int.Parse(tosaletxtbox.Text));
                    }

                    if (!string.IsNullOrEmpty(frombilltxtbox.Text) && !string.IsNullOrEmpty(tobilltxtbox.Text))
                    {
                        cmd.Parameters.AddWithValue("@FromBillNo", int.Parse(frombilltxtbox.Text));
                        cmd.Parameters.AddWithValue("@ToBillNo", int.Parse(tobilltxtbox.Text));
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sales data: " + ex.Message);
            }

            return dt;
        }
    }
}