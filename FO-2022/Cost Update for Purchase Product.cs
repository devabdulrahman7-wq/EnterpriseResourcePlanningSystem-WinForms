//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace FO_2022
//{
//    public partial class Cost_Update_for_Purchase_Product : Form
//    {
//        public Cost_Update_for_Purchase_Product()
//        {
//            InitializeComponent();
//        }

//        private void pictureBox2_Click(object sender, EventArgs e)
//        {
//            this.Close();
//        }

//        private void closebtn_Click(object sender, EventArgs e)
//        {
//            Mainform1 mf = new Mainform1();
//            mf.Show();
//            this.Hide();
//        }

//        private void Cost_Update_for_Purchase_Product_Load(object sender, EventArgs e)
//        {

//        }




//        private void findbtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void updatebtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void purchaseidtxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void productidtxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void productnametxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void qtytxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void pricepacktxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void discountpercentagetxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void TTLvaluetxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }
//    }
//}



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
    public partial class Cost_Update_for_Purchase_Product : Form
    {
        private string connectionString;

        public Cost_Update_for_Purchase_Product()
        {
            InitializeComponent();

            // Get connection string from App.config
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
        }

        private void Cost_Update_for_Purchase_Product_Load(object sender, EventArgs e)
        {
            // Clear all textboxes on form load
            ClearForm();
        }

        private void ClearForm()
        {
            purchaseidtxtbox.Text = "";
            productidtxtbox.Text = "";
            productnametxtbox.Text = "";
            qtytxtbox.Text = "";
            pricepacktxtbox.Text = "";
            discountpercentagetxtbox.Text = "";
            TTLvaluetxtbox.Text = "";
        }

        private void findbtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(purchaseidtxtbox.Text))
            {
                MessageBox.Show("Please enter Purchase ID to find.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Query to get purchase details from PurchasesBody table
                    string query = @"
                    SELECT 
                        pb.ProductId,
                        p.ProductName,
                        pb.QtyPack,
                        pb.PricePack,
                        pb.DiscRatio,
                        pb.TTLValue
                    FROM PurchasesBody pb
                    INNER JOIN Products p ON pb.ProductId = p.ProductID
                    WHERE pb.PurchaseId = @PurchaseId 
AND pb.ProductId=@ProductId
                    AND pb.IsDeleted = 0";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PurchaseId", purchaseidtxtbox.Text);
                        cmd.Parameters.AddWithValue("@ProductId", productidtxtbox.Text);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();

                                // Fill the textboxes with data
                                //productidtxtbox.Text = reader["ProductId"].ToString();
                                productnametxtbox.Text = reader["ProductName"].ToString();
                                qtytxtbox.Text = reader["QtyPack"].ToString();
                                pricepacktxtbox.Text = reader["PricePack"].ToString();
                                discountpercentagetxtbox.Text = reader["DiscRatio"].ToString();
                                TTLvaluetxtbox.Text = reader["TTLValue"].ToString();

                                MessageBox.Show("Data fetched successfully!", "Success",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("No record found with this Purchase ID.", "Information",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                ClearForm();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void updatebtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(purchaseidtxtbox.Text) || string.IsNullOrEmpty(productidtxtbox.Text))
            {
                MessageBox.Show("Please fetch data first or enter valid Purchase ID.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate required fields
            if (string.IsNullOrEmpty(pricepacktxtbox.Text) || string.IsNullOrEmpty(discountpercentagetxtbox.Text))
            {
                MessageBox.Show("Please enter Price Pack and Discount Percentage.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Calculate TTLValue based on Qty and Price
            decimal qty, pricePack, discRatio, ttlValue;

            if (!decimal.TryParse(qtytxtbox.Text, out qty) ||
                !decimal.TryParse(pricepacktxtbox.Text, out pricePack) ||
                !decimal.TryParse(discountpercentagetxtbox.Text, out discRatio))
            {
                MessageBox.Show("Please enter valid numeric values for Qty, Price Pack and Discount.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Calculate TTLValue: (Qty * Price) - (Qty * Price * DiscRatio/100)
            ttlValue = (qty * pricePack) - (qty * pricePack * discRatio / 100);
            TTLvaluetxtbox.Text = ttlValue.ToString("F2");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Update query for PurchasesBody table
                    string updateQuery = @"
                    UPDATE PurchasesBody 
                    SET 
                        PricePack = @PricePack,
                        DiscRatio = @DiscRatio,
                        TTLValue = @TTLValue
                    WHERE PurchaseId = @PurchaseId 
                    AND ProductId = @ProductId
                    AND IsDeleted = 0";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@PurchaseId", purchaseidtxtbox.Text);
                        cmd.Parameters.AddWithValue("@ProductId", productidtxtbox.Text);
                        cmd.Parameters.AddWithValue("@PricePack", pricePack);
                        cmd.Parameters.AddWithValue("@DiscRatio", discRatio);
                        cmd.Parameters.AddWithValue("@TTLValue", ttlValue);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Data updated successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("No record updated. Please check the Purchase ID and Product ID.", "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void purchaseidtxtbox_TextChanged(object sender, EventArgs e)
        {
            // You can add any validation or auto-search logic here if needed
        }

        private void productidtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Auto-fetch product name when product ID is entered
            if (!string.IsNullOrEmpty(productidtxtbox.Text))
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string query = "SELECT ProductName FROM Products WHERE ProductID = @ProductId";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@ProductId", productidtxtbox.Text);

                            object result = cmd.ExecuteScalar();
                            if (result != null)
                            {
                                productnametxtbox.Text = result.ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Silently handle error - don't show message for auto-fetch
                    Console.WriteLine("Error fetching product name: " + ex.Message);
                }
            }
        }

        private void productnametxtbox_TextChanged(object sender, EventArgs e)
        {
            // No action needed
        }

        private void qtytxtbox_TextChanged(object sender, EventArgs e)
        {
            // Auto-calculate TTLValue when Qty, Price or Discount changes
            AutoCalculateTTLValue();
        }

        private void pricepacktxtbox_TextChanged(object sender, EventArgs e)
        {
            // Auto-calculate TTLValue when Qty, Price or Discount changes
            AutoCalculateTTLValue();
        }

        private void discountpercentagetxtbox_TextChanged(object sender, EventArgs e)
        {
            // Auto-calculate TTLValue when Qty, Price or Discount changes
            AutoCalculateTTLValue();
        }

        private void TTLvaluetxtbox_TextChanged(object sender, EventArgs e)
        {
            // No action needed
        }

        private void AutoCalculateTTLValue()
        {
            // Only calculate if all required fields have values
            if (!string.IsNullOrEmpty(qtytxtbox.Text) &&
                !string.IsNullOrEmpty(pricepacktxtbox.Text) &&
                !string.IsNullOrEmpty(discountpercentagetxtbox.Text))
            {
                decimal qty, pricePack, discRatio;

                if (decimal.TryParse(qtytxtbox.Text, out qty) &&
                    decimal.TryParse(pricepacktxtbox.Text, out pricePack) &&
                    decimal.TryParse(discountpercentagetxtbox.Text, out discRatio))
                {
                    // Calculate TTLValue: (Qty * Price) - (Qty * Price * DiscRatio/100)
                    decimal ttlValue = (qty * pricePack) - (qty * pricePack * discRatio / 100);
                    TTLvaluetxtbox.Text = ttlValue.ToString("F2");
                }
            }
        }
    }
}