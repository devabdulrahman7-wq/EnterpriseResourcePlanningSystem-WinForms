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
    public partial class Trade_Offers : Form
    {
        private SqlConnection conn;
        private string connStr;
        private DataTable partiesDataTable;
        private int currentOfferID = 0;
        private bool isEditMode = false;

        //public Trade_Offers()
        //{
        //    InitializeComponent();
        //    InitializeDatabaseConnection();
        //    LoadInitialData();
        //    this.Load += new EventHandler(Trade_Offers_Load);
        //    AddInputValidation();
        //    // KeyPress event for Enter key
        //    offeridsearchtxtbox.KeyPress += OfferIdSearchTextBox_KeyPress;

        //    // Grid cell click event
        //    dataGridViewavlblepkng.CellClick += DataGridViewavlblepkng_CellClick;
        //}
        // Constructor mein yeh line add karein
        public Trade_Offers()
        {
            InitializeComponent();
            InitializeDatabaseConnection();
            LoadInitialData();
            this.Load += new EventHandler(Trade_Offers_Load);
            AddInputValidation();
            // KeyPress event for Enter key
            offeridsearchtxtbox.KeyPress += OfferIdSearchTextBox_KeyPress;

            // Grid cell click event
            dataGridViewavlblepkng.CellClick += DataGridViewavlblepkng_CellClick;

            // **YEH NAYA LINE ADD KAREIN**
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
            dataGridView1.CurrentCellDirtyStateChanged += DataGridView1_CurrentCellDirtyStateChanged;
        }

        // Cell value change hone par event handler
        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Agar "Select" column mein change hua hai
                if (dataGridView1.Columns[e.ColumnIndex].Name == "Select")
                {
                    UpdateTotalSelectedParties();
                }
            }
        }

        // Checkbox cell ke liye commit changes
        private void DataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        // Selected parties ka total calculate karein
        private void UpdateTotalSelectedParties()
        {
            if (dataGridView1.Rows.Count > 0 && dataGridView1.Columns.Contains("Select"))
            {
                int selectedCount = 0;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    // Check karein ke cell null toh nahi hai
                    if (row.Cells["Select"].Value != null &&
                        row.Cells["Select"].Value != DBNull.Value)
                    {
                        if (Convert.ToBoolean(row.Cells["Select"].Value))
                        {
                            selectedCount++;
                        }
                    }
                }

                // Total selected parties ko textbox mein show karein
                totalpartiestxtbox.Text = selectedCount.ToString();
            }
        }
        private void DataGridViewavlblepkng_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewRow row = dataGridViewavlblepkng.Rows[e.RowIndex];
                string offerID = row.Cells["OfferID"].Value?.ToString();

                if (!string.IsNullOrEmpty(offerID))
                {
                    // Put the OfferID in search box and trigger search
                    offeridsearchtxtbox.Text = offerID;
                    SearchOffer();
                }
            }
        }
        private void AddInputValidation()
        {
            // Allow only numbers in quantity, bonus, price, discount fields
            qtytxtbox.KeyPress += NumericTextBox_KeyPress;
            bonustxtbox.KeyPress += NumericTextBox_KeyPress;
            pricetxtbox.KeyPress += DecimalTextBox_KeyPress;
            disctxtbox.KeyPress += DecimalTextBox_KeyPress;
            discvaltxtbox.KeyPress += DecimalTextBox_KeyPress;
        }

        private void NumericTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only numbers and backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void DecimalTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow numbers, decimal point, and backspace
            TextBox textBox = sender as TextBox;

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Only allow one decimal point
            if (e.KeyChar == '.' && textBox.Text.Contains('.'))
            {
                e.Handled = true;
            }
        }
        private void InitializeDatabaseConnection()
        {
            // Make sure to add reference to System.Configuration
            // and add using System.Configuration;
            connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            conn = new SqlConnection(connStr);
        }
       

        private void LoadInitialData()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                // Load last OfferID + 1 and disable the textbox
                LoadNextOfferID();

                // Load towns from Towns table
                LoadTowns();

                // Load parties with PartyType 'C' from Parties table
                LoadCustomers();

                // Load sectors from Sectors table
                LoadSectors();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private void LoadNextOfferID()
        {
            try
            {
                // First check if connection is open
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                string query = "SELECT ISNULL(MAX(OfferID), 0) + 1 AS NextOfferID FROM TradeOffer";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        currentOfferID = Convert.ToInt32(result);
                        offerid1txtbox.Text = currentOfferID.ToString();
                        offerid1txtbox.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading OfferID: " + ex.Message);
            }
        }

        private void LoadTowns()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                string query = "SELECT TownID, TownName FROM Towns ORDER BY TownName";
                using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    towncomobobox.DataSource = dt;
                    towncomobobox.DisplayMember = "TownName";
                    towncomobobox.ValueMember = "TownID";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading towns: " + ex.Message);
            }
        }

        private void LoadSectors()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                string query = "SELECT SectorID, SectorName FROM Sectors ORDER BY SectorName";
                using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    sectorcombobox.DataSource = dt;
                    sectorcombobox.DisplayMember = "SectorName";
                    sectorcombobox.ValueMember = "SectorID";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sectors: " + ex.Message);
            }
        }

        //private void LoadCustomers()
        //{
        //    try
        //    {
        //        if (conn.State == ConnectionState.Closed)
        //        {
        //            conn.Open();
        //        }

        //        string query = "SELECT PartyID, PartyName FROM Parties WHERE PartyType = 'C' ORDER BY PartyName";
        //        using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
        //        {
        //            partiesDataTable = new DataTable();
        //            da.Fill(partiesDataTable);

        //            // Bind to Grid
        //            dataGridView1.DataSource = partiesDataTable;

        //            // Display total count
        //            totalpartiestxtbox.Text = partiesDataTable.Rows.Count.ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error loading customers: " + ex.Message);
        //    }
        //}
        private void LoadCustomers()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                string query = "SELECT PartyID, PartyName FROM Parties WHERE PartyType = 'C' ORDER BY PartyName";
                using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                {
                    partiesDataTable = new DataTable();
                    da.Fill(partiesDataTable);

                    // DataTable mein aik aur column add karein checkboxes ke liye
                    partiesDataTable.Columns.Add("Select", typeof(bool));

                    // Har row ke liye default false set karein
                    foreach (DataRow row in partiesDataTable.Rows)
                    {
                        row["Select"] = false;
                    }

                    // DataSource set karein
                    dataGridView1.DataSource = partiesDataTable;

                    // Columns ko manually configure karein
                    ConfigureDataGridViewColumns();

                    // Total parties count show karein
                    UpdateTotalSelectedParties();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customers: " + ex.Message);
            }
        }

        private void ConfigureDataGridViewColumns()
        {
            // Columns ko hide/show karein aur customize karein
            if (dataGridView1.Columns.Count > 0)
            {
                // Columns ke names set karein
                if (dataGridView1.Columns.Contains("PartyID"))
                {
                    dataGridView1.Columns["PartyID"].HeaderText = "Customer ID";
                    dataGridView1.Columns["PartyID"].Width = 100;
                }

                if (dataGridView1.Columns.Contains("PartyName"))
                {
                    dataGridView1.Columns["PartyName"].HeaderText = "Customer Name";
                    dataGridView1.Columns["PartyName"].Width = 250;
                }

                if (dataGridView1.Columns.Contains("Select"))
                {
                    dataGridView1.Columns["Select"].HeaderText = "Select";
                    dataGridView1.Columns["Select"].Width = 70;

                    // Checkbox column type set karein
                    dataGridView1.Columns["Select"].ValueType = typeof(bool);

                    // Column ko readonly nahi banana taki user check/uncheck kar sake
                    dataGridView1.Columns["Select"].ReadOnly = false;
                }

                // DataTable se automatic generated columns ko hide karein
                // (agar zaroorat ho)
                dataGridView1.AutoGenerateColumns = false;
            }
        }

        // Add this method to handle form load event
        private void Trade_Offers_Load(object sender, EventArgs e)
        {
            // Set default dates
            offerdateTimePicker1.Value = DateTime.Now;
            effectivefromTimePicker3.Value = DateTime.Now;
            effectivetodateTimePicker2.Value = DateTime.Now.AddDays(30);
            LoadTradeOffersSummary();

            dataGridView1.DataBindingComplete += dataGridView1_DataBindingComplete;
        }

        private void OfferIdSearchTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // If Enter key is pressed
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true; // Prevent beep sound
                SearchOffer();
            }
        }
       
        private void prdctididsearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(prdctidtxtbox.Text))
                {
                    MessageBox.Show("Please enter Product ID");
                    return;
                }

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                string query = "SELECT ProductName FROM Products WHERE ProductID = @ProductID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductID", prdctidtxtbox.Text);

                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        prdctnametxtbox.Text = result.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Product not found");
                        prdctnametxtbox.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching product: " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        // Auto-calculate discount value when discount percentage changes
        private void disctxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateDiscount();
        }

        // Auto-calculate discount percentage when discount value changes
        private void discvaltxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateDiscountPercentage();
        }

        private void CalculateDiscount()
        {
            try
            {
                if (!string.IsNullOrEmpty(pricetxtbox.Text) && !string.IsNullOrEmpty(disctxtbox.Text))
                {
                    decimal price = Convert.ToDecimal(pricetxtbox.Text);
                    decimal discPercentage = Convert.ToDecimal(disctxtbox.Text);

                    decimal discValue = (price * discPercentage) / 100;
                    discvaltxtbox.Text = discValue.ToString("N2");
                }
            }
            catch
            {
                // Ignore conversion errors
            }
        }

        private void CalculateDiscountPercentage()
        {
            try
            {
                if (!string.IsNullOrEmpty(pricetxtbox.Text) && !string.IsNullOrEmpty(discvaltxtbox.Text))
                {
                    decimal price = Convert.ToDecimal(pricetxtbox.Text);
                    decimal discValue = Convert.ToDecimal(discvaltxtbox.Text);

                    if (price > 0)
                    {
                        decimal discPercentage = (discValue * 100) / price;
                        disctxtbox.Text = discPercentage.ToString("N2");
                    }
                }
            }
            catch
            {
                // Ignore conversion errors
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                partyidtxtbox.Text = row.Cells["PartyID"].Value?.ToString() ?? "";
                searchpartynametxtbox.Text = row.Cells["PartyName"].Value?.ToString() ?? "";
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            totalpartiestxtbox.Text = dataGridView1.SelectedRows.Count.ToString();
        }

        private void enterbtn_Click(object sender, EventArgs e)
        {
            SearchOffer();
        }
        private void SearchOffer()
        {
            try
            {
                if (string.IsNullOrEmpty(offeridsearchtxtbox.Text))
                {
                    MessageBox.Show("Please enter Offer ID to search");
                    return;
                }

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                // Search in TradeOffer table
                string query = @"
            SELECT 
                o.OfferID,
                o.OfferDate,
                o.OfferName,
                o.StartDate,
                o.EndDate,
                o.ProductID,
                p.ProductName,
                o.CompanyID,
                o.Quantity,
                o.Bonus,
                o.Price,
                o.DiscRatio,
                o.DiscValue,
                o.UserNo,
                o.IsWithPrice,
                o.IsOnCompanyOffer,
                o.IsPosted
            FROM TradeOffer o
            INNER JOIN Products p ON o.ProductID = p.ProductID
            WHERE o.OfferID = @OfferID";

                // Create a DataTable to store the results instead of using DataReader
                DataTable offerData = new DataTable();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@OfferID", offeridsearchtxtbox.Text);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(offerData);
                    }
                }

                if (offerData.Rows.Count > 0)
                {
                    DataRow row = offerData.Rows[0];

                    // Fill main form fields
                    offerid1txtbox.Text = row["OfferID"].ToString();
                    offerdateTimePicker1.Value = Convert.ToDateTime(row["OfferDate"]);
                    offernametxtbox.Text = row["OfferName"].ToString();
                    effectivefromTimePicker3.Value = Convert.ToDateTime(row["StartDate"]);
                    effectivetodateTimePicker2.Value = Convert.ToDateTime(row["EndDate"]);
                    prdctidtxtbox.Text = row["ProductID"].ToString();
                    prdctnametxtbox.Text = row["ProductName"].ToString();
                    qtytxtbox.Text = row["Quantity"].ToString();
                    bonustxtbox.Text = row["Bonus"].ToString();
                    pricetxtbox.Text = row["Price"].ToString();
                    disctxtbox.Text = row["DiscRatio"].ToString();
                    discvaltxtbox.Text = row["DiscValue"].ToString();

                    // Note: Ab hum parties load nahi karenge kyunki grid mein sirf TradeOffer ka data dikhana hai
                    // Agar aap parties bhi dekhana chahte hain to alag grid ya method use karein
                    searchoffertxtbox.Text = row["OfferName"].ToString();

                    // Reload trade offers summary
                    LoadTradeOffersSummary();

                    // Now select the row in grid
                    SelectRowInGrid(offeridsearchtxtbox.Text);
                }
                else
                {
                    MessageBox.Show("Offer not found");
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching offer: " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }


        private void newbtn_Click(object sender, EventArgs e)
        {
            isEditMode = false;
            ClearForm();
            LoadNextOfferID();
            EnableFormControls(true);
            LoadTradeOffersSummary();
        }

        private void editbtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(offerid1txtbox.Text) || offerid1txtbox.Text == "0")
            {
                MessageBox.Show("Please select an offer to edit");
                return;
            }

            isEditMode = true;
            EnableFormControls(true);
            MessageBox.Show("Edit mode activated. Make changes and click Save.");
        }

     
        private void savebtn_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (isEditMode)
                {
                    UpdateOffer();
                }
                else
                {
                    InsertOffer();
                }

                // Save selected parties
                SaveOfferParties();

                MessageBox.Show("Offer saved successfully!");

                // Reload the trade offers summary in grid
                LoadTradeOffersSummary();

                ClearForm();
                LoadNextOfferID();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving offer: " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private bool ValidateForm()
        {
            // Check Offer Name
            if (string.IsNullOrEmpty(offernametxtbox.Text))
            {
                MessageBox.Show("Please enter Offer Name");
                offernametxtbox.Focus();
                return false;
            }

            // Check Product ID
            if (string.IsNullOrEmpty(prdctidtxtbox.Text))
            {
                MessageBox.Show("Please enter Product ID");
                prdctidtxtbox.Focus();
                return false;
            }

            // Check Quantity - allow 0 but ensure it's a valid number
            if (string.IsNullOrEmpty(qtytxtbox.Text))
            {
                MessageBox.Show("Please enter Quantity (enter 0 if not applicable)");
                qtytxtbox.Focus();
                return false;
            }

            if (!int.TryParse(qtytxtbox.Text, out int qty))
            {
                MessageBox.Show("Please enter a valid number for Quantity");
                qtytxtbox.Focus();
                qtytxtbox.SelectAll();
                return false;
            }

            // Check Price - allow 0 but ensure it's a valid number
            if (string.IsNullOrEmpty(pricetxtbox.Text))
            {
                MessageBox.Show("Please enter Price (enter 0 if not applicable)");
                pricetxtbox.Focus();
                return false;
            }

            if (!decimal.TryParse(pricetxtbox.Text, out decimal price))
            {
                MessageBox.Show("Please enter a valid number for Price");
                pricetxtbox.Focus();
                pricetxtbox.SelectAll();
                return false;
            }

            // Check Date Validation
            if (effectivefromTimePicker3.Value > effectivetodateTimePicker2.Value)
            {
                MessageBox.Show("Effective From date cannot be after Effective To date");
                effectivefromTimePicker3.Focus();
                return false;
            }

            return true;
        }

        private void InsertOffer()
        {
            string query = @"
                INSERT INTO TradeOffer (
                    OfferID, OfferDate, OfferName, StartDate, EndDate,
                    ProductID, CompanyID, Quantity, Bonus, Price,
                    DiscRatio, DiscValue, UserNo, IsWithPrice,
                    IsOnCompanyOffer, IsPosted
                ) VALUES (
                    @OfferID, @OfferDate, @OfferName, @StartDate, @EndDate,
                    @ProductID, @CompanyID, @Quantity, @Bonus, @Price,
                    @DiscRatio, @DiscValue, @UserNo, @IsWithPrice,
                    @IsOnCompanyOffer, @IsPosted
                )";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                AddOfferParameters(cmd);
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateOffer()
        {
            string query = @"
                UPDATE TradeOffer SET
                    OfferDate = @OfferDate,
                    OfferName = @OfferName,
                    StartDate = @StartDate,
                    EndDate = @EndDate,
                    ProductID = @ProductID,
                    CompanyID = @CompanyID,
                    Quantity = @Quantity,
                    Bonus = @Bonus,
                    Price = @Price,
                    DiscRatio = @DiscRatio,
                    DiscValue = @DiscValue,
                    UserNo = @UserNo,
                    IsWithPrice = @IsWithPrice,
                    IsOnCompanyOffer = @IsOnCompanyOffer,
                    IsPosted = @IsPosted
                WHERE OfferID = @OfferID";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                AddOfferParameters(cmd);
                cmd.ExecuteNonQuery();
            }
        }

        private void AddOfferParameters(SqlCommand cmd)
        {
            // Extract company ID from product ID (assuming first 2 chars)
            string companyID = prdctidtxtbox.Text.Length >= 2 ? prdctidtxtbox.Text.Substring(0, 2) : "01";

            cmd.Parameters.AddWithValue("@OfferID", Convert.ToInt32(offerid1txtbox.Text));
            cmd.Parameters.AddWithValue("@OfferDate", offerdateTimePicker1.Value);
            cmd.Parameters.AddWithValue("@OfferName", offernametxtbox.Text);
            cmd.Parameters.AddWithValue("@StartDate", effectivefromTimePicker3.Value);
            cmd.Parameters.AddWithValue("@EndDate", effectivetodateTimePicker2.Value);
            cmd.Parameters.AddWithValue("@ProductID", prdctidtxtbox.Text);
            cmd.Parameters.AddWithValue("@CompanyID", companyID);

            // Handle Quantity - use 0 if empty
            int quantity = 0;
            if (!string.IsNullOrEmpty(qtytxtbox.Text) && int.TryParse(qtytxtbox.Text, out int qty))
                quantity = qty;
            cmd.Parameters.AddWithValue("@Quantity", quantity);

            // Handle Bonus - use 0 if empty
            int bonus = 0;
            if (!string.IsNullOrEmpty(bonustxtbox.Text) && int.TryParse(bonustxtbox.Text, out int bon))
                bonus = bon;
            cmd.Parameters.AddWithValue("@Bonus", bonus);

            // Handle Price - use 0 if empty
            decimal price = 0;
            if (!string.IsNullOrEmpty(pricetxtbox.Text) && decimal.TryParse(pricetxtbox.Text, out decimal prc))
                price = prc;
            cmd.Parameters.AddWithValue("@Price", price);

            // Handle DiscRatio - use 0 if empty
            decimal discRatio = 0;
            if (!string.IsNullOrEmpty(disctxtbox.Text) && decimal.TryParse(disctxtbox.Text, out decimal dr))
                discRatio = dr;
            cmd.Parameters.AddWithValue("@DiscRatio", discRatio);

            // Handle DiscValue - use 0 if empty
            decimal discValue = 0;
            if (!string.IsNullOrEmpty(discvaltxtbox.Text) && decimal.TryParse(discvaltxtbox.Text, out decimal dv))
                discValue = dv;
            cmd.Parameters.AddWithValue("@DiscValue", discValue);

            cmd.Parameters.AddWithValue("@UserNo", 1); // Default user
            cmd.Parameters.AddWithValue("@IsWithPrice", effecticeprdctchkbox.Checked);
            cmd.Parameters.AddWithValue("@IsOnCompanyOffer", false);
            cmd.Parameters.AddWithValue("@IsPosted", false);
        }


        //        private void SaveOfferParties()
        //{
        //    // First delete existing parties for this offer
        //    string deleteQuery = "DELETE FROM TradeOfferBody WHERE OfferID = @OfferID";
        //    using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
        //    {
        //        if (int.TryParse(offerid1txtbox.Text, out int offerID))
        //        {
        //            deleteCmd.Parameters.AddWithValue("@OfferID", offerID);
        //            deleteCmd.ExecuteNonQuery();
        //        }
        //        else
        //        {
        //            MessageBox.Show("Invalid Offer ID");
        //            return;
        //        }
        //    }

        //    // Insert selected parties
        //    if (!int.TryParse(offerid1txtbox.Text, out int currentOfferID))
        //    {
        //        MessageBox.Show("Invalid Offer ID");
        //        return;
        //    }

        //    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
        //    {
        //        string partyID = row.Cells["PartyID"].Value?.ToString();

        //        if (!string.IsNullOrEmpty(partyID))
        //        {
        //            string insertQuery = @"
        //                INSERT INTO TradeOfferBody (OfferID, PartyID)
        //                VALUES (@OfferID, @PartyID)";

        //            using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
        //            {
        //                insertCmd.Parameters.AddWithValue("@OfferID", currentOfferID);
        //                insertCmd.Parameters.AddWithValue("@PartyID", partyID);
        //                insertCmd.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //}
        private void SaveOfferParties()
        {
            // First delete existing parties for this offer
            string deleteQuery = "DELETE FROM TradeOfferBody WHERE OfferID = @OfferID";
            using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
            {
                if (int.TryParse(offerid1txtbox.Text, out int offerID))
                {
                    deleteCmd.Parameters.AddWithValue("@OfferID", offerID);
                    deleteCmd.ExecuteNonQuery();
                }
                else
                {
                    MessageBox.Show("Invalid Offer ID");
                    return;
                }
            }

            // Insert selected parties (checkboxes ke through)
            if (!int.TryParse(offerid1txtbox.Text, out int currentOfferID))
            {
                MessageBox.Show("Invalid Offer ID");
                return;
            }

            int selectedCount = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Check karein ke checkbox checked hai ya nahi
                if (row.Cells["Select"].Value != null &&
                    row.Cells["Select"].Value != DBNull.Value &&
                    Convert.ToBoolean(row.Cells["Select"].Value))
                {
                    string partyID = row.Cells["PartyID"].Value?.ToString();

                    if (!string.IsNullOrEmpty(partyID))
                    {
                        string insertQuery = @"
                    INSERT INTO TradeOfferBody (OfferID, PartyID)
                    VALUES (@OfferID, @PartyID)";

                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@OfferID", currentOfferID);
                            insertCmd.Parameters.AddWithValue("@PartyID", partyID);
                            insertCmd.ExecuteNonQuery();
                            selectedCount++;
                        }
                    }
                }
            }

            MessageBox.Show($"{selectedCount} parties added to offer.");
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(offerid1txtbox.Text) || offerid1txtbox.Text == "0")
            {
                MessageBox.Show("Please select an offer to delete");
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this offer?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    // Delete from TradeOfferBody first (foreign key constraint)
                    string deleteBodyQuery = "DELETE FROM TradeOfferBody WHERE OfferID = @OfferID";
                    using (SqlCommand deleteBodyCmd = new SqlCommand(deleteBodyQuery, conn))
                    {
                        deleteBodyCmd.Parameters.AddWithValue("@OfferID", Convert.ToInt32(offerid1txtbox.Text));
                        deleteBodyCmd.ExecuteNonQuery();
                    }

                    // Delete from TradeOffer
                    string deleteQuery = "DELETE FROM TradeOffer WHERE OfferID = @OfferID";
                    using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@OfferID", Convert.ToInt32(offerid1txtbox.Text));

                        int rowsAffected = deleteCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Offer deleted successfully!");
                            ClearForm();
                            LoadNextOfferID();

                            // Reload trade offers summary
                            LoadTradeOffersSummary();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting offer: " + ex.Message);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
            LoadTradeOffersSummary();

        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ClearForm()
        {
            offernametxtbox.Clear();
            prdctidtxtbox.Clear();
            prdctnametxtbox.Clear();
            qtytxtbox.Text = "0"; // Set default to 0 instead of empty
            bonustxtbox.Text = "0"; // Set default to 0 instead of empty
            pricetxtbox.Text = "0.00"; // Set default to 0.00 instead of empty
            disctxtbox.Text = "0.00"; // Set default to 0.00 instead of empty
            discvaltxtbox.Text = "0.00"; // Set default to 0.00 instead of empty
            partyidtxtbox.Clear();
            searchpartynametxtbox.Clear();
            searchoffertxtbox.Clear();
            offeridsearchtxtbox.Clear();
            totalpartiestxtbox.Clear();
            effecticeprdctchkbox.Checked = false;

            dataGridViewavlblepkng.DataSource = null;

            // Reset to current date
            offerdateTimePicker1.Value = DateTime.Now;
            effectivefromTimePicker3.Value = DateTime.Now;
            effectivetodateTimePicker2.Value = DateTime.Now.AddDays(30);

            // Clear selection in grid
            dataGridView1.ClearSelection();

            // Reload next OfferID
            LoadNextOfferID();
            if (partiesDataTable != null && partiesDataTable.Columns.Contains("Select"))
            {
                foreach (DataRow row in partiesDataTable.Rows)
                {
                    row["Select"] = false;
                }

                // Refresh grid
                if (dataGridView1.InvokeRequired)
                {
                    dataGridView1.Invoke(new MethodInvoker(delegate {
                        dataGridView1.Refresh();
                        UpdateTotalSelectedParties();
                    }));
                }
                else
                {
                    dataGridView1.Refresh();
                    UpdateTotalSelectedParties();
                }
            }
        }
        // Select All button ke liye (agar aap add karna chahein)
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells["Select"].Value = true;
            }
            UpdateTotalSelectedParties();
        }

        // Unselect All button ke liye
        private void btnUnselectAll_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells["Select"].Value = false;
            }
            UpdateTotalSelectedParties();
        }

        private void SelectRowInGrid(string offerID)
        {
            try
            {
                if (dataGridViewavlblepkng.Rows.Count == 0)
                    return;

                // Find the row with matching OfferID
                foreach (DataGridViewRow row in dataGridViewavlblepkng.Rows)
                {
                    if (row.Cells["OfferID"].Value != null &&
                        row.Cells["OfferID"].Value.ToString() == offerID)
                    {
                        // Select the row
                        row.Selected = true;

                        // Make it visible by scrolling to it
                        dataGridViewavlblepkng.FirstDisplayedScrollingRowIndex = row.Index;

                        // Highlight the row
                        dataGridViewavlblepkng.CurrentCell = row.Cells[0];

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                // Silent fail - don't show error if selection fails
                Console.WriteLine("Error selecting row: " + ex.Message);
            }
        }

        private void EnableFormControls(bool enable)
        {
            offernametxtbox.Enabled = enable;
            prdctidtxtbox.Enabled = enable;
            qtytxtbox.Enabled = enable;
            bonustxtbox.Enabled = enable;
            pricetxtbox.Enabled = enable;
            disctxtbox.Enabled = enable;
            discvaltxtbox.Enabled = enable;
            effecticeprdctchkbox.Enabled = enable;
            effectivefromTimePicker3.Enabled = enable;
            effectivetodateTimePicker2.Enabled = enable;
            dataGridView1.Enabled = enable;
        }

        // Update the Form_Load event handler in Designer.cs or add this
      
        // Other event handlers
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
       
        private void LoadTradeOffersSummary()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                string query = @"
            SELECT 
                OfferID,
                OfferDate,
                OfferName
            FROM TradeOffer 
            ORDER BY OfferID DESC";

                using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Store current selection before rebinding
                    string currentOfferID = offeridsearchtxtbox.Text;

                    // Bind to grid
                    dataGridViewavlblepkng.DataSource = dt;

                    // Format the date column
                    if (dataGridViewavlblepkng.Columns["OfferDate"] != null)
                    {
                        dataGridViewavlblepkng.Columns["OfferDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    }

                    // Set column headers if needed
                    if (dataGridViewavlblepkng.Columns["OfferID"] != null)
                        dataGridViewavlblepkng.Columns["OfferID"].HeaderText = "Offer ID";
                    if (dataGridViewavlblepkng.Columns["OfferDate"] != null)
                        dataGridViewavlblepkng.Columns["OfferDate"].HeaderText = "Offer Date";
                    if (dataGridViewavlblepkng.Columns["OfferName"] != null)
                        dataGridViewavlblepkng.Columns["OfferName"].HeaderText = "Offer Name";

                    // Restore selection if there was one
                    if (!string.IsNullOrEmpty(currentOfferID))
                    {
                        SelectRowInGrid(currentOfferID);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading trade offers: " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
        // Empty event handlers for controls that don't need functionality
        private void offerdateTimePicker1_ValueChanged(object sender, EventArgs e) { }
        private void offerid1txtbox_TextChanged(object sender, EventArgs e) { }
        private void prdctidtxtbox_TextChanged(object sender, EventArgs e) { }
        private void offernametxtbox_TextChanged(object sender, EventArgs e) { }
        private void effectivetodateTimePicker2_ValueChanged(object sender, EventArgs e) { }
        private void effectivefromTimePicker3_ValueChanged(object sender, EventArgs e) { }
        private void prdctnametxtbox_TextChanged(object sender, EventArgs e) { }
        private void qtytxtbox_TextChanged(object sender, EventArgs e) { }
        private void bonustxtbox_TextChanged(object sender, EventArgs e) { }
        private void pricetxtbox_TextChanged(object sender, EventArgs e) { }
        private void towncomobobox_SelectedIndexChanged(object sender, EventArgs e) { }
        private void sectorcombobox_SelectedIndexChanged(object sender, EventArgs e) { }
        private void partyidtxtbox_TextChanged(object sender, EventArgs e) { }
        private void searchpartynametxtbox_TextChanged(object sender, EventArgs e) { }
        private void effecticeprdctchkbox_CheckedChanged(object sender, EventArgs e) { }
        private void offeridsearchtxtbox_TextChanged(object sender, EventArgs e) { }
        private void searchoffertxtbox_TextChanged(object sender, EventArgs e) { }
        private void dataGridViewavlblepkng_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Data binding complete hone par total update karein
            UpdateTotalSelectedParties();
        }
    }
}