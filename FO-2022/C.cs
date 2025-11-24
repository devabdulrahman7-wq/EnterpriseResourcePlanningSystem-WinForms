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
    public partial class SaleInvoice : Form
    {
        // Database connection string from app.config
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private SqlConnection connection;
        private DataTable productsDataTable;
        private decimal grossTotal = 0;
        private decimal discountTotal = 0;
        private decimal salesTaxTotal = 0;
        private decimal netTotal = 0;

        public SaleInvoice()
        {
            InitializeComponent();
            InitializeDatabase();
            InitializeDataGridView();
            AutoGenerateSaleID();
            SetCurrentDate();
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

        private void InitializeDataGridView()
        {
            productsDataTable = new DataTable();
            productsDataTable.Columns.Add("Pro ID", typeof(string));
            productsDataTable.Columns.Add("Product Name", typeof(string));
            productsDataTable.Columns.Add("Pack Name", typeof(string));
            productsDataTable.Columns.Add("Pack", typeof(int));
            productsDataTable.Columns.Add("Qty(P)", typeof(int));
            productsDataTable.Columns.Add("Qty(L)", typeof(int));
            productsDataTable.Columns.Add("Bns(L)", typeof(int));
            productsDataTable.Columns.Add("Price", typeof(decimal));
            productsDataTable.Columns.Add("Disc%", typeof(decimal));
            productsDataTable.Columns.Add("Disc 2%", typeof(decimal));
            productsDataTable.Columns.Add("Disc.Val", typeof(decimal));
            productsDataTable.Columns.Add("S-T%", typeof(decimal));
            productsDataTable.Columns.Add("S-T Val", typeof(decimal));
            productsDataTable.Columns.Add("TTL Value", typeof(decimal));

            dataGridView1.DataSource = productsDataTable;
            LoadPackingsComboBox();
        }

        private void LoadPackingsComboBox()
        {
            try
            {
                string query = "SELECT PackingId, PackingName FROM Packings";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader reader = cmd.ExecuteReader();

                var packNames = new List<string>();
                while (reader.Read())
                {
                    packNames.Add(reader["PackingName"].ToString());
                }
                reader.Close();

                // Set items for the ComboBox column - USING YOUR CORRECTED CODE
                var comboCol = dataGridView1.Columns["Pack Name"] as DataGridViewComboBoxColumn;
                if (comboCol != null)
                {
                    comboCol.Items.Clear();
                    comboCol.Items.AddRange(packNames.ToArray());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading packings: " + ex.Message);
            }
        }

        private void AutoGenerateSaleID()
        {
            try
            {
                string query = "SELECT ISNULL(MAX(SaleId), 0) + 1 FROM Sales";
                SqlCommand cmd = new SqlCommand(query, connection);
                object result = cmd.ExecuteScalar();
                saleidtxtbox.Text = result.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating Sale ID: " + ex.Message);
            }
        }

        private void SetCurrentDate()
        {
            dateTimePicker1.Value = DateTime.Now;
        }

        // Customer ID Search
        private void customeridsearch_Click(object sender, EventArgs e)
        {
            SearchCustomer();
        }

        private void SearchCustomer()
        {
            try
            {
                string query = "SELECT PartyID, PartyName, City FROM Parties WHERE PartyID = @PartyID";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@PartyID", customeridtxtbox.Text);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    customernametxtbox.Text = reader["PartyName"].ToString();
                    searchtowntxtbox.Text = reader["City"].ToString();
                    LoadCustomerPreviousBalance();
                }
                else
                {
                    MessageBox.Show("Customer not found!");
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching customer: " + ex.Message);
            }
        }

        private void LoadCustomerPreviousBalance()
        {
            try
            {
                string query = @"SELECT ISNULL(SUM(NetValue - ReceivedAmount), 0) as Balance 
                                FROM Sales WHERE CustomerId = @CustomerId AND IsPosted = 1";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@CustomerId", customeridtxtbox.Text);

                object result = cmd.ExecuteScalar();
                prevbaltxtbox.Text = result?.ToString() ?? "0";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading previous balance: " + ex.Message);
                prevbaltxtbox.Text = "0";
            }
        }

        // Salesman Search
        private void salesmanudsearch_Click(object sender, EventArgs e)
        {
            SearchSalesman();
        }

        private void SearchSalesman()
        {
            try
            {
                string query = "SELECT SalesmanID, SalesmanName FROM Salesmen WHERE SalesmanID = @SalesmanID";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@SalesmanID", salesmenidtxtbox.Text);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    salesmennametxtbox.Text = reader["SalesmanName"].ToString();
                }
                else
                {
                    MessageBox.Show("Salesman not found!");
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching salesman: " + ex.Message);
            }
        }

        // Order ID Search
        private void orderidsearch_Click(object sender, EventArgs e)
        {
            SearchOrder();
        }

        private void SearchOrder()
        {
            try
            {
                string query = @"SELECT so.OrderId, so.OrderDate, so.CustomerId, p.PartyName 
                                FROM SaleOrder so 
                                INNER JOIN Parties p ON so.CustomerId = p.PartyID 
                                WHERE so.OrderId = @OrderId";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@OrderId", orderidtxtbox.Text);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    orderdatetxtbo.Text = Convert.ToDateTime(reader["OrderDate"]).ToString("yyyy-MM-dd");
                    customeridtxtbox.Text = reader["CustomerId"].ToString();
                    customernametxtbox.Text = reader["PartyName"].ToString();
                    LoadOrderItems();
                }
                else
                {
                    MessageBox.Show("Order not found!");
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching order: " + ex.Message);
            }
        }

        private void LoadOrderItems()
        {
            try
            {
                productsDataTable.Clear();

                string query = @"SELECT sob.ProductId, p.ProductName, pk.PackingName, sob.Multiplier, 
                                sob.QtyPack, sob.QtyLose, sob.BnsLose, sob.PricePack, sob.DiscRatio,
                                sob.STaxRatio, sob.TTLValue
                                FROM SaleOrderBody sob
                                INNER JOIN Products p ON sob.ProductId = p.ProductID
                                INNER JOIN Packings pk ON sob.PackingId = pk.PackingId
                                WHERE sob.OrderId = @OrderId AND sob.IsDeleted = 0";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@OrderId", orderidtxtbox.Text);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    productsDataTable.Rows.Add(
                        reader["ProductId"].ToString(),
                        reader["ProductName"].ToString(),
                        reader["PackingName"].ToString(),
                        reader["Multiplier"],
                        reader["QtyPack"],
                        reader["QtyLose"],
                        reader["BnsLose"],
                        reader["PricePack"],
                        reader["DiscRatio"],
                        0, // Disc 2%
                        0, // Disc Value
                        reader["STaxRatio"],
                        0, // S-T Value
                        reader["TTLValue"]
                    );
                }
                reader.Close();

                CalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading order items: " + ex.Message);
            }
        }

        // Save Button
        private void savebtn_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                SaveSaleInvoice();
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrEmpty(customeridtxtbox.Text))
            {
                MessageBox.Show("Please select a customer!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (productsDataTable.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one product!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void SaveSaleInvoice()
        {
            SqlTransaction transaction = null;

            try
            {
                transaction = connection.BeginTransaction();

                // Save to Sales table
                string salesQuery = @"INSERT INTO Sales (SaleId, SaleDate, BillNo, CustomerId, SalesmanId, 
                                    Narration, Disc2Ratio, SpecialDiscount, ReceivedAmount, 
                                    PreviousCredit, IsPosted, UserNo, DueDate, IsPendingRate, WithPartyDescription, 
                                    FreightExpenses, GrossValue, DiscountValue, SalesTaxValue, NetValue)
                                    VALUES (@SaleId, @SaleDate, @BillNo, @CustomerId, @SalesmanId, 
                                    @Narration, @Disc2Ratio, @SpecialDiscount, @ReceivedAmount, 
                                    @PreviousCredit, 1, @UserNo, @DueDate, @IsPendingRate, @WithPartyDescription, 
                                    @FreightExpenses, @GrossValue, @DiscountValue, @SalesTaxValue, @NetValue)";

                SqlCommand salesCmd = new SqlCommand(salesQuery, connection, transaction);
                salesCmd.Parameters.AddWithValue("@SaleId", Convert.ToInt32(saleidtxtbox.Text));
                salesCmd.Parameters.AddWithValue("@SaleDate", dateTimePicker1.Value);
                salesCmd.Parameters.AddWithValue("@BillNo", string.IsNullOrEmpty(billnootxtbox.Text) ? (object)DBNull.Value : billnootxtbox.Text);
                salesCmd.Parameters.AddWithValue("@CustomerId", customeridtxtbox.Text);
                salesCmd.Parameters.AddWithValue("@SalesmanId", string.IsNullOrEmpty(salesmenidtxtbox.Text) ? (object)DBNull.Value : salesmenidtxtbox.Text);
                salesCmd.Parameters.AddWithValue("@Narration", string.IsNullOrEmpty(narrationtxtbox.Text) ? "" : narrationtxtbox.Text);
                salesCmd.Parameters.AddWithValue("@Disc2Ratio", string.IsNullOrEmpty(disc2txtbox.Text) ? 0 : Convert.ToDecimal(disc2txtbox.Text));
                salesCmd.Parameters.AddWithValue("@SpecialDiscount", string.IsNullOrEmpty(specialdisctxtbox.Text) ? 0 : Convert.ToDecimal(specialdisctxtbox.Text));
                salesCmd.Parameters.AddWithValue("@ReceivedAmount", string.IsNullOrEmpty(cashrectxtbox.Text) ? 0 : Convert.ToDecimal(cashrectxtbox.Text));
                salesCmd.Parameters.AddWithValue("@PreviousCredit", string.IsNullOrEmpty(prevbaltxtbox.Text) ? 0 : Convert.ToDecimal(prevbaltxtbox.Text));
                salesCmd.Parameters.AddWithValue("@UserNo", 1);
                salesCmd.Parameters.AddWithValue("@DueDate", string.IsNullOrEmpty(duedatetxtbox.Text) ? (object)DBNull.Value : Convert.ToDateTime(duedatetxtbox.Text));
                salesCmd.Parameters.AddWithValue("@IsPendingRate", pendingratechkbox.Checked);
                salesCmd.Parameters.AddWithValue("@WithPartyDescription", withpartydescriptionchkbox.Checked);
                salesCmd.Parameters.AddWithValue("@FreightExpenses", string.IsNullOrEmpty(freightexpensetxtbox.Text) ? 0 : Convert.ToDecimal(freightexpensetxtbox.Text));
                salesCmd.Parameters.AddWithValue("@GrossValue", grossTotal);
                salesCmd.Parameters.AddWithValue("@DiscountValue", discountTotal);
                salesCmd.Parameters.AddWithValue("@SalesTaxValue", salesTaxTotal);
                salesCmd.Parameters.AddWithValue("@NetValue", netTotal);

                salesCmd.ExecuteNonQuery();

                // Save to SalesBody table
                foreach (DataRow row in productsDataTable.Rows)
                {
                    if (row.RowState != DataRowState.Deleted)
                    {
                        string salesBodyQuery = @"INSERT INTO SalesBody (SaleId, ProductId, PackingId, Multiplier, QtyPack, 
                                                QtyLose, BnsLose, PricePack, DiscRatio, DiscValPack, STaxRatio, 
                                                STaxValPack, TTLValue, PerUnitCostLose, PriceHistoryId, OfferDiscount)
                                                VALUES (@SaleId, @ProductId, @PackingId, @Multiplier, @QtyPack, 
                                                @QtyLose, @BnsLose, @PricePack, @DiscRatio, @DiscValPack, @STaxRatio, 
                                                @STaxValPack, @TTLValue, @PerUnitCostLose, @PriceHistoryId, @OfferDiscount)";

                        SqlCommand salesBodyCmd = new SqlCommand(salesBodyQuery, connection, transaction);
                        salesBodyCmd.Parameters.AddWithValue("@SaleId", Convert.ToInt32(saleidtxtbox.Text));
                        salesBodyCmd.Parameters.AddWithValue("@ProductId", row["Pro ID"]?.ToString() ?? "");
                        salesBodyCmd.Parameters.AddWithValue("@PackingId", GetPackingIdByName(row["Pack Name"]?.ToString() ?? ""));
                        salesBodyCmd.Parameters.AddWithValue("@Multiplier", Convert.ToInt32(row["Pack"]));
                        salesBodyCmd.Parameters.AddWithValue("@QtyPack", Convert.ToInt32(row["Qty(P)"]));
                        salesBodyCmd.Parameters.AddWithValue("@QtyLose", Convert.ToInt32(row["Qty(L)"]));
                        salesBodyCmd.Parameters.AddWithValue("@BnsLose", Convert.ToInt32(row["Bns(L)"]));
                        salesBodyCmd.Parameters.AddWithValue("@PricePack", Convert.ToDecimal(row["Price"]));
                        salesBodyCmd.Parameters.AddWithValue("@DiscRatio", Convert.ToDecimal(row["Disc%"]));
                        salesBodyCmd.Parameters.AddWithValue("@DiscValPack", Convert.ToDecimal(row["Disc.Val"]));
                        salesBodyCmd.Parameters.AddWithValue("@STaxRatio", Convert.ToDecimal(row["S-T%"]));
                        salesBodyCmd.Parameters.AddWithValue("@STaxValPack", Convert.ToDecimal(row["S-T Val"]));
                        salesBodyCmd.Parameters.AddWithValue("@TTLValue", Convert.ToDecimal(row["TTL Value"]));
                        salesBodyCmd.Parameters.AddWithValue("@PerUnitCostLose", 0);
                        salesBodyCmd.Parameters.AddWithValue("@PriceHistoryId", 1);
                        salesBodyCmd.Parameters.AddWithValue("@OfferDiscount", 0);

                        salesBodyCmd.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
                MessageBox.Show("Sale invoice saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                AutoGenerateSaleID();
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                MessageBox.Show("Error saving sale invoice: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetPackingIdByName(string packingName)
        {
            try
            {
                string query = "SELECT PackingId FROM Packings WHERE PackingName = @PackingName";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@PackingName", packingName);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 1;
            }
            catch
            {
                return 1;
            }
        }

        // Calculate Totals
        private void CalculateTotals()
        {
            try
            {
                grossTotal = 0;
                discountTotal = 0;
                salesTaxTotal = 0;

                foreach (DataRow row in productsDataTable.Rows)
                {
                    if (row.RowState != DataRowState.Deleted)
                    {
                        grossTotal += Convert.ToDecimal(row["TTL Value"]);
                        discountTotal += Convert.ToDecimal(row["Disc.Val"]);
                        salesTaxTotal += Convert.ToDecimal(row["S-T Val"]);
                    }
                }

                decimal disc2 = string.IsNullOrEmpty(disc2txtbox.Text) ? 0 : Convert.ToDecimal(disc2txtbox.Text);
                decimal specialDisc = string.IsNullOrEmpty(specialdisctxtbox.Text) ? 0 : Convert.ToDecimal(specialdisctxtbox.Text);
                decimal freight = string.IsNullOrEmpty(freightexpensetxtbox.Text) ? 0 : Convert.ToDecimal(freightexpensetxtbox.Text);

                netTotal = grossTotal - discountTotal - (grossTotal * disc2 / 100) - specialDisc + freight + salesTaxTotal;

                grosstxtbox.Text = grossTotal.ToString("N2");
                discounttxtbox.Text = discountTotal.ToString("N2");
                salestxtbox.Text = salesTaxTotal.ToString("N2");
                netvaluetxtbox.Text = netTotal.ToString("N2");

                // Calculate balance
                decimal cashRec = string.IsNullOrEmpty(cashrectxtbox.Text) ? 0 : Convert.ToDecimal(cashrectxtbox.Text);
                decimal prevBal = string.IsNullOrEmpty(prevbaltxtbox.Text) ? 0 : Convert.ToDecimal(prevbaltxtbox.Text);
                decimal balance = prevBal + netTotal - cashRec;
                textBox12.Text = balance.ToString("N2");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating totals: " + ex.Message);
            }
        }

        // Clear Form
        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            try
            {
                customeridtxtbox.Clear();
                customernametxtbox.Clear();
                searchtowntxtbox.Clear();
                salesmenidtxtbox.Clear();
                salesmennametxtbox.Clear();
                orderidtxtbox.Clear();
                orderdatetxtbo.Clear();
                billnootxtbox.Clear();
                productsDataTable.Clear();
                narrationtxtbox.Clear();
                duedatetxtbox.Clear();
                grosstxtbox.Clear();
                discounttxtbox.Clear();
                salestxtbox.Clear();
                disc2txtbox.Clear();
                specialdisctxtbox.Clear();
                freightexpensetxtbox.Clear();
                netvaluetxtbox.Clear();
                prevbaltxtbox.Clear();
                cashrectxtbox.Clear();
                textBox12.Clear();
                pendingratechkbox.Checked = false;
                withpartydescriptionchkbox.Checked = false;
                withbuiltychkbox.Checked = false;
                uploadsaledocchkbox.Checked = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error clearing form: " + ex.Message);
            }
        }

        // DataGridView Events
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                CalculateRowTotal(e.RowIndex);
                CalculateTotals();
            }
        }

        private void CalculateRowTotal(int rowIndex)
        {
            try
            {
                DataGridViewRow row = dataGridView1.Rows[rowIndex];

                if (row.Cells["Qty(P)"].Value != null && row.Cells["Price"].Value != null)
                {
                    int qtyPack = Convert.ToInt32(row.Cells["Qty(P)"].Value);
                    int qtyLose = row.Cells["Qty(L)"].Value != null ? Convert.ToInt32(row.Cells["Qty(L)"].Value) : 0;
                    decimal price = Convert.ToDecimal(row.Cells["Price"].Value);
                    decimal discPercent = row.Cells["Disc%"].Value != null ? Convert.ToDecimal(row.Cells["Disc%"].Value) : 0;
                    decimal staxPercent = row.Cells["S-T%"].Value != null ? Convert.ToDecimal(row.Cells["S-T%"].Value) : 0;

                    decimal totalBefore = (qtyPack + (decimal)qtyLose / 100) * price;
                    decimal discValue = totalBefore * discPercent / 100;
                    decimal staxValue = (totalBefore - discValue) * staxPercent / 100;
                    decimal finalTotal = totalBefore - discValue + staxValue;

                    row.Cells["Disc.Val"].Value = discValue;
                    row.Cells["S-T Val"].Value = staxValue;
                    row.Cells["TTL Value"].Value = finalTotal;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating row total: " + ex.Message);
            }
        }

        // Remove selected row from DataGridView
        private void removebtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        if (!row.IsNewRow)
                        {
                            dataGridView1.Rows.Remove(row);
                        }
                    }
                    CalculateTotals();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error removing row: " + ex.Message);
            }
        }

        // TextBox Change Events for Auto Calculation
        private void cashrectxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateTotals();
        }

        private void disc2txtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateTotals();
        }

        private void specialdisctxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateTotals();
        }

        private void freightexpensetxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateTotals();
        }

        // Existing methods
        private void checkBox1_CheckedChanged(object sender, EventArgs e) { }
        private void label12_Click(object sender, EventArgs e) { }
        private void button6_Click(object sender, EventArgs e) { }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e) { }
        private void SaleInvoice_Load(object sender, EventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openpendingbtn_Click(object sender, EventArgs e)
        {
            Search_Pending_Invoice spi = new Search_Pending_Invoice();
            spi.Show();
            this.Close();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
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
                MessageBox.Show("Error closing connection: " + ex.Message);
            }
            base.OnFormClosing(e);
        }
    }
}