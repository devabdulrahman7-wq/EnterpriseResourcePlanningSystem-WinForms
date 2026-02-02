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
    public partial class Invoice_Detailed_Profit : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private int saleId = 0;

        public Invoice_Detailed_Profit()
        {
            InitializeComponent();
        }

        private void Invoice_Detailed_Profit_Load(object sender, EventArgs e)
        {
            // Optional: Set default values
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saleidsearch_Click(object sender, EventArgs e)
        {
            string saleIdText = saleidtxtbox.Text.Trim();

            if (string.IsNullOrEmpty(saleIdText))
            {
                MessageBox.Show("Please enter Sale ID", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                saleidtxtbox.Focus();
                return;
            }

            if (!int.TryParse(saleIdText, out saleId))
            {
                MessageBox.Show("Please enter a valid numeric Sale ID", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                saleidtxtbox.Focus();
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            S.SaleDate,
                            P.PartyName AS CustomerName,
                            SE.SectorName,
                            S.SaleId
                        FROM Sales S
                        INNER JOIN Parties P ON S.CustomerId = P.PartyID
                        LEFT JOIN Sectors SE ON S.SectorID = SE.SectorID
                        WHERE S.SaleId = @SaleId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SaleId", saleId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        saledatetxtbox.Text = Convert.ToDateTime(reader["SaleDate"]).ToString("dd-MMM-yyyy");
                        cstmrnametxtbox.Text = reader["CustomerName"].ToString();
                        sectornametxtbox.Text = reader["SectorName"] != DBNull.Value ? reader["SectorName"].ToString() : "N/A";
                        saleidtxtbox.Text = reader["SaleId"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("Sale not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ClearForm();
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching sale: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (saleId == 0)
                {
                    MessageBox.Show("Please search for a sale first", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DataTable invoiceDetailData = GetInvoiceDetailedData();

                if (invoiceDetailData.Rows.Count == 0)
                {
                    MessageBox.Show("No data found for this invoice", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", invoiceDetailData);

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                //parameters.Add("ReportTitle", "Invoice Detailed Profit Report");
                //parameters.Add("GeneratedDate", DateTime.Now.ToString("dd-MMM-yyyy HH:mm"));
                //parameters.Add("SaleID", saleidtxtbox.Text);
                //parameters.Add("SaleDate", saledatetxtbox.Text);
                //parameters.Add("CustomerName", cstmrnametxtbox.Text);
                //parameters.Add("SectorName", sectornametxtbox.Text);
                //parameters.Add("TotalProducts", invoiceDetailData.Rows.Count.ToString());

                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Invoice_Detailed_Profit", dataSources);
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

        private DataTable GetInvoiceDetailedData()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            SB.ProductId,
                            P.ProductName,
                            C.CompanyName,
                            PG.GroupName,
                            PK.PackingName,
                            SB.QtyLose,
                            SB.PricePack,
                            SB.DiscValLose,
                            SB.STaxValPack,
                            SB.TTLValue AS SaleValue,
                            SB.PerUnitCostLose,
                            (SB.QtyLose * SB.PerUnitCostLose) AS Cost,
                            (SB.TTLValue - (SB.QtyLose * SB.PerUnitCostLose)) AS Profit,
                            CASE 
                                WHEN SB.TTLValue > 0 
                                THEN ((SB.TTLValue - (SB.QtyLose * SB.PerUnitCostLose)) / SB.TTLValue) * 100 
                                ELSE 0 
                            END AS ProfitPercentage
                        FROM SalesBody SB
                        INNER JOIN Products P ON SB.ProductId = P.ProductID
                        INNER JOIN Companies C ON P.CompanyID = C.CompanyID
                        INNER JOIN ProductGroups PG ON P.CompanyID = PG.CompanyId AND P.GroupId = PG.GroupId
                        LEFT JOIN Packings PK ON SB.PackingId = PK.PackingId
                        WHERE SB.SaleId = @SaleId
                        ORDER BY C.CompanyName, P.ProductName";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SaleId", saleId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading invoice detail data: " + ex.Message);
            }

            return dt;
        }

        private void ClearForm()
        {
            saledatetxtbox.Clear();
            cstmrnametxtbox.Clear();
            sectornametxtbox.Clear();
            saleId = 0;
        }
    }
}