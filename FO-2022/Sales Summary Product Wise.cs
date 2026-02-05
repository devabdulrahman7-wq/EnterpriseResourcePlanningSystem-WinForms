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

        private void cstmridsearch_Click(object sender, EventArgs e)
        {
            SearchEntity("Parties", "PartyName", cstmridtxtbox.Text.Trim(), cstmrnametxtbox, "PartyID");
        }

        private void salesmanidsearch_Click(object sender, EventArgs e)
        {
            SearchEntity("Salesmen", "SalesmanName", salesmanidtxtbox.Text.Trim(), salesmannametxtbox, "SalesmanID");
        }

        private void sectoridsearch_Click(object sender, EventArgs e)
        {
            SearchEntity("Sectors", "SectorName", sectoridtxtbox.Text.Trim(), sectornametxtbox, "SectorID");
        }

        private void townidsearch_Click(object sender, EventArgs e)
        {
            SearchEntity("Towns", "TownName", townidtxtbox.Text.Trim(), townnametxtbox, "TownID");
        }

        private void SearchEntity(string tableName, string nameColumn, string id, TextBox nameTextBox, string idColumn)
        {
            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show($"Please enter ID", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = $"SELECT {nameColumn} FROM {tableName} WHERE {idColumn} = @ID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", id);

                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        nameTextBox.Text = result.ToString();
                    }
                    else
                    {
                        MessageBox.Show($"Entity not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        nameTextBox.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                DataTable salesData = GetProductWiseSalesData();

                if (salesData.Rows.Count == 0)
                {
                    MessageBox.Show("No sales data found for selected criteria", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", salesData);

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                //parameters.Add("ReportTitle", "Sales Summary Product Wise Report");
                //parameters.Add("GeneratedDate", DateTime.Now.ToString("dd-MMM-yyyy HH:mm"));
                //parameters.Add("FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy"));
                //parameters.Add("ToDate", todatepcker.Value.ToString("dd-MMM-yyyy"));
                //parameters.Add("CustomerName", cstmrnametxtbox.Text);
                //parameters.Add("SalesmanName", salesmannametxtbox.Text);
                //parameters.Add("SectorName", sectornametxtbox.Text);
                //parameters.Add("TownName", townnametxtbox.Text);

                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Sales_Summary_Product_Wise", dataSources);
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

                    // Add filters
                    if (!string.IsNullOrEmpty(cstmridtxtbox.Text))
                        query.AppendLine("AND S.CustomerId = @CustomerID");

                    if (!string.IsNullOrEmpty(salesmanidtxtbox.Text))
                        query.AppendLine("AND S.SalesmanId = @SalesmanID");

                    if (!string.IsNullOrEmpty(sectoridtxtbox.Text))
                        query.AppendLine("AND SEC.SectorID = @SectorID");

                    if (!string.IsNullOrEmpty(townidtxtbox.Text))
                        query.AppendLine("AND T.TownID = @TownID");

                    if (!string.IsNullOrEmpty(fromsaletxtbox.Text) && !string.IsNullOrEmpty(tosaletxtbox.Text))
                        query.AppendLine("AND S.SaleId BETWEEN @FromSaleID AND @ToSaleID");

                    query.AppendLine("GROUP BY P.ProductID, P.ProductName, C.CompanyName, PG.GroupName");
                    query.AppendLine("ORDER BY C.CompanyName, PG.GroupName, P.ProductName");

                    SqlCommand cmd = new SqlCommand(query.ToString(), conn);
                    cmd.Parameters.AddWithValue("@FromDate", fromdatepcker.Value);
                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value);

                    if (!string.IsNullOrEmpty(cstmridtxtbox.Text))
                        cmd.Parameters.AddWithValue("@CustomerID", cstmridtxtbox.Text);

                    if (!string.IsNullOrEmpty(salesmanidtxtbox.Text))
                        cmd.Parameters.AddWithValue("@SalesmanID", salesmanidtxtbox.Text);

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