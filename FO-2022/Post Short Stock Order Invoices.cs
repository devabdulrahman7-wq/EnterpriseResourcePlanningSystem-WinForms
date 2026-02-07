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
    public partial class Post_Short_Stock_Order_Invoices : Form
    {
        // Database connection string
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // DataTable to hold sales data
        private DataTable salesDataTable;

        public Post_Short_Stock_Order_Invoices()
        {
            InitializeComponent();

            // Initialize DataTable
            salesDataTable = new DataTable();

            // Configure DataGridView
            ConfigureDataGridView();
        }

        private void ConfigureDataGridView()
        {
            // Set up DataGridView properties
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;

            // Add columns to DataGridView
            dataGridView1.Columns.Clear();

            // Add columns
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colSaleId",
                HeaderText = "Sale ID",
                DataPropertyName = "SaleId",
                Width = 80
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colSaleDate",
                HeaderText = "Sale Date",
                DataPropertyName = "SaleDate",
                Width = 100
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colCustomerId",
                HeaderText = "Customer ID",
                DataPropertyName = "CustomerId",
                Width = 100
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colCustomerName",
                HeaderText = "Customer Name",
                DataPropertyName = "CustomerName",
                Width = 150
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colTotalAmount",
                HeaderText = "Total Amount",
                DataPropertyName = "ReceivedAmount",
                Width = 100
            });

            dataGridView1.Columns.Add(new DataGridViewCheckBoxColumn()
            {
                Name = "colIsPosted",
                HeaderText = "Is Posted",
                DataPropertyName = "IsPosted",
                Width = 80
            });
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void datepcker_ValueChanged(object sender, EventArgs e)
        {
            // Clear previous data when date changes
            ClearData();
        }

        private void searchdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Get selected date
                DateTime selectedDate = datepcker.Value.Date;

                // Fetch sales data for selected date
                FetchSalesData(selectedDate);

                // Update totals
                UpdateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FetchSalesData(DateTime selectedDate)
        {
            try
            {
                // Clear previous data
                salesDataTable.Clear();

                // SQL query to fetch sales data for selected date
                string query = @"
                    SELECT 
                        s.SaleId,
                        s.SaleDate,
                        s.CustomerId,
                        s.CustomerName,
                        s.ReceivedAmount,
                        s.IsPosted,
                        p.PartyName
                    FROM Sales s
                    LEFT JOIN Parties p ON s.CustomerId = p.PartyID
                    WHERE CAST(s.SaleDate AS DATE) = @SelectedDate
                    ORDER BY s.SaleId";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SelectedDate", selectedDate);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(salesDataTable);
                        }
                    }
                }

                // Bind data to DataGridView
                dataGridView1.DataSource = salesDataTable;

                // If no data found, show message
                if (salesDataTable.Rows.Count == 0)
                {
                    MessageBox.Show($"No sales found for {selectedDate.ToShortDateString()}",
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database error: {sqlEx.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTotals()
        {
            try
            {
                // Count total number of sales
                int totalSales = salesDataTable.Rows.Count;
                noofsalestxtbox.Text = totalSales.ToString();

                // Calculate total net sales
                decimal totalNetSales = 0;
                foreach (DataRow row in salesDataTable.Rows)
                {
                    if (row["ReceivedAmount"] != DBNull.Value)
                    {
                        totalNetSales += Convert.ToDecimal(row["ReceivedAmount"]);
                    }
                }

                totalnetsalestxtbox.Text = totalNetSales.ToString("N2");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error calculating totals: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearData()
        {
            salesDataTable.Clear();
            dataGridView1.DataSource = null;
            noofsalestxtbox.Text = "0";
            totalnetsalestxtbox.Text = "0.00";
        }

        private void slctbtn_Click(object sender, EventArgs e)
        {
            // Select all rows in DataGridView
            dataGridView1.SelectAll();
        }

        private void noofsalestxtbox_TextChanged(object sender, EventArgs e)
        {
            // This event handler is already implemented
        }

        private void totalnetsalestxtbox_TextChanged(object sender, EventArgs e)
        {
            // This event handler is already implemented
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // You can add additional functionality here if needed
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void postsalebtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Get selected rows
                var selectedRows = dataGridView1.SelectedRows;

                if (selectedRows.Count == 0)
                {
                    MessageBox.Show("Please select at least one sale to post.",
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Confirm with user
                DialogResult result = MessageBox.Show(
                    $"Are you sure you want to post {selectedRows.Count} selected sale(s)?",
                    "Confirm Posting",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Update selected sales to posted status
                    UpdateSalesStatus(selectedRows);

                    // Refresh data
                    DateTime selectedDate = datepcker.Value.Date;
                    FetchSalesData(selectedDate);
                    UpdateTotals();

                    MessageBox.Show($"{selectedRows.Count} sale(s) posted successfully!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error posting sales: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateSalesStatus(DataGridViewSelectedRowCollection selectedRows)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    foreach (DataGridViewRow row in selectedRows)
                    {
                        if (row.DataBoundItem is DataRowView rowView)
                        {
                            int saleId = Convert.ToInt32(rowView["SaleId"]);

                            string updateQuery = @"
                                UPDATE Sales 
                                SET IsPosted = 1 
                                WHERE SaleId = @SaleId";

                            using (SqlCommand command = new SqlCommand(updateQuery, connection))
                            {
                                command.Parameters.AddWithValue("@SaleId", saleId);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update sales status: {ex.Message}");
            }
        }

        // Additional helper methods
        private void ExportToExcel()
        {
            // You can implement export functionality here if needed
        }

        private void PrintReport()
        {
            // You can implement print functionality here if needed
        }
    }
}