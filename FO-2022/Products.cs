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

namespace FO_2022
{
    public partial class stockalertlvltxtbox : Form
    {
        // Connection string
        private string connectionString = "Data Source=SQL5110.site4now.net;Initial Catalog=db_ac0035_pos;User Id=db_ac0035_pos_admin;Password=PTCL0614581188!";

        // DataTables for storing data
        private DataTable dtCompanies;
        private DataTable dtProductGroups;
        private DataTable dtSubGroups;
        private DataTable dtPackings;
        private DataTable dtProducts;
        private DataTable dtPackingsGrid; // New DataTable for packings grid

        // Variable to store selected packing ID
        private int selectedPackingId = -1;

        public stockalertlvltxtbox()
        {
            InitializeComponent();
        }

        private int SafeParseInt(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            text = text.Trim();

            if (int.TryParse(text, out int result))
                return result;

            return 0;
        }

        private void Products_Load(object sender, EventArgs e)
        {
            //LoadAllData();

            //// Packings grid load karein
            //LoadPackingsGrid();

            //// Event handlers attach karein
            //srchprdctnametxtbox.KeyPress += srchprdctnametxtbox_KeyPress;
            //slctprdctgrpcomboBox1.Click += slctprdctgrpcomboBox1_Click;
            //dataGridViewavlblepkng.SelectionChanged += dataGridViewavlblepkng_SelectionChanged;

            //// Packing weight textbox ke liye event handler
            //packinloseweighttxtbox.TextChanged += packinloseweighttxtbox_TextChanged_Multiplier;

            //// DataGridView2 events
            //dataGridView2.SelectionChanged += dataGridView2_SelectionChanged;
            //dataGridView2.CellEndEdit += dataGridView2_CellEndEdit;

            //// Purchase packing combobox selection change event
            //purchasepackcombobox.SelectedIndexChanged += purchasepackcombobox_SelectedIndexChanged_Multiplier;
            LoadAllData();

            // Packings grid load karein
            LoadPackingsGrid();

            // SARE PRODUCTS LOAD KAREIN FORM LOAD HOTE HI
            LoadAllProducts();

            // Event handlers attach karein
            srchprdctnametxtbox.KeyPress += srchprdctnametxtbox_KeyPress;
            slctprdctgrpcomboBox1.Click += slctprdctgrpcomboBox1_Click;
            dataGridViewavlblepkng.SelectionChanged += dataGridViewavlblepkng_SelectionChanged;

            // Packing weight textbox ke liye event handler
            packinloseweighttxtbox.TextChanged += packinloseweighttxtbox_TextChanged_Multiplier;

            // DataGridView2 events
            dataGridView2.SelectionChanged += dataGridView2_SelectionChanged;
            dataGridView2.CellEndEdit += dataGridView2_CellEndEdit;

            // Purchase packing combobox selection change event
            purchasepackcombobox.SelectedIndexChanged += purchasepackcombobox_SelectedIndexChanged_Multiplier;
        }
        private void LoadAllProducts()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Sare products load karein
                    string query = @"SELECT ProductID, ProductName 
                           FROM Products 
                           ORDER BY ProductName";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        // dataGridViewavlblepkng mein data show ho
                        dataGridViewavlblepkng.DataSource = dt;

                        // Grid formatting
                        dataGridViewavlblepkng.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                        // Header text set karein
                        if (dataGridViewavlblepkng.Columns.Count > 0)
                        {
                            dataGridViewavlblepkng.Columns["ProductID"].HeaderText = "Product ID";
                            dataGridViewavlblepkng.Columns["ProductName"].HeaderText = "Product Name";
                        }
                    }
                    else
                    {
                        dataGridViewavlblepkng.DataSource = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products: " + ex.Message, "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAllData()
        {
            try
            {
                // Load Companies
                LoadCompanies();

                // Load Product Groups
                LoadProductGroups();

                // Load Sub Groups
                LoadSubGroups();

                // Load Packings
                LoadPackings();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Data Loading Methods

        private void LoadCompanies()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT CompanyID, CompanyName FROM Companies ORDER BY CompanyName";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    dtCompanies = new DataTable();
                    da.Fill(dtCompanies);

                    // Bind to main company combobox
                    cmpnynamecombobox1.DataSource = dtCompanies.Copy();
                    cmpnynamecombobox1.DisplayMember = "CompanyName";
                    cmpnynamecombobox1.ValueMember = "CompanyID";

                    // REQUIREMENT 1: slctcmpnycombobox ko alag se bind karna
                    if (slctcmpnycombobox != null)
                    {
                        // Alag DataTable copy banayein independent binding ke liye
                        DataTable dtForSearchCombo = dtCompanies.Copy();
                        slctcmpnycombobox.DataSource = dtForSearchCombo;
                        slctcmpnycombobox.DisplayMember = "CompanyName";
                        slctcmpnycombobox.ValueMember = "CompanyID";
                        slctcmpnycombobox.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading companies: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProductGroups()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT CompanyId, GroupId, GroupName FROM ProductGroups ORDER BY GroupName";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    dtProductGroups = new DataTable();
                    da.Fill(dtProductGroups);

                    // Ensure columns exist
                    if (!dtProductGroups.Columns.Contains("CompanyId"))
                        dtProductGroups.Columns.Add("CompanyId", typeof(string));
                    if (!dtProductGroups.Columns.Contains("GroupId"))
                        dtProductGroups.Columns.Add("GroupId", typeof(int));
                    if (!dtProductGroups.Columns.Contains("GroupName"))
                        dtProductGroups.Columns.Add("GroupName", typeof(string));

                    // Bind to main product group combobox
                    grpnamecomboBox.DataSource = dtProductGroups.Copy();
                    grpnamecomboBox.DisplayMember = "GroupName";
                    grpnamecomboBox.ValueMember = "GroupId";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading product groups: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // REQUIREMENT 2: slctprdctgrpcomboBox1 click par ProductGroups load karne ka method
        private void LoadProductGroupsForSearchCombo()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT GroupId, GroupName FROM ProductGroups ORDER BY GroupName";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dtProductGroupsForSearch = new DataTable();
                    da.Fill(dtProductGroupsForSearch);

                    // Bind to search product group combobox
                    slctprdctgrpcomboBox1.DataSource = dtProductGroupsForSearch;
                    slctprdctgrpcomboBox1.DisplayMember = "GroupName";
                    slctprdctgrpcomboBox1.ValueMember = "GroupId";
                    slctprdctgrpcomboBox1.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading product groups: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSubGroups()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"SELECT sg.GroupID, sg.SubGroupId, sg.CompanyId, sg.SubGroupName 
                                   FROM ProductSubGroups sg
                                   INNER JOIN ProductGroups pg ON sg.CompanyId = pg.CompanyId AND sg.GroupID = pg.GroupId
                                   ORDER BY sg.SubGroupName";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    dtSubGroups = new DataTable();
                    da.Fill(dtSubGroups);

                    // Ensure columns exist
                    if (!dtSubGroups.Columns.Contains("CompanyId"))
                        dtSubGroups.Columns.Add("CompanyId", typeof(string));
                    if (!dtSubGroups.Columns.Contains("GroupID"))
                        dtSubGroups.Columns.Add("GroupID", typeof(int));
                    if (!dtSubGroups.Columns.Contains("SubGroupId"))
                        dtSubGroups.Columns.Add("SubGroupId", typeof(int));
                    if (!dtSubGroups.Columns.Contains("SubGroupName"))
                        dtSubGroups.Columns.Add("SubGroupName", typeof(string));

                    // Bind to sub group combobox
                    subgrpnamecomboBox.DataSource = dtSubGroups.Copy();
                    subgrpnamecomboBox.DisplayMember = "SubGroupName";
                    subgrpnamecomboBox.ValueMember = "SubGroupId";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sub groups: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPackings()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT PackingId, PackingName FROM Packings ORDER BY PackingName";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    dtPackings = new DataTable();
                    da.Fill(dtPackings);

                    // Bind to purchase pack combobox
                    purchasepackcombobox.DataSource = dtPackings.Copy();
                    purchasepackcombobox.DisplayMember = "PackingName";
                    purchasepackcombobox.ValueMember = "PackingId";

                    // Bind to sale pack combobox
                    salepackcombobox.DataSource = dtPackings.Copy();
                    salepackcombobox.DisplayMember = "PackingName";
                    salepackcombobox.ValueMember = "PackingId";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading packings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Packings grid load karne ka method (dataGridView2 ke liye)
        private void LoadPackingsGrid()
        {
            //try
            //{
            //    using (SqlConnection conn = new SqlConnection(connectionString))
            //    {
            //        string query = @"SELECT 
            //                        p.PackingId,
            //                        p.PackingName,
            //                        p.Abbrevation,
            //                        CAST(0 AS decimal(10,2)) AS Multiplier  -- Default 0 value
            //                     FROM Packings p
            //                     ORDER BY p.PackingName";

            //        SqlDataAdapter da = new SqlDataAdapter(query, conn);
            //        dtPackingsGrid = new DataTable();
            //        da.Fill(dtPackingsGrid);

            //        // Bind to DataGridView2
            //        dataGridView2.DataSource = dtPackingsGrid;
            //        FormatPackingsGrid();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Error loading packings grid: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Sirf PackingId, PackingName aur Multiplier column select karein
                    // Abbrevation column nahi select karein
                    string query = @"SELECT 
                            p.PackingId,
                            p.PackingName,
                            CAST(0 AS decimal(10,2)) AS Multiplier  -- Default 0 value
                         FROM Packings p
                         ORDER BY p.PackingName";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    dtPackingsGrid = new DataTable();
                    da.Fill(dtPackingsGrid);

                    // Bind to DataGridView2
                    dataGridView2.DataSource = dtPackingsGrid;
                    FormatPackingsGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading packings grid: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatPackingsGrid()
        {
            //if (dataGridView2.Columns.Count > 0)
            //{
            //    dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            //    // Column names set karein
            //    dataGridView2.Columns["PackingId"].HeaderText = "Packing ID";
            //    dataGridView2.Columns["PackingName"].HeaderText = "Packing Name";
            //    dataGridView2.Columns["Multiplier"].HeaderText = "Multiplier";
            //    dataGridView2.Columns["Abbrevation"].HeaderText = "Abbreviation";

            //    // PackingId column hide karein
            //    dataGridView2.Columns["PackingId"].Visible = false;

            //    // Multiplier column ko editable banayein
            //    dataGridView2.Columns["Multiplier"].ReadOnly = false;

            //    // Column widths set karein
            //    dataGridView2.Columns["PackingName"].Width = 150;
            //    dataGridView2.Columns["Multiplier"].Width = 100;
            //    dataGridView2.Columns["Abbrevation"].Width = 80;
            //}
            if (dataGridView2.Columns.Count > 0)
            {
                dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Sirf do columns ke header text set karein
                // Abbrevation column ab DataTable mein nahi hai
                dataGridView2.Columns["PackingId"].HeaderText = "Packing ID";
                dataGridView2.Columns["PackingName"].HeaderText = "Packing Name";
                dataGridView2.Columns["Multiplier"].HeaderText = "Multiplier";

                // PackingId column hide karein
                dataGridView2.Columns["PackingId"].Visible = false;

                // Multiplier column ko editable banayein
                dataGridView2.Columns["Multiplier"].ReadOnly = false;

                // Sirf do columns visible hain
                // Column widths set karein
                dataGridView2.Columns["PackingName"].Width = 200;  // Zyada width dein kyunki ab do hi columns hain
                dataGridView2.Columns["Multiplier"].Width = 150;   // Zyada width dein
            }
        }

        #endregion

        #region REQUIREMENT 2: slctprdctgrpcomboBox1 Click Event

        private void slctprdctgrpcomboBox1_Click(object sender, EventArgs e)
        {
            // REQUIREMENT 2: Click karne par ProductGroups table se data fetch ho
            LoadProductGroupsForSearchCombo();
        }

        #endregion

        #region REQUIREMENT 3: Search Product and Show in GridView

        private void srchprdctnametxtbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // REQUIREMENT 3: Enter press karne par search ho
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SearchProductByName();
            }
        }

        private void SearchProductByName()
        {
            string productName = srchprdctnametxtbox.Text.Trim();

            if (string.IsNullOrWhiteSpace(productName))
            {
                MessageBox.Show("Please enter product name to search", "Information",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
                srchprdctnametxtbox.Focus();
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"SELECT ProductID, ProductName 
                                   FROM Products 
                                   WHERE ProductName LIKE @ProductName 
                                   ORDER BY ProductName";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    da.SelectCommand.Parameters.AddWithValue("@ProductName", "%" + productName + "%");

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        // REQUIREMENT 3: dataGridViewavlblepkng mein data show ho
                        dataGridViewavlblepkng.DataSource = dt;

                        // Grid formatting
                        dataGridViewavlblepkng.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                        // Header text set karein
                        if (dataGridViewavlblepkng.Columns.Count > 0)
                        {
                            dataGridViewavlblepkng.Columns["ProductID"].HeaderText = "Product ID";
                            dataGridViewavlblepkng.Columns["ProductName"].HeaderText = "Product Name";
                        }

                        // Show message kitne products milay
                        MessageBox.Show($"{dt.Rows.Count} product(s) found", "Search Results",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No products found with this name", "Search Results",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dataGridViewavlblepkng.DataSource = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching products: " + ex.Message, "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region DataGridView Selection Events

        private void dataGridViewavlblepkng_SelectionChanged(object sender, EventArgs e)
        {
            //// REQUIREMENT 3: Grid se select karne par textboxes mein data show ho
            //if (dataGridViewavlblepkng.SelectedRows.Count > 0)
            //{
            //    DataGridViewRow row = dataGridViewavlblepkng.SelectedRows[0];

            //    // ptdcidttxtbox1 mein ProductID show ho
            //    if (row.Cells["ProductID"].Value != null)
            //    {
            //        ptdcidttxtbox1.Text = row.Cells["ProductID"].Value.ToString();
            //    }

            //    // prdctnametxtbox mein ProductName show ho
            //    if (row.Cells["ProductName"].Value != null)
            //    {
            //        prdctnametxtbox.Text = row.Cells["ProductName"].Value.ToString();
            //    }
            //}
            // REQUIREMENT 3: Grid se select karne par textboxes mein data show ho
            if (dataGridViewavlblepkng.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridViewavlblepkng.SelectedRows[0];

                // ptdcidttxtbox1 mein ProductID show ho
                if (row.Cells["ProductID"].Value != null)
                {
                    string productId = row.Cells["ProductID"].Value.ToString();
                    ptdcidttxtbox1.Text = productId;

                    // PURA PRODUCT DATA LOAD KAREIN TEXTBOXES AUR COMBOBOXES MEIN
                    LoadProductForEdit(productId);
                }
            }
        }
        private void LoadProductForEdit(string productId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"SELECT p.*, c.CompanyName, pg.GroupName, psg.SubGroupName
                           FROM Products p
                           LEFT JOIN Companies c ON p.CompanyID = c.CompanyID
                           LEFT JOIN ProductGroups pg ON p.CompanyID = pg.CompanyId AND p.GroupId = pg.GroupId
                           LEFT JOIN ProductSubGroups psg ON p.CompanyID = psg.CompanyId AND p.GroupId = psg.GroupID AND p.SubGroupId = psg.SubGroupId
                           WHERE p.ProductID = @ProductID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProductID", productId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];

                        prdctnametxtbox.Text = row["ProductName"].ToString();

                        // Set company
                        string companyId = row["CompanyID"].ToString();
                        foreach (DataRowView item in cmpnynamecombobox1.Items)
                        {
                            if (item["CompanyID"].ToString() == companyId)
                            {
                                cmpnynamecombobox1.SelectedValue = companyId;
                                break;
                            }
                        }

                        // Set group
                        if (row["GroupId"] != DBNull.Value)
                        {
                            int groupId = Convert.ToInt32(row["GroupId"]);
                            foreach (DataRowView item in grpnamecomboBox.Items)
                            {
                                if (Convert.ToInt32(item["GroupId"]) == groupId)
                                {
                                    grpnamecomboBox.SelectedValue = groupId;
                                    break;
                                }
                            }
                        }

                        // Set sub group
                        if (row["SubGroupId"] != DBNull.Value)
                        {
                            int subGroupId = Convert.ToInt32(row["SubGroupId"]);
                            foreach (DataRowView item in subgrpnamecomboBox.Items)
                            {
                                if (Convert.ToInt32(item["SubGroupId"]) == subGroupId)
                                {
                                    subgrpnamecomboBox.SelectedValue = subGroupId;
                                    break;
                                }
                            }
                        }

                        // Set packings
                        if (row["PurPackingId"] != DBNull.Value)
                        {
                            purchasepackcombobox.SelectedValue = row["PurPackingId"];
                            selectedPackingId = Convert.ToInt32(row["PurPackingId"]);
                        }

                        if (row["SalPackingId"] != DBNull.Value)
                        {
                            salepackcombobox.SelectedValue = row["SalPackingId"];
                        }

                        // Set other fields
                        purchasepricetxtbox1.Text = row["PurchasePrice"].ToString();
                        salepricetxtbox2.Text = row["SalePrice"].ToString();
                        retailpricetxtbox.Text = row["RetailPrice"]?.ToString() ?? "";
                        packinloseweighttxtbox.Text = row["ProductWeight"].ToString();
                        purchasedisctxtbox1.Text = row["PurDiscRatio"].ToString();
                        saledisctxtbox2.Text = row["SaleDiscRatio"].ToString();
                        purchasediscvaltxtbox1.Text = row["PurDiscValue"].ToString();
                        putchasediscvaltxtbox2.Text = row["SaleDiscValue"].ToString();
                        sttxtbox.Text = row["PurSTRatio"].ToString();
                        stvaltxtbox.Text = row["SaleSTRatio"].ToString();
                        orderstocktxtbox.Text = row["OrderStockLevel"].ToString();
                        barcodetxtbox.Text = row["BarCode"]?.ToString() ?? "";

                        // Load multipliers from ProductPacking table
                        LoadProductMultipliers(productId);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // DataGridView2 mein selection change hone par packing details show karein
        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            //if (dataGridView2.SelectedRows.Count > 0)
            //{
            //    DataGridViewRow row = dataGridView2.SelectedRows[0];

            //    // Agar packing comboboxes mein select karna chahate hain
            //    if (row.Cells["PackingId"].Value != null)
            //    {
            //        int packingId = Convert.ToInt32(row.Cells["PackingId"].Value);

            //        // Purchase packing combobox mein select karein
            //        foreach (DataRowView item in purchasepackcombobox.Items)
            //        {
            //            if (Convert.ToInt32(item["PackingId"]) == packingId)
            //            {
            //                purchasepackcombobox.SelectedItem = item;
            //                break;
            //            }
            //        }

            //        // Sale packing combobox mein select karein
            //        foreach (DataRowView item in salepackcombobox.Items)
            //        {
            //            if (Convert.ToInt32(item["PackingId"]) == packingId)
            //            {
            //                salepackcombobox.SelectedItem = item;
            //                break;
            //            }
            //        }
            //    }
            //}
            if (dataGridView2.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView2.SelectedRows[0];

                // Agar packing comboboxes mein select karna chahate hain
                if (row.Cells["PackingId"].Value != null)
                {
                    int packingId = Convert.ToInt32(row.Cells["PackingId"].Value);

                    // Purchase packing combobox mein select karein
                    foreach (DataRowView item in purchasepackcombobox.Items)
                    {
                        if (Convert.ToInt32(item["PackingId"]) == packingId)
                        {
                            purchasepackcombobox.SelectedItem = item;
                            break;
                        }
                    }

                    // Sale packing combobox mein select karein
                    foreach (DataRowView item in salepackcombobox.Items)
                    {
                        if (Convert.ToInt32(item["PackingId"]) == packingId)
                        {
                            salepackcombobox.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region Multiplier Update Logic (Selected Packing Only)

        // Purchase packing combobox selection change event
        private void purchasepackcombobox_SelectedIndexChanged_Multiplier(object sender, EventArgs e)
        {
            // Selected packing ID store karein
            if (purchasepackcombobox.SelectedValue != null)
            {
                selectedPackingId = Convert.ToInt32(purchasepackcombobox.SelectedValue);
            }
            else
            {
                selectedPackingId = -1;
            }
        }

        private void packinloseweighttxtbox_TextChanged_Multiplier(object sender, EventArgs e)
        {
            // Packing weight textbox mein value change hone par selected packing ka Multiplier update karein
            UpdateSelectedPackingMultiplier();
        }

        private void UpdateSelectedPackingMultiplier()
        {
            try
            {
                if (selectedPackingId == -1)
                {
                    MessageBox.Show("Please select a packing first", "Information",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(packinloseweighttxtbox.Text))
                {
                    // Safely parse the weight value
                    if (decimal.TryParse(packinloseweighttxtbox.Text, out decimal weightValue))
                    {
                        // Agar weight zero ya negative hai to 0 show karein
                        if (weightValue <= 0)
                        {
                            UpdateSingleMultiplier(selectedPackingId, 0);
                        }
                        else
                        {
                            // Weight ki value ko multiplier ki tarah show karein
                            UpdateSingleMultiplier(selectedPackingId, weightValue);
                        }
                    }
                    else
                    {
                        // Invalid number - selected packing ka multiplier 0 set karein
                        UpdateSingleMultiplier(selectedPackingId, 0);
                        MessageBox.Show("Please enter a valid number", "Validation Error",
                                       MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else if (string.IsNullOrWhiteSpace(packinloseweighttxtbox.Text))
                {
                    // Textbox empty hai to selected packing ka multiplier 0 set karein
                    UpdateSingleMultiplier(selectedPackingId, 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating multiplier: " + ex.Message,
                               "Error",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error);
            }
        }

        private void UpdateSingleMultiplier(int packingId, decimal value)
        {
            // Sirf selected packing ka multiplier update karein
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (!row.IsNewRow && row.Cells["PackingId"].Value != null)
                {
                    int currentPackingId = Convert.ToInt32(row.Cells["PackingId"].Value);
                    if (currentPackingId == packingId)
                    {
                        row.Cells["Multiplier"].Value = value;
                        break; // Sirf ek hi row update karna hai
                    }
                }
            }
        }

        #endregion

        #region Event Handlers

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            //Mainform1 mainForm = new Mainform1();
            //mainForm.Show();
            //this.Hide();
            this.Close();
        }

        private void cmpnynamecombobox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // When company changes, filter product groups
            if (cmpnynamecombobox1.SelectedValue != null && dtProductGroups != null && dtProductGroups.Rows.Count > 0)
            {
                string selectedCompanyId = cmpnynamecombobox1.SelectedValue.ToString();
                FilterProductGroupsByCompany(selectedCompanyId);
            }
            else
            {
                ResetGroupComboBoxes();
            }
        }

        private void FilterProductGroupsByCompany(string companyId)
        {
            try
            {
                if (dtProductGroups == null || dtProductGroups.Rows.Count == 0)
                {
                    ResetGroupComboBoxes();
                    return;
                }

                DataView dv = new DataView(dtProductGroups);
                dv.RowFilter = $"CompanyId = '{companyId}'";

                // Create a new DataTable from the filtered DataView
                DataTable filteredTable = dv.ToTable();

                // Check if the filtered table has rows
                if (filteredTable.Rows.Count > 0)
                {
                    grpnamecomboBox.DataSource = filteredTable;
                    grpnamecomboBox.DisplayMember = "GroupName";
                    grpnamecomboBox.ValueMember = "GroupId";
                    grpnamecomboBox.Enabled = true;
                }
                else
                {
                    // If no groups for this company, reset the combobox
                    grpnamecomboBox.DataSource = null;
                    grpnamecomboBox.Items.Clear();
                    grpnamecomboBox.Text = "";
                    grpnamecomboBox.Enabled = false;

                    // Also reset sub group combobox
                    subgrpnamecomboBox.DataSource = null;
                    subgrpnamecomboBox.Items.Clear();
                    subgrpnamecomboBox.Text = "";
                    subgrpnamecomboBox.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error filtering groups: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetGroupComboBoxes();
            }
        }

        private void grpnamecomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // When group changes, filter sub groups
            if (grpnamecomboBox.SelectedValue != null && cmpnynamecombobox1.SelectedValue != null
                && dtSubGroups != null && dtSubGroups.Rows.Count > 0)
            {
                string selectedCompanyId = cmpnynamecombobox1.SelectedValue.ToString();
                string selectedGroupId = grpnamecomboBox.SelectedValue.ToString();
                FilterSubGroups(selectedCompanyId, selectedGroupId);
            }
            else
            {
                ResetSubGroupComboBox();
            }
        }

        private void FilterSubGroups(string companyId, string groupId)
        {
            try
            {
                if (dtSubGroups == null || dtSubGroups.Rows.Count == 0)
                {
                    ResetSubGroupComboBox();
                    return;
                }

                DataView dv = new DataView(dtSubGroups);
                dv.RowFilter = $"CompanyId = '{companyId}' AND GroupID = {groupId}";

                // Create a new DataTable from the filtered DataView
                DataTable filteredTable = dv.ToTable();

                // Check if the filtered table has rows
                if (filteredTable.Rows.Count > 0)
                {
                    subgrpnamecomboBox.DataSource = filteredTable;
                    subgrpnamecomboBox.DisplayMember = "SubGroupName";
                    subgrpnamecomboBox.ValueMember = "SubGroupId";
                    subgrpnamecomboBox.Enabled = true;
                }
                else
                {
                    // If no sub-groups for this group, reset the combobox
                    subgrpnamecomboBox.DataSource = null;
                    subgrpnamecomboBox.Items.Clear();
                    subgrpnamecomboBox.Text = "";
                    subgrpnamecomboBox.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error filtering sub groups: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetSubGroupComboBox();
            }
        }

        private void ResetGroupComboBoxes()
        {
            grpnamecomboBox.DataSource = null;
            grpnamecomboBox.Items.Clear();
            grpnamecomboBox.Text = "";
            grpnamecomboBox.Enabled = false;

            ResetSubGroupComboBox();
        }

        private void ResetSubGroupComboBox()
        {
            subgrpnamecomboBox.DataSource = null;
            subgrpnamecomboBox.Items.Clear();
            subgrpnamecomboBox.Text = "";
            subgrpnamecomboBox.Enabled = false;
        }

        #endregion

        #region Button Click Events

        private void newbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
            ptdcidttxtbox1.Focus();
        }

        private void editbtn_Click(object sender, EventArgs e)
        {
            //if (dataGridViewavlblepkng.SelectedRows.Count > 0)
            //{
            //    LoadProductForEdit();
            //}
            //else
            //{
            //    MessageBox.Show("Please select a product to edit.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
            if (dataGridViewavlblepkng.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridViewavlblepkng.SelectedRows[0];
                if (row.Cells["ProductID"].Value != null)
                {
                    string productId = row.Cells["ProductID"].Value.ToString();
                    LoadProductForEdit(productId);
                }
            }
            else if (!string.IsNullOrWhiteSpace(ptdcidttxtbox1.Text))
            {
                // Agar grid se nahi, to textbox se product ID lekar load karein
                LoadProductForEdit(ptdcidttxtbox1.Text.Trim());
            }
            else
            {
                MessageBox.Show("Please select a product to edit.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ptdcidttxtbox1.Text))
            {
                if (MessageBox.Show("Are you sure you want to delete this product?", "Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DeleteProduct();
                }
            }
            else
            {
                MessageBox.Show("Please select a product to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                SaveProduct();
                
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region CRUD Operations

        private void LoadProductForEdit()
        {
            try
            {
                string productId = ptdcidttxtbox1.Text.Trim();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"SELECT p.*, c.CompanyName, pg.GroupName, psg.SubGroupName
                                   FROM Products p
                                   LEFT JOIN Companies c ON p.CompanyID = c.CompanyID
                                   LEFT JOIN ProductGroups pg ON p.CompanyID = pg.CompanyId AND p.GroupId = pg.GroupId
                                   LEFT JOIN ProductSubGroups psg ON p.CompanyID = psg.CompanyId AND p.GroupId = psg.GroupID AND p.SubGroupId = psg.SubGroupId
                                   WHERE p.ProductID = @ProductID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProductID", productId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];

                        prdctnametxtbox.Text = row["ProductName"].ToString();

                        // Set company
                        string companyId = row["CompanyID"].ToString();
                        foreach (DataRowView item in cmpnynamecombobox1.Items)
                        {
                            if (item["CompanyID"].ToString() == companyId)
                            {
                                cmpnynamecombobox1.SelectedValue = companyId;
                                break;
                            }
                        }

                        // Set group
                        if (row["GroupId"] != DBNull.Value)
                        {
                            int groupId = Convert.ToInt32(row["GroupId"]);
                            foreach (DataRowView item in grpnamecomboBox.Items)
                            {
                                if (Convert.ToInt32(item["GroupId"]) == groupId)
                                {
                                    grpnamecomboBox.SelectedValue = groupId;
                                    break;
                                }
                            }
                        }

                        // Set sub group
                        if (row["SubGroupId"] != DBNull.Value)
                        {
                            int subGroupId = Convert.ToInt32(row["SubGroupId"]);
                            foreach (DataRowView item in subgrpnamecomboBox.Items)
                            {
                                if (Convert.ToInt32(item["SubGroupId"]) == subGroupId)
                                {
                                    subgrpnamecomboBox.SelectedValue = subGroupId;
                                    break;
                                }
                            }
                        }

                        // Set packings
                        if (row["PurPackingId"] != DBNull.Value)
                        {
                            purchasepackcombobox.SelectedValue = row["PurPackingId"];
                            selectedPackingId = Convert.ToInt32(row["PurPackingId"]);
                        }

                        if (row["SalPackingId"] != DBNull.Value)
                        {
                            salepackcombobox.SelectedValue = row["SalPackingId"];
                        }

                        // Set other fields
                        purchasepricetxtbox1.Text = row["PurchasePrice"].ToString();
                        salepricetxtbox2.Text = row["SalePrice"].ToString();
                        retailpricetxtbox.Text = row["RetailPrice"]?.ToString() ?? "";
                        packinloseweighttxtbox.Text = row["ProductWeight"].ToString();
                        purchasedisctxtbox1.Text = row["PurDiscRatio"].ToString();
                        saledisctxtbox2.Text = row["SaleDiscRatio"].ToString();
                        purchasediscvaltxtbox1.Text = row["PurDiscValue"].ToString();
                        putchasediscvaltxtbox2.Text = row["SaleDiscValue"].ToString();
                        sttxtbox.Text = row["PurSTRatio"].ToString();
                        stvaltxtbox.Text = row["SaleSTRatio"].ToString();
                        orderstocktxtbox.Text = row["OrderStockLevel"].ToString();
                        barcodetxtbox.Text = row["BarCode"]?.ToString() ?? "";

                        // Load multipliers from ProductPacking table
                        LoadProductMultipliers(productId);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProductMultipliers(string productId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"SELECT pp.PackingId, pp.Multiplier
                                   FROM ProductPacking pp
                                   WHERE pp.ProductId = @ProductId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProductId", productId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dtMultipliers = new DataTable();
                    da.Fill(dtMultipliers);

                    // Grid mein multipliers set karein
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (!row.IsNewRow && row.Cells["PackingId"].Value != null)
                        {
                            int packingId = Convert.ToInt32(row.Cells["PackingId"].Value);

                            // Find multiplier for this packing
                            DataRow[] matchingRows = dtMultipliers.Select($"PackingId = {packingId}");
                            if (matchingRows.Length > 0)
                            {
                                row.Cells["Multiplier"].Value = matchingRows[0]["Multiplier"];
                            }
                            else
                            {
                                row.Cells["Multiplier"].Value = 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading multipliers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SaveProduct()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // ... existing save code ...

                    MessageBox.Show("Product saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Refresh grids
                    LoadPackingsGrid();
                    LoadAllProducts(); // SARE PRODUCTS REFRESH KAREIN
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //private void SaveProduct()
        //{
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();

        //            string productId = ptdcidttxtbox1.Text.Trim();
        //            string companyId = cmpnynamecombobox1.SelectedValue?.ToString();
        //            string groupId = grpnamecomboBox.SelectedValue?.ToString();
        //            string subGroupId = subgrpnamecomboBox.SelectedValue?.ToString();
        //            string packingId = purchasepackcombobox.SelectedValue?.ToString();
        //            string salePackingId = salepackcombobox.SelectedValue?.ToString();

        //            // Check if product exists
        //            string checkQuery = "SELECT COUNT(*) FROM Products WHERE ProductID = @ProductID";
        //            SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
        //            checkCmd.Parameters.AddWithValue("@ProductID", productId);
        //            int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

        //            string query;
        //            if (exists > 0)
        //            {
        //                // Update existing product
        //                query = @"UPDATE Products SET 
        //                CompanyID = @CompanyID,
        //                GroupId = @GroupId,
        //                SubGroupId = @SubGroupId,
        //                ProductName = @ProductName,
        //                PurchasePrice = @PurchasePrice,
        //                SalePrice = @SalePrice,
        //                RetailPrice = @RetailPrice,
        //                PurDiscRatio = @PurDiscRatio,
        //                SaleDiscRatio = @SaleDiscRatio,
        //                PurDiscValue = @PurDiscValue,
        //                SaleDiscValue = @SaleDiscValue,
        //                PurSTRatio = @PurSTRatio,
        //                SaleSTRatio = @SaleSTRatio,
        //                PurPackingId = @PurPackingId,
        //                SalPackingId = @SalPackingId,
        //                OrderStockLevel = @OrderStockLevel,
        //                BarCode = @BarCode,
        //                ProductWeight = @ProductWeight,
        //                MinStockLevel = @MinStockLevel
        //                WHERE ProductID = @ProductID";
        //            }
        //            else
        //            {
        //                // Insert new product
        //                query = @"INSERT INTO Products 
        //                (ProductID, CompanyID, GroupId, SubGroupId, ProductName, 
        //                PurchasePrice, SalePrice, RetailPrice, PurDiscRatio, SaleDiscRatio,
        //                PurDiscValue, SaleDiscValue, PurSTRatio, SaleSTRatio, PurPackingId, 
        //                SalPackingId, OrderStockLevel, BarCode, ProductWeight, MinStockLevel,
        //                IsFinishProduct, IsValueB4Ratio, IsFTaxExempted)
        //                VALUES 
        //                (@ProductID, @CompanyID, @GroupId, @SubGroupId, @ProductName,
        //                @PurchasePrice, @SalePrice, @RetailPrice, @PurDiscRatio, @SaleDiscRatio,
        //                @PurDiscValue, @SaleDiscValue, @PurSTRatio, @SaleSTRatio, @PurPackingId,
        //                @SalPackingId, @OrderStockLevel, @BarCode, @ProductWeight, @MinStockLevel,
        //                0, 0, 0)";
        //            }

        //            SqlCommand cmd = new SqlCommand(query, conn);
        //            cmd.Parameters.AddWithValue("@ProductID", productId);
        //            cmd.Parameters.AddWithValue("@CompanyID", companyId ?? (object)DBNull.Value);
        //            cmd.Parameters.AddWithValue("@GroupId", groupId ?? (object)DBNull.Value);
        //            cmd.Parameters.AddWithValue("@SubGroupId", subGroupId ?? (object)DBNull.Value);
        //            cmd.Parameters.AddWithValue("@ProductName", prdctnametxtbox.Text.Trim());
        //            cmd.Parameters.AddWithValue("@PurPackingId", packingId ?? (object)DBNull.Value);
        //            cmd.Parameters.AddWithValue("@SalPackingId", salePackingId ?? (object)DBNull.Value);
        //            cmd.Parameters.AddWithValue("@BarCode", barcodetxtbox.Text.Trim());

        //            // Use SafeParse methods
        //            cmd.Parameters.AddWithValue("@PurchasePrice", SafeParseDecimal(purchasepricetxtbox1.Text));
        //            cmd.Parameters.AddWithValue("@SalePrice", SafeParseDecimal(salepricetxtbox2.Text));
        //            cmd.Parameters.AddWithValue("@RetailPrice", SafeParseDecimal(retailpricetxtbox.Text));
        //            cmd.Parameters.AddWithValue("@OrderStockLevel", SafeParseInt(orderstocktxtbox.Text));
        //            cmd.Parameters.AddWithValue("@ProductWeight", SafeParseDecimal(packinloseweighttxtbox.Text));
        //            cmd.Parameters.AddWithValue("@PurDiscRatio", SafeParseDecimal(purchasedisctxtbox1.Text));
        //            cmd.Parameters.AddWithValue("@SaleDiscRatio", SafeParseDecimal(saledisctxtbox2.Text));
        //            cmd.Parameters.AddWithValue("@PurDiscValue", SafeParseDecimal(purchasediscvaltxtbox1.Text));
        //            cmd.Parameters.AddWithValue("@SaleDiscValue", SafeParseDecimal(putchasediscvaltxtbox2.Text));
        //            cmd.Parameters.AddWithValue("@PurSTRatio", SafeParseDecimal(sttxtbox.Text));
        //            cmd.Parameters.AddWithValue("@SaleSTRatio", SafeParseDecimal(stvaltxtbox.Text));
        //            cmd.Parameters.AddWithValue("@MinStockLevel", SafeParseInt(orderstocktxtbox.Text));

        //            cmd.ExecuteNonQuery();

        //            // Product save hone ke baad packing information save karein
        //            SaveProductPacking(productId);

        //            MessageBox.Show("Product saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

        //            // Refresh grids
        //            LoadPackingsGrid();
        //            ClearForm();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error saving product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
        private void DeleteProduct()
        {
            try
            {
                string productId = ptdcidttxtbox1.Text.Trim();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // ... existing delete code ...

                    MessageBox.Show("Product deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Refresh grids
                    LoadPackingsGrid();
                    LoadAllProducts(); // SARE PRODUCTS REFRESH KAREIN
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveProductPacking(string productId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Pehle existing records delete karein
                    string deleteQuery = "DELETE FROM ProductPacking WHERE ProductId = @ProductId";
                    SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn);
                    deleteCmd.Parameters.AddWithValue("@ProductId", productId);
                    deleteCmd.ExecuteNonQuery();

                    // Phir naye records insert karein
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (!row.IsNewRow && row.Cells["Multiplier"].Value != null)
                        {
                            int packingId = Convert.ToInt32(row.Cells["PackingId"].Value);

                            if (decimal.TryParse(row.Cells["Multiplier"].Value.ToString(), out decimal multiplier))
                            {
                                // Agar multiplier zero nahi hai to hi insert karein
                                if (multiplier > 0)
                                {
                                    string insertQuery = @"INSERT INTO ProductPacking 
                                                        (ProductId, PackingId, Multiplier) 
                                                        VALUES (@ProductId, @PackingId, @Multiplier)";

                                    SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                                    insertCmd.Parameters.AddWithValue("@ProductId", productId);
                                    insertCmd.Parameters.AddWithValue("@PackingId", packingId);
                                    insertCmd.Parameters.AddWithValue("@Multiplier", multiplier);
                                    insertCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving product packing: " + ex.Message,
                               "Error",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error);
            }
        }


        //private void DeleteProduct()
        //{
        //    try
        //    {
        //        string productId = ptdcidttxtbox1.Text.Trim();

        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();

        //            // Pehle ProductPacking records delete karein
        //            string deletePackingQuery = "DELETE FROM ProductPacking WHERE ProductId = @ProductID";
        //            SqlCommand deletePackingCmd = new SqlCommand(deletePackingQuery, conn);
        //            deletePackingCmd.Parameters.AddWithValue("@ProductID", productId);
        //            deletePackingCmd.ExecuteNonQuery();

        //            // Phir product delete karein
        //            string query = "DELETE FROM Products WHERE ProductID = @ProductID";
        //            SqlCommand cmd = new SqlCommand(query, conn);
        //            cmd.Parameters.AddWithValue("@ProductID", productId);
        //            cmd.ExecuteNonQuery();

        //            MessageBox.Show("Product deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

        //            // Refresh grids
        //            LoadPackingsGrid();
        //            ClearForm();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error deleting product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(ptdcidttxtbox1.Text))
            {
                MessageBox.Show("Please enter Product ID", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ptdcidttxtbox1.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(prdctnametxtbox.Text))
            {
                MessageBox.Show("Please enter Product Name", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                prdctnametxtbox.Focus();
                return false;
            }

            if (cmpnynamecombobox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a Company", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmpnynamecombobox1.Focus();
                return false;
            }

            // Validate numeric fields
            if (!decimal.TryParse(purchasepricetxtbox1.Text, out decimal purchasePrice) || purchasePrice < 0)
            {
                MessageBox.Show("Please enter a valid Purchase Price", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                purchasepricetxtbox1.Focus();
                return false;
            }

            if (!decimal.TryParse(salepricetxtbox2.Text, out decimal salePrice) || salePrice < 0)
            {
                MessageBox.Show("Please enter a valid Sale Price", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                salepricetxtbox2.Focus();
                return false;
            }

            if (!decimal.TryParse(retailpricetxtbox.Text, out decimal retailPrice) || retailPrice < 0)
            {
                MessageBox.Show("Please enter a valid Retail Price", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                retailpricetxtbox.Focus();
                return false;
            }

            if (!int.TryParse(orderstocktxtbox.Text, out int orderStock) || orderStock < 0)
            {
                MessageBox.Show("Please enter a valid Order Stock Level", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                orderstocktxtbox.Focus();
                return false;
            }

            if (!decimal.TryParse(packinloseweighttxtbox.Text, out decimal productWeight) || productWeight < 0)
            {
                MessageBox.Show("Please enter a valid Product Weight", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                packinloseweighttxtbox.Focus();
                return false;
            }

            return true;
        }

        private decimal SafeParseDecimal(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            text = text.Trim();
            text = text.Replace(",", ".");

            if (decimal.TryParse(text, out decimal result))
                return result;

            return 0;
        }

        private void ClearForm()
        {
            ptdcidttxtbox1.Clear();
            prdctnametxtbox.Clear();
            cmpnynamecombobox1.SelectedIndex = -1;
            grpnamecomboBox.DataSource = null;
            grpnamecomboBox.Items.Clear();
            grpnamecomboBox.Text = "";
            subgrpnamecomboBox.DataSource = null;
            subgrpnamecomboBox.Items.Clear();
            subgrpnamecomboBox.Text = "";
            purchasepackcombobox.SelectedIndex = -1;
            salepackcombobox.SelectedIndex = -1;
            packinloseweighttxtbox.Clear();
            retailpricetxtbox.Clear();
            purchasepricetxtbox1.Clear();
            salepricetxtbox2.Clear();
            purchasedisctxtbox1.Clear();
            saledisctxtbox2.Clear();
            purchasediscvaltxtbox1.Clear();
            putchasediscvaltxtbox2.Clear();
            sttxtbox.Clear();
            stvaltxtbox.Clear();
            orderstocktxtbox.Clear();
            barcodetxtbox.Clear();
            genbarcodetxtbox.Clear();

            // Also clear search textbox and grid
            srchprdctnametxtbox.Clear();
            dataGridViewavlblepkng.DataSource = null;

            // Reset selected packing ID
            selectedPackingId = -1;

            // Multiplier ko reset karein grid mein
            ResetMultipliersInGrid();
        }

        private void ResetMultipliersInGrid()
        {
            if (dataGridView2.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        row.Cells["Multiplier"].Value = 0;
                    }
                }
            }
        }

        #endregion

        #region DataGridView Events

        // DataGridView2 ke liye CellEndEdit event (agar manual multiplier change karna hai)
        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Agar Multiplier column edit ki gayi hai
            if (dataGridView2.Columns[e.ColumnIndex].Name == "Multiplier")
            {
                try
                {
                    // Validate karein ke numeric value hai
                    if (dataGridView2.Rows[e.RowIndex].Cells["Multiplier"].Value != null)
                    {
                        string value = dataGridView2.Rows[e.RowIndex].Cells["Multiplier"].Value.ToString();
                        if (!decimal.TryParse(value, out decimal multiplier))
                        {
                            MessageBox.Show("Please enter a valid number for multiplier",
                                          "Validation Error",
                                          MessageBoxButtons.OK,
                                          MessageBoxIcon.Warning);
                            dataGridView2.Rows[e.RowIndex].Cells["Multiplier"].Value = 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating multiplier: " + ex.Message,
                                  "Error",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Handle grid cell clicks if needed
        }

        private void dataGridViewavlblepkng_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Handle available packings grid clicks if needed
        }

        #endregion

        // Other event handlers (unchanged from original)
        private void ptdcidttxtbox1_TextChanged(object sender, EventArgs e) { }
        private void prdctnametxtbox_TextChanged(object sender, EventArgs e) { }
        private void notxtbox_TextChanged(object sender, EventArgs e) { }
        private void subgrpnamecomboBox_SelectedIndexChanged(object sender, EventArgs e) { }
        private void slctcmpnycombobox_SelectedIndexChanged(object sender, EventArgs e) { }
        private void slctprdctgrpcomboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void purchasepackcombobox_SelectedIndexChanged(object sender, EventArgs e) { }
        private void salepackcombobox_SelectedIndexChanged(object sender, EventArgs e) { }
        private void packinloseweighttxtbox_TextChanged(object sender, EventArgs e) { }
        private void retailpricetxtbox_TextChanged(object sender, EventArgs e) { }
        private void purchasepricetxtbox1_TextChanged(object sender, EventArgs e) { }
        private void salepricetxtbox2_TextChanged(object sender, EventArgs e) { }
        private void purchasedisctxtbox1_TextChanged(object sender, EventArgs e) { }
        private void saledisctxtbox2_TextChanged(object sender, EventArgs e) { }
        private void purchasediscvaltxtbox1_TextChanged(object sender, EventArgs e) { }
        private void putchasediscvaltxtbox2_TextChanged(object sender, EventArgs e) { }
        private void sttxtbox_TextChanged(object sender, EventArgs e) { }
        private void stvaltxtbox_TextChanged(object sender, EventArgs e) { }
        private void orderstocktxtbox_TextChanged(object sender, EventArgs e) { }
        private void barcodetxtbox_TextChanged(object sender, EventArgs e) { }
        private void genbarcodetxtbox_TextChanged(object sender, EventArgs e) { }
    }
}