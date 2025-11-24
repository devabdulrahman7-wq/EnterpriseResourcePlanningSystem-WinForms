using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Sale_Order : Form
    {
        string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Sale_Order()
        {
            InitializeComponent();
        }

        private void Sale_Order_Load(object sender, EventArgs e)
        {
            LoadAllOrders();
        }

        // 🔹 Load All Orders
        private void LoadAllOrders()
        {
            string query = @"
                SELECT SO.*, SOB.*
                FROM [POS_1.2].[dbo].[SaleOrder1st] SO
                INNER JOIN [POS_1.2].[dbo].[SaleOrderBody] SOB
                    ON SO.OrderId = SOB.OrderId
                ORDER BY SO.OrderDate DESC";

            BindDataToGrid(query);
        }

        // 🔹 Bind Data to DataGridView
        private void BindDataToGrid(string query)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }

        // 🔹 Save Order (Insert Master + Details)
        private void savebtn_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    // Insert Master
                    string insertMaster = @"
                    INSERT INTO [POS_1.2].[dbo].[SaleOrder1st]
                    (OrderDate, BillNo, CustomerId, SectorID, SalesmanId, IsStockist, OldIsStockist, Narration, Disc2Ratio, FTaxRatio, SpecialDiscount, InvOfferDiscount, InvOfferSTax, InvOfferFTax, ReceivedAmount, PreviousCredit, InvoiceNotes, IsPosted, UserNo, BiltyNo, TransportName, DistributorPartyId, VehicleNo, DriverName, FreightExpenses, DispatchedDate, IsParchoon)
                    VALUES
                    (@OrderDate, @BillNo, @CustomerId, @SectorID, @SalesmanId, @IsStockist, @OldIsStockist, @Narration, @Disc2Ratio, @FTaxRatio, @SpecialDiscount, @InvOfferDiscount, @InvOfferSTax, @InvOfferFTax, @ReceivedAmount, @PreviousCredit, @InvoiceNotes, @IsPosted, @UserNo, @BiltyNo, @TransportName, @DistributorPartyId, @VehicleNo, @DriverName, @FreightExpenses, @DispatchedDate, @IsParchoon);
                    SELECT SCOPE_IDENTITY();";

                    SqlCommand cmd = new SqlCommand(insertMaster, con, tran);
                    cmd.Parameters.AddWithValue("@OrderDate", orderdatepcker.Value);
                    cmd.Parameters.AddWithValue("@BillNo", billnotxtbox.Text);
                    cmd.Parameters.AddWithValue("@CustomerId", customeridtxtbox.Text);
                    // Add remaining parameters similarly
                    int newOrderId = Convert.ToInt32(cmd.ExecuteScalar());

                    // Insert Details
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow) continue;

                        string insertDetail = @"
                        INSERT INTO [POS_1.2].[dbo].[SaleOrderBody]
                        (OrderId, ProductId, PackingId, Multiplier, QtyPack, QtyLose, BnsLose, PricePack, DiscRatio, DiscValPack, DiscValLose, STaxRatio, STaxValPack, IsValueB4Ratio, IsSTaxOnBns, IsFTaxExempted, TTLValue, TTLSTax, TTLFTax, ExtraSTaxBns, ExtraFTaxBns, PerUnitCostLose, IsFreeProduct, PriceHistoryId, OfferDiscount, IsDeleted)
                        VALUES
                        (@OrderId, @ProductId, @PackingId, @Multiplier, @QtyPack, @QtyLose, @BnsLose, @PricePack, @DiscRatio, @DiscValPack, @DiscValLose, @STaxRatio, @STaxValPack, @IsValueB4Ratio, @IsSTaxOnBns, @IsFTaxExempted, @TTLValue, @TTLSTax, @TTLFTax, @ExtraSTaxBns, @ExtraFTaxBns, @PerUnitCostLose, @IsFreeProduct, @PriceHistoryId, @OfferDiscount, @IsDeleted)";

                        SqlCommand cmdDetail = new SqlCommand(insertDetail, con, tran);
                        cmdDetail.Parameters.AddWithValue("@OrderId", newOrderId);
                        cmdDetail.Parameters.AddWithValue("@ProductId", row.Cells["ProductId"].Value);
                        cmdDetail.Parameters.AddWithValue("@PackingId", row.Cells["PackingId"].Value);
                        cmdDetail.Parameters.AddWithValue("@Multiplier", row.Cells["Multiplier"].Value);
                        cmdDetail.Parameters.AddWithValue("@QtyPack", row.Cells["QtyPack"].Value);
                        cmdDetail.Parameters.AddWithValue("@QtyLose", row.Cells["QtyLose"].Value);
                        // Add remaining params
                        cmdDetail.ExecuteNonQuery();
                    }

                    tran.Commit();
                    MessageBox.Show("Order Saved Successfully!");
                    LoadAllOrders();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        // 🔹 Remove Order
        private void removebtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(orderidtxtbox.Text)) return;
            int orderId = Convert.ToInt32(orderidtxtbox.Text);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    SqlCommand cmdDetail = new SqlCommand("DELETE FROM [POS_1.2].[dbo].[SaleOrderBody] WHERE OrderId=@OrderId", con, tran);
                    cmdDetail.Parameters.AddWithValue("@OrderId", orderId);
                    cmdDetail.ExecuteNonQuery();

                    SqlCommand cmdMaster = new SqlCommand("DELETE FROM [POS_1.2].[dbo].[SaleOrder1st] WHERE OrderId=@OrderId", con, tran);
                    cmdMaster.Parameters.AddWithValue("@OrderId", orderId);
                    cmdMaster.ExecuteNonQuery();

                    tran.Commit();
                    MessageBox.Show("Order Deleted Successfully!");
                    LoadAllOrders();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        // 🔹 Clear Form
        private void clearbtn_Click(object sender, EventArgs e)
        {
            billnotxtbox.Clear();
            customeridtxtbox.Clear();
            dataGridView1.DataSource = null;
        }

        // 🔹 Open Order
        private void openbtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(orderidtxtbox.Text)) return;
            int orderId = Convert.ToInt32(orderidtxtbox.Text);
            string query = $@"
                SELECT SO.*, SOB.*
                FROM [POS_1.2].[dbo].[SaleOrder1st] SO
                INNER JOIN [POS_1.2].[dbo].[SaleOrderBody] SOB ON SO.OrderId = SOB.OrderId
                WHERE SO.OrderId = {orderId}";
            BindDataToGrid(query);
        }

        // 🔹 Close Form
        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // 🔹 Search by BillNo
        private void btnSearchBill_Click(object sender, EventArgs e)
        {
            string billNo = billnotxtbox.Text.Trim();
            if (string.IsNullOrEmpty(billNo)) return;

            string query = $@"
                SELECT SO.*, SOB.*
                FROM [POS_1.2].[dbo].[SaleOrder1st] SO
                INNER JOIN [POS_1.2].[dbo].[SaleOrderBody] SOB ON SO.OrderId = SOB.OrderId
                WHERE SO.BillNo LIKE '%{billNo}%'";
            BindDataToGrid(query);
        }
        // 🔹 Filter by Customer
        private void btnFilterCustomer_Click(object sender, EventArgs e)
        {
            string customerId = customeridtxtbox.Text.Trim();
            if (string.IsNullOrEmpty(customerId)) return;

            string query = $@"
                SELECT SO.*, SOB.*
                FROM [POS_1.2].[dbo].[SaleOrder1st] SO
                INNER JOIN [POS_1.2].[dbo].[SaleOrderBody] SOB ON SO.OrderId = SOB.OrderId
                WHERE SO.CustomerId = '{customerId}'";
            BindDataToGrid(query);
        }
        // 🔹 Filter by Date
        private void btnFilterDate_Click(object sender, EventArgs e)
        {
            DateTime date = orderdatepcker.Value.Date;
            string query = $@"
                SELECT SO.*, SOB.*
                FROM [POS_1.2].[dbo].[SaleOrder1st] SO
                INNER JOIN [POS_1.2].[dbo].[SaleOrderBody] SOB ON SO.OrderId = SOB.OrderId
                WHERE SO.OrderDate = '{date:yyyy-MM-dd}'";
            BindDataToGrid(query);
        }
    }
}
