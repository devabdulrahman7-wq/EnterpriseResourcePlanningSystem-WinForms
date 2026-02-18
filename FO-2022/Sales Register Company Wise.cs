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
    public partial class Sales_Register_Company_Wise : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Sales_Register_Company_Wise()
        {
            InitializeComponent();
        }

        private void Sales_Register_Company_Wise_Load(object sender, EventArgs e)
        {
            fromdatepcker.Value = DateTime.Now.AddMonths(-36);
            todatepcker.Value = DateTime.Now;
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmpnyidsearch_Click(object sender, EventArgs e)
        {
            SearchEntity("Companies", "CompanyID", "CompanyName",
                cmpnyidtxtbox, cmpnynametxtbox);
        }

        private void cstmridsearch_Click(object sender, EventArgs e)
        {
            SearchEntity("Parties", "PartyID", "PartyName",
                cstmridtxtbox, cstmrnametxtbox);
        }

        private void sectoridsearch_Click(object sender, EventArgs e)
        {
            SearchEntity("Sectors", "SectorID", "SectorName",
                sectoridtxtbox, sectornametxtbox);
        }

        private void townidsearch_Click(object sender, EventArgs e)
        {
            SearchEntity("Towns", "TownID", "TownName",
                townidtxtbox, townnametxtbox);
        }

        private void SearchEntity(string tableName,
                                  string idColumn,
                                  string nameColumn,
                                  TextBox idTextBox,
                                  TextBox nameTextBox)
        {
            string idValue = idTextBox.Text.Trim();
            string nameValue = nameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(idValue) && string.IsNullOrEmpty(nameValue))
            {
                MessageBox.Show("Please enter ID or Name",
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                idTextBox.Focus();
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query;

                    if (!string.IsNullOrEmpty(idValue))
                        query = $"SELECT {idColumn}, {nameColumn} FROM {tableName} WHERE {idColumn} = @Value";
                    else
                        query = $"SELECT {idColumn}, {nameColumn} FROM {tableName} WHERE {nameColumn} = @Value";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Value",
                            !string.IsNullOrEmpty(idValue) ? idValue : nameValue);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                idTextBox.Text = reader[idColumn].ToString();
                                nameTextBox.Text = reader[nameColumn].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Record not found",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                idTextBox.Clear();
                                nameTextBox.Clear();
                                idTextBox.Focus();
                            }
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


        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (fromdatepcker.Value > todatepcker.Value)
                {
                    MessageBox.Show("From date cannot be greater than To date", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DataTable salesData = GetCompanyWiseSalesData();

                if (salesData.Rows.Count == 0)
                {
                    MessageBox.Show("No sales data found for selected criteria", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", salesData);

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                //parameters.Add("ReportTitle", "Sales Register Company Wise Report");
                //parameters.Add("GeneratedDate", DateTime.Now.ToString("dd-MMM-yyyy HH:mm"));
                //parameters.Add("FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy"));
                //parameters.Add("ToDate", todatepcker.Value.ToString("dd-MMM-yyyy"));
                //parameters.Add("CompanyName", cmpnynametxtbox.Text);
                //parameters.Add("CustomerName", cstmrnametxtbox.Text);
                //parameters.Add("SectorName", sectornametxtbox.Text);
                //parameters.Add("TownName", townnametxtbox.Text);

                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Sales_Register_Company_Wise", dataSources);
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

        private DataTable GetCompanyWiseSalesData()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("SELECT ");
                    query.AppendLine("    C.CompanyID,");
                    query.AppendLine("    C.CompanyName,");
                    query.AppendLine("    COUNT(DISTINCT S.SaleId) AS TotalInvoices,");
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
                    query.AppendLine("LEFT JOIN Parties PT ON S.CustomerId = PT.PartyID");
                    query.AppendLine("LEFT JOIN Sectors SEC ON S.SectorID = SEC.SectorID");
                    query.AppendLine("LEFT JOIN Towns T ON SEC.TownID = T.TownID");
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

                    query.AppendLine("GROUP BY C.CompanyID, C.CompanyName");
                    query.AppendLine("ORDER BY C.CompanyName");

                    SqlCommand cmd = new SqlCommand(query.ToString(), conn);
                    cmd.Parameters.AddWithValue("@FromDate", fromdatepcker.Value);
                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value);

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