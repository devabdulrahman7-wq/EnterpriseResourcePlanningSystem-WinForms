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

namespace FO_2022
{
    public partial class Sales_Summary_Product_Wise : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        private string selectedCustomerId = "";
        private string selectedSalesmanId = "";
        private string selectedSectorId = "";
        private string selectedTownId = "";

        public Sales_Summary_Product_Wise()
        {
            InitializeComponent();
        }

        private void Sales_Summary_Product_Wise_Load(object sender, EventArgs e)
        {
            fromdatepcker.Value = DateTime.Now.AddMonths(-1);
            todatepcker.Value = DateTime.Now;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // ================= SEARCH BUTTONS =================

        private void cstmridsearch_Click(object sender, EventArgs e)
        {
            SearchEntity("Parties", "PartyID", "PartyName",
                cstmridtxtbox, cstmrnametxtbox, ref selectedCustomerId);
        }

        private void salesmanidsearch_Click(object sender, EventArgs e)
        {
            SearchEntity("Salesmen", "SalesmanID", "SalesmanName",
                salesmanidtxtbox, salesmannametxtbox, ref selectedSalesmanId);
        }

        private void sectoridsearch_Click(object sender, EventArgs e)
        {
            SearchEntity("Sectors", "SectorID", "SectorName",
                sectoridtxtbox, sectornametxtbox, ref selectedSectorId);
        }

        private void townidsearch_Click(object sender, EventArgs e)
        {
            SearchEntity("Towns", "TownID", "TownName",
                townidtxtbox, townnametxtbox, ref selectedTownId);
        }

        // ================= GENERIC SEARCH METHOD =================

        private void SearchEntity(string tableName, string idColumn, string nameColumn,
            TextBox idTextBox, TextBox nameTextBox, ref string selectedId)
        {
            string id = idTextBox.Text.Trim();
            string name = nameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter ID or Name",
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query;
                    SqlCommand cmd;

                    if (!string.IsNullOrEmpty(id))
                    {
                        query = $"SELECT {idColumn}, {nameColumn} FROM {tableName} WHERE {idColumn} = @Value";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Value", id);
                    }
                    else
                    {
                        query = $"SELECT {idColumn}, {nameColumn} FROM {tableName} WHERE {nameColumn} = @Value";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Value", name);
                    }

                    using (cmd)
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            idTextBox.Text = reader[idColumn].ToString();
                            nameTextBox.Text = reader[nameColumn].ToString();
                            selectedId = reader[idColumn].ToString();
                        }
                        else
                        {
                            idTextBox.Clear();
                            nameTextBox.Clear();
                            selectedId = "";

                            MessageBox.Show("Record not found",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ================= PREVIEW =================

        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (fromdatepcker.Value > todatepcker.Value)
                {
                    MessageBox.Show("From date cannot be greater than To date",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!string.IsNullOrEmpty(fromsaletxtbox.Text) && !string.IsNullOrEmpty(tosaletxtbox.Text))
                {
                    if (!int.TryParse(fromsaletxtbox.Text, out int fromSale) ||
                        !int.TryParse(tosaletxtbox.Text, out int toSale))
                    {
                        MessageBox.Show("Please enter valid numeric Sale IDs",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (fromSale > toSale)
                    {
                        MessageBox.Show("From Sale ID cannot be greater than To Sale ID",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                DataTable salesData = GetProductWiseSalesData();

                if (salesData.Rows.Count == 0)
                {
                    MessageBox.Show("No sales data found for selected criteria",
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", salesData);

                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Sales_Summary_Product_Wise", dataSources);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            previewbtn_Click(sender, e);
        }

        // ================= DATA METHOD =================

        private DataTable GetProductWiseSalesData()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    StringBuilder query = new StringBuilder();

                    query.AppendLine("SELECT ");
                    query.AppendLine("    P.ProductID,");
                    query.AppendLine("    P.ProductName,");
                    query.AppendLine("    C.CompanyName,");
                    query.AppendLine("    PG.GroupName,");
                    query.AppendLine("    SUM(SB.QtyLose) AS TotalQty,");
                    query.AppendLine("    SUM(SB.TTLValue) AS TotalSale,");
                    query.AppendLine("    SUM(SB.PerUnitCostLose * SB.QtyLose) AS TotalCost,");
                    query.AppendLine("    SUM(SB.TTLValue) - SUM(SB.PerUnitCostLose * SB.QtyLose) AS TotalProfit,");
                    query.AppendLine("    CASE ");
                    query.AppendLine("        WHEN SUM(SB.TTLValue) > 0 ");
                    query.AppendLine("        THEN ((SUM(SB.TTLValue) - SUM(SB.PerUnitCostLose * SB.QtyLose)) / SUM(SB.TTLValue)) * 100 ");
                    query.AppendLine("        ELSE 0 ");
                    query.AppendLine("    END AS ProfitPercentage");
                    query.AppendLine("FROM Sales S");
                    query.AppendLine("INNER JOIN SalesBody SB ON S.SaleId = SB.SaleId");
                    query.AppendLine("INNER JOIN Products P ON SB.ProductId = P.ProductID");
                    query.AppendLine("INNER JOIN Companies C ON P.CompanyID = C.CompanyID");
                    query.AppendLine("INNER JOIN ProductGroups PG ON P.CompanyID = PG.CompanyId AND P.GroupId = PG.GroupId");
                    query.AppendLine("LEFT JOIN Parties PT ON S.CustomerId = PT.PartyID");
                    query.AppendLine("LEFT JOIN Salesmen SM ON S.SalesmanId = SM.SalesmanID");
                    query.AppendLine("LEFT JOIN Sectors SEC ON S.SectorID = SEC.SectorID");
                    query.AppendLine("LEFT JOIN Towns T ON SEC.TownID = T.TownID");
                    query.AppendLine("WHERE S.SaleDate BETWEEN @FromDate AND @ToDate");

                    if (!string.IsNullOrEmpty(selectedCustomerId))
                        query.AppendLine("AND S.CustomerId = @CustomerID");

                    if (!string.IsNullOrEmpty(selectedSalesmanId))
                        query.AppendLine("AND S.SalesmanId = @SalesmanID");

                    if (!string.IsNullOrEmpty(selectedSectorId))
                        query.AppendLine("AND SEC.SectorID = @SectorID");

                    if (!string.IsNullOrEmpty(selectedTownId))
                        query.AppendLine("AND T.TownID = @TownID");

                    if (!string.IsNullOrEmpty(fromsaletxtbox.Text) && !string.IsNullOrEmpty(tosaletxtbox.Text))
                        query.AppendLine("AND S.SaleId BETWEEN @FromSaleID AND @ToSaleID");

                    query.AppendLine("GROUP BY P.ProductID, P.ProductName, C.CompanyName, PG.GroupName");
                    query.AppendLine("ORDER BY C.CompanyName, PG.GroupName, P.ProductName");

                    SqlCommand cmd = new SqlCommand(query.ToString(), conn);

                    cmd.Parameters.AddWithValue("@FromDate", fromdatepcker.Value);
                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value);

                    if (!string.IsNullOrEmpty(selectedCustomerId))
                        cmd.Parameters.AddWithValue("@CustomerID", selectedCustomerId);

                    if (!string.IsNullOrEmpty(selectedSalesmanId))
                        cmd.Parameters.AddWithValue("@SalesmanID", selectedSalesmanId);

                    if (!string.IsNullOrEmpty(selectedSectorId))
                        cmd.Parameters.AddWithValue("@SectorID", selectedSectorId);

                    if (!string.IsNullOrEmpty(selectedTownId))
                        cmd.Parameters.AddWithValue("@TownID", selectedTownId);

                    if (!string.IsNullOrEmpty(fromsaletxtbox.Text) && !string.IsNullOrEmpty(tosaletxtbox.Text))
                    {
                        cmd.Parameters.AddWithValue("@FromSaleID", int.Parse(fromsaletxtbox.Text));
                        cmd.Parameters.AddWithValue("@ToSaleID", int.Parse(tosaletxtbox.Text));
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

        private void cstmridtxtbox_TextChanged_1(object sender, EventArgs e)
        {
            cstmrnametxtbox.Clear();
            selectedCustomerId = "";
        }

        private void salesmanidtxtbox_TextChanged_1(object sender, EventArgs e)
        {
            salesmannametxtbox.Clear();
            selectedSalesmanId = "";
        }

        private void townidtxtbox_TextChanged_1(object sender, EventArgs e)
        {
            townnametxtbox.Clear();
            selectedTownId = "";
        }

        private void sectoridtxtbox_TextChanged_1(object sender, EventArgs e)
        {
            sectornametxtbox.Clear();
            selectedSectorId = "";
        }
    }
}
