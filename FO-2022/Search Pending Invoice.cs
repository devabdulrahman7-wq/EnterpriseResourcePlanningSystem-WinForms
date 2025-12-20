using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace FO_2022
{
    public partial class Search_Pending_Invoice : Form
    {
        private string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private int selectedPendingId = 0;

        public Search_Pending_Invoice()
        {
            InitializeComponent();
            LoadPendingInvoices();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            FilterPendingInvoices();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            FilterPendingInvoices();
        }

        private void LoadPendingInvoices()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    string query = @"
                    SELECT 
                        p.SaleId AS [Pending ID],
                        p.SaleDate AS [Date],
                        ISNULL(p.BillNo, '') AS [Bill No],
                        pt.PartyName AS [Customer],
                        p.ReceivedAmount AS [Amount],
                        p.CustomerId,
                        p.SalesmanID,
                        p.Narration
                    FROM Pendings p
                    LEFT JOIN Parties pt ON p.CustomerId = pt.PartyID
                    WHERE p.IsPosted = 0 -- Only pending invoices
                    ORDER BY p.SaleDate DESC, p.SaleId DESC";

                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.DataSource = dt;

                    // Hide extra columns
                    if (dataGridView1.Columns.Contains("CustomerId"))
                        dataGridView1.Columns["CustomerId"].Visible = false;
                    if (dataGridView1.Columns.Contains("SalesmanID"))
                        dataGridView1.Columns["SalesmanID"].Visible = false;
                    if (dataGridView1.Columns.Contains("Narration"))
                        dataGridView1.Columns["Narration"].Visible = false;

                    // Format columns
                    dataGridView1.Columns["Pending ID"].Width = 80;
                    dataGridView1.Columns["Date"].Width = 80;
                    dataGridView1.Columns["Bill No"].Width = 70;
                    dataGridView1.Columns["Customer"].Width = 150;
                    dataGridView1.Columns["Amount"].Width = 80;
                    dataGridView1.Columns["Amount"].DefaultCellStyle.Format = "N2";

                    // Set default dates
                    todatepcker.Value = DateTime.Today.AddDays(-30); // Last 30 days
                    todatepcker.Value = DateTime.Today;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading pending invoices: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FilterPendingInvoices()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    string query = @"
                    SELECT 
                        p.SaleId AS [Pending ID],
                        p.SaleDate AS [Date],
                        ISNULL(p.BillNo, '') AS [Bill No],
                        pt.PartyName AS [Customer],
                        p.ReceivedAmount AS [Amount],
                        p.CustomerId,
                        p.SalesmanID,
                        p.Narration
                    FROM Pendings p
                    LEFT JOIN Parties pt ON p.CustomerId = pt.PartyID
                    WHERE p.IsPosted = 0 
                    AND p.SaleDate >= @FromDate 
                    AND p.SaleDate <= @ToDate
                    ORDER BY p.SaleDate DESC, p.SaleId DESC";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@FromDate", todatepcker.Value.Date);
                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value.Date.AddDays(1)); // Include end date

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.DataSource = dt;

                    // Hide extra columns
                    if (dataGridView1.Columns.Contains("CustomerId"))
                        dataGridView1.Columns["CustomerId"].Visible = false;
                    if (dataGridView1.Columns.Contains("SalesmanID"))
                        dataGridView1.Columns["SalesmanID"].Visible = false;
                    if (dataGridView1.Columns.Contains("Narration"))
                        dataGridView1.Columns["Narration"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error filtering invoices: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Cell content click handler
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                if (row.Cells["Pending ID"].Value != null)
                {
                    selectedPendingId = Convert.ToInt32(row.Cells["Pending ID"].Value);

                    // Highlight selected row
                    foreach (DataGridViewRow r in dataGridView1.Rows)
                    {
                        r.DefaultCellStyle.BackColor = Color.White;
                    }
                    row.DefaultCellStyle.BackColor = Color.LightBlue;
                }
            }
        }

        private void selectbtn_Click(object sender, EventArgs e)
        {
            if (selectedPendingId == 0)
            {
                MessageBox.Show("Please select a pending invoice first", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Load the pending invoice in SaleInvoice form
                SaleInvoice saleInvoiceForm = Application.OpenForms["SaleInvoice"] as SaleInvoice;

                if (saleInvoiceForm == null)
                {
                    // If SaleInvoice form is not open, open it
                    saleInvoiceForm = new SaleInvoice();
                    saleInvoiceForm.Show();
                }

                // Load pending invoice data into SaleInvoice form
                LoadPendingIntoSaleInvoice(saleInvoiceForm, selectedPendingId);

                // Bring SaleInvoice form to front
                saleInvoiceForm.BringToFront();
                saleInvoiceForm.Focus();

                // Close search form
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading pending invoice: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPendingIntoSaleInvoice(SaleInvoice saleInvoiceForm, int pendingId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // Load pending header
                    string headerQuery = @"
                    SELECT 
                        p.*,
                        pt.PartyName,
                        s.SectorName,
                        t.TownName,
                        sm.SalesmanName
                    FROM Pendings p
                    LEFT JOIN Parties pt ON p.CustomerId = pt.PartyID
                    LEFT JOIN Sectors s ON pt.SectorID = s.SectorID
                    LEFT JOIN Towns t ON s.TownID = t.TownID
                    LEFT JOIN Salesmen sm ON p.SalesmanID = sm.SalesmanID
                    WHERE p.SaleId = @PendingId";

                    SqlCommand headerCmd = new SqlCommand(headerQuery, con);
                    headerCmd.Parameters.AddWithValue("@PendingId", pendingId);

                    SqlDataReader reader = headerCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        // Pass data to SaleInvoice form using a public method
                        // You'll need to add this method to your SaleInvoice form
                        saleInvoiceForm.LoadPendingInvoice(
                            pendingId,
                            Convert.ToDateTime(reader["SaleDate"]),
                            reader["BillNo"] != DBNull.Value ? Convert.ToInt32(reader["BillNo"]) : 0,
                            reader["CustomerId"].ToString(),
                            reader["PartyName"].ToString(),
                            reader["SectorName"] != DBNull.Value ? reader["SectorName"].ToString() : "",
                            reader["TownName"] != DBNull.Value ? reader["TownName"].ToString() : "",
                            reader["SalesmanID"] != DBNull.Value ? reader["SalesmanID"].ToString() : "",
                            reader["SalesmanName"] != DBNull.Value ? reader["SalesmanName"].ToString() : "",
                            reader["PreviousCredit"] != DBNull.Value ? Convert.ToDecimal(reader["PreviousCredit"]) : 0,
                            reader["ReceivedAmount"] != DBNull.Value ? Convert.ToDecimal(reader["ReceivedAmount"]) : 0,
                            reader["SpecialDiscount"] != DBNull.Value ? Convert.ToDecimal(reader["SpecialDiscount"]) : 0,
                            reader["Narration"] != DBNull.Value ? reader["Narration"].ToString() : ""
                        );
                    }
                    reader.Close();

                    // Load pending items
                    string itemsQuery = @"
                    SELECT 
                        pb.ProductId,
                        pr.ProductName,
                        pk.PackingName,
                        pb.Multiplier,
                        pb.QtyPack,
                        pb.QtyLose,
                        pb.BnsLose,
                        pb.PricePack,
                        pb.DiscRatio,
                        pb.DiscValPack,
                        pb.STaxRatio,
                        pb.STaxValPack,
                        pb.TTLValue,
                        pb.PackingId
                    FROM PendingBody pb
                    INNER JOIN Products pr ON pb.ProductId = pr.ProductID
                    INNER JOIN Packings pk ON pb.PackingId = pk.PackingId
                    WHERE pb.SaleId = @PendingId AND (pb.IsDeleted = 0 OR pb.IsDeleted IS NULL)";

                    SqlCommand itemsCmd = new SqlCommand(itemsQuery, con);
                    itemsCmd.Parameters.AddWithValue("@PendingId", pendingId);

                    // Pass DataReader to SaleInvoice form or create DataTable
                    SqlDataReader itemsReader = itemsCmd.ExecuteReader();
                    DataTable itemsTable = new DataTable();
                    itemsTable.Load(itemsReader);
                    itemsReader.Close();

                    // Pass items to SaleInvoice form
                    saleInvoiceForm.LoadPendingItems(itemsTable);

                    MessageBox.Show($"Pending invoice #{pendingId} loaded successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading pending data: " + ex.Message);
            }
        }

        private void cancelbtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void refreshbtn_Click(object sender, EventArgs e)
        {
            FilterPendingInvoices();
        }

        // Add refresh button to your form (add a button named refreshbtn)
    }
}