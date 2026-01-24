using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Product_Price_List : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Product_Price_List()
        {
            InitializeComponent();
        }

        // 📌 COMPANY SEARCH
        private void searchcompany_Click(object sender, EventArgs e)
        {
            string companyID = cmpnyidtxtbox.Text.Trim();

            if (string.IsNullOrEmpty(companyID))
            {
                MessageBox.Show("Please enter Company ID", "Input Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT CompanyName FROM Companies WHERE CompanyID = @CompanyID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CompanyID", companyID);

                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        companynametxtbox.Text = result.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Company not found!", "Not Found",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        companynametxtbox.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 📌 GET PRODUCT DATA
        private DataTable GetProductPriceData(string companyID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT 
                    ProductID,
                    ProductName,
                    RetailPrice,
                    PurchasePrice,
                    PurDiscRatio,
                    PurDiscValue,
                    PurSTRatio,
                    SalePrice,
                    SaleDiscRatio,
                    SaleDiscValue,
                    SaleSTRatio
                FROM Products
                WHERE CompanyID = @CompanyID
                ORDER BY ProductName";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CompanyID", companyID);

                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        // 📌 PREVIEW BUTTON
        private void previewbtn_Click(object sender, EventArgs e)
        {
            string companyID = cmpnyidtxtbox.Text.Trim();

            if (string.IsNullOrEmpty(companyID))
            {
                MessageBox.Show("Please enter Company ID", "Input Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(companynametxtbox.Text))
            {
                MessageBox.Show("Please search for company first", "Input Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Determine price type
            string priceType = "";
            if (salepricewiseradiobtn.Checked)
                priceType = "Sale";
            else if (purchasepricewiseradiobtn.Checked)
                priceType = "Purchase";
            else if (retailpricewiseradiobtn.Checked)
                priceType = "Retail";
            else
            {
                MessageBox.Show("Please select a price type", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Get data
                DataTable dtReport = GetProductPriceData(companyID);

                if (dtReport.Rows.Count == 0)
                {
                    MessageBox.Show("No products found for this company", "No Data",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare DataSources
                var dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", dtReport);

                // Prepare parameters for RDLC
                var parameters = new Dictionary<string, string>
                {
                    { "CompanyName", companynametxtbox.Text },
                    { "PriceType", priceType }
                };

                // Show Report
                FO_2022.Reports.Report_viewer reportForm = new FO_2022.Reports.Report_viewer();
                reportForm.ShowReport("Product_Price_List", dataSources, parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading report: {ex.Message}", "Report Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 📌 PRINT BUTTON
        private void printbtn_Click(object sender, EventArgs e)
        {
            previewbtn_Click(sender, e);
        }

        // 📌 CLOSE BUTTON
        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // 📌 RADIO BUTTON EVENTS
        private void salepricewiseradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            if (salepricewiseradiobtn.Checked)
            {
            }
        }

        private void purchasepricewiseradiobtn_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void retailpricewiseradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            if (retailpricewiseradiobtn.Checked)
            {
                //UpdateRadioButtonStyles();
            }
        }

       

        private void cmpnyidtxtbox_TextChanged(object sender, EventArgs e)
        {
            companynametxtbox.Clear();
        }

        private void Product_Price_List_Load(object sender, EventArgs e)
        {
            // Set default selection and focus
            salepricewiseradiobtn.Checked = true;
            //UpdateRadioButtonStyles();
            cmpnyidtxtbox.Focus();
        }

        // 📌 KEYPRESS EVENTS FOR BETTER UX
        //private void cmpnyidtxtbox_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (e.KeyChar == (char)Keys.Enter)
        //    {
        //        searchcompany_Click(sender, e);
        //        e.Handled = true;
        //    }
        //}

       

        private void cmpnyidtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;       
            }
        }
    }
}