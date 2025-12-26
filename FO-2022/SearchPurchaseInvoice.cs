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
    public partial class SearchPurchaseInvoice : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private SqlConnection connection;
        private DataTable purchasesDataTable;
        private int selectedPurchaseId = 0;

        public SearchPurchaseInvoice()
        {
            InitializeComponent();
            InitializeDatabase();
            SetupDataGridView();
            LoadPurchasesData();
        }

        private void InitializeDatabase()
        {
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database connection failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupDataGridView()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.AllowUserToAddRows = false;

            // Clear existing columns
            dataGridView1.Columns.Clear();

            // Add columns - ONLY the 4 columns you requested
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PurchaseID",
                HeaderText = "Purchase ID",
                DataPropertyName = "PurchaseID",
                Width = 100
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PurchaseDate",
                HeaderText = "Purchase Date",
                DataPropertyName = "PurchaseDate",
                Width = 120
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "VendorName",
                HeaderText = "Vendor Name",
                DataPropertyName = "PartyName",
                Width = 200
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalAmount",
                HeaderText = "Total Amount",
                DataPropertyName = "TotalAmount",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });
        }

        private void LoadPurchasesData()
        {
            try
            {
                string query = @"SELECT 
                                    p.PurchaseID,
                                    p.PurchaseDate,
                                    pt.PartyName,
                                    ISNULL(SUM(pb.TTLValue), 0) as TotalAmount
                                FROM Purchase p
                                LEFT JOIN Parties pt ON p.VendorID = pt.PartyID
                                LEFT JOIN PurchasesBody pb ON p.PurchaseID = pb.PurchaseId AND pb.IsDeleted = 0
                                GROUP BY p.PurchaseID, p.PurchaseDate, pt.PartyName
                                ORDER BY p.PurchaseDate DESC, p.PurchaseID DESC";

                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                purchasesDataTable = new DataTable();
                da.Fill(purchasesDataTable);

                dataGridView1.DataSource = purchasesDataTable;

                // Show count of records loaded
                //lblRecordCount.Text = $"Total Records: {purchasesDataTable.Rows.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading purchases: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                string query = @"SELECT 
                                    p.PurchaseID,
                                    p.PurchaseDate,
                                    pt.PartyName,
                                    ISNULL(SUM(pb.TTLValue), 0) as TotalAmount
                                FROM Purchase p
                                LEFT JOIN Parties pt ON p.VendorID = pt.PartyID
                                LEFT JOIN PurchasesBody pb ON p.PurchaseID = pb.PurchaseId AND pb.IsDeleted = 0";

                List<string> conditions = new List<string>();
                List<SqlParameter> parameters = new List<SqlParameter>();

                // Date filter
                if (fromdatepicker.Checked && todatepicker.Checked)
                {
                    conditions.Add("p.PurchaseDate BETWEEN @FromDate AND @ToDate");
                    parameters.Add(new SqlParameter("@FromDate", fromdatepicker.Value.Date));
                    parameters.Add(new SqlParameter("@ToDate", todatepicker.Value.Date.AddDays(1).AddSeconds(-1)));
                }
                else if (fromdatepicker.Checked)
                {
                    conditions.Add("p.PurchaseDate >= @FromDate");
                    parameters.Add(new SqlParameter("@FromDate", fromdatepicker.Value.Date));
                }
                else if (todatepicker.Checked)
                {
                    conditions.Add("p.PurchaseDate <= @ToDate");
                    parameters.Add(new SqlParameter("@ToDate", todatepicker.Value.Date.AddDays(1).AddSeconds(-1)));
                }

                // Purchase ID filter
                if (!string.IsNullOrEmpty(searchpurchaseidtxtbox.Text))
                {
                    if (int.TryParse(searchpurchaseidtxtbox.Text, out int purchaseId))
                    {
                        conditions.Add("p.PurchaseID = @PurchaseID");
                        parameters.Add(new SqlParameter("@PurchaseID", purchaseId));
                    }
                }

                // Add conditions to query
                if (conditions.Count > 0)
                {
                    query += " WHERE " + string.Join(" AND ", conditions);
                }

                // Add GROUP BY and ORDER BY
                query += " GROUP BY p.PurchaseID, p.PurchaseDate, pt.PartyName";

                // Order by
                if (radioButtonorderbydate.Checked)
                {
                    query += " ORDER BY p.PurchaseDate DESC, p.PurchaseID DESC";
                }
                else if (radioButtonorderbyid.Checked)
                {
                    query += " ORDER BY p.PurchaseID DESC";
                }

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddRange(parameters.ToArray());

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                purchasesDataTable.Clear();
                da.Fill(purchasesDataTable);

                dataGridView1.DataSource = purchasesDataTable;
                //lblRecordCount.Text = $"Total Records: {purchasesDataTable.Rows.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error applying filters: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void selectbtn_Click(object sender, EventArgs e)
        {
            if (selectedPurchaseId > 0)
            {
                PurchaseInvoice purchaseForm = new PurchaseInvoice(selectedPurchaseId);
                purchaseForm.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select a purchase invoice first", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void refreshbtn_Click(object sender, EventArgs e)
        {
            fromdatepicker.Checked = false;
            todatepicker.Checked = false;
            searchpurchaseidtxtbox.Clear();
            radioButtonorderbydate.Checked = true;
            LoadPurchasesData();
            selectedPurchaseId = 0;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                if (row.Cells["PurchaseID"].Value != null)
                {
                    selectedPurchaseId = Convert.ToInt32(row.Cells["PurchaseID"].Value);
                    // Highlight the selected row
                    dataGridView1.ClearSelection();
                    row.Selected = true;
                }
            }
        }

        private void fromdatepicker_ValueChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void todatepicker_ValueChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void searchpurchaseidtxtbox_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void radioButtonorderbydate_CheckedChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void radioButtonorderbyid_CheckedChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void cancelbtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Empty handler
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error closing connection: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            base.OnFormClosing(e);
        }

        // Add this label to your form designer
        //private System.Windows.Forms.Label lblRecordCount;

        // Add this line in InitializeComponent() method in designer file:
        // this.lblRecordCount = new System.Windows.Forms.Label();
    }
}