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
//    public partial class SO_s_ASM_Wise : Form
//    {
//        public SO_s_ASM_Wise()
//        {
//            InitializeComponent();
//        }

//        private void pictureBox2_Click(object sender, EventArgs e)
//        {
//            this.Close();
//        }

//        private void nsmcombobox1_SelectedIndexChanged(object sender, EventArgs e)
//        {

//        }

//        private void rmcombobox1_SelectedIndexChanged(object sender, EventArgs e)
//        {

//        }

//        private void salesmancombobox_SelectedIndexChanged(object sender, EventArgs e)
//        {

//        }

//        private void salesofficertxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void mbltxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void isactivechkbox_CheckedChanged(object sender, EventArgs e)
//        {

//        }

//        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {

//        }

//        private void nsmcombobox_SelectedIndexChanged(object sender, EventArgs e)
//        {

//        }

//        private void rmcombobox_SelectedIndexChanged(object sender, EventArgs e)
//        {

//        }

//        private void btnSave_Click(object sender, EventArgs e)
//        {

//        }

//        private void btnClear_Click(object sender, EventArgs e)
//        {

//        }

//        private void btnClose_Click(object sender, EventArgs e)
//        {

//        }

//        private void btnNew_Click(object sender, EventArgs e)
//        {

//        }

//        private void btnEdit_Click(object sender, EventArgs e)
//        {

//        }

//        private void btnRemove_Click(object sender, EventArgs e)
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

namespace FO_2022
{
    public partial class SO_s_ASM_Wise : Form
    {
        // Database connection string
        private string connectionString = "Data Source=SQL5110.site4now.net;Initial Catalog=db_ac0035_pos;User Id=db_ac0035_pos_admin;Password=PTCL0614581188!";

        // Variables
        private DataTable spoDataTable;
        private int selectedSPOId = -1;

        public SO_s_ASM_Wise()
        {
            InitializeComponent();
            LoadComboBoxes();
            LoadSPOGrid();
            ClearForm();
        }

        // Database Methods
        private DataTable GetData(string query)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        private int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteNonQuery();
            }
        }

        // Load Data Methods
        private void LoadComboBoxes()
        {
            try
            {
                // Load NSM into nsmcombobox1
                DataTable nsmDt = GetData("SELECT NSMid, NSMName FROM NSM ORDER BY NSMName");
                nsmcombobox1.DataSource = nsmDt;
                nsmcombobox1.DisplayMember = "NSMName";
                nsmcombobox1.ValueMember = "NSMid";

                nsmcombobox.DataSource = nsmDt;
                nsmcombobox.DisplayMember = "NSMName";
                nsmcombobox.ValueMember = "NSMid";

                // Load ASM based on selected NSM
                LoadASMComboBox();
                

                // Load Salesmen
                DataTable salesmenDt = GetData("SELECT SalesmanID, SalesmanName FROM Salesmen ORDER BY SalesmanName");
                salesmancombobox.DataSource = salesmenDt;
                salesmancombobox.DisplayMember = "SalesmanName";
                salesmancombobox.ValueMember = "SalesmanID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadASMComboBox()
        {
            try
            {
                if (nsmcombobox1.SelectedValue != null)
                {
                    string nsmId = nsmcombobox1.SelectedValue.ToString();
                    DataTable asmDt = GetData($"SELECT ASMId, ASMName FROM ASMs WHERE NSMid = '{nsmId}' ORDER BY ASMName");
                    rmcombobox1.DataSource = asmDt;
                    rmcombobox1.DisplayMember = "ASMName";
                    rmcombobox1.ValueMember = "ASMId";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading ASM data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
       

        private void LoadSPOGrid()
        {
            try
            {
                string query = @"SELECT 
                                s.SPOId, 
                                s.SPOName, 
                                s.MobileNo, 
                                s.IsActive,
                                n.NSMName, 
                                a.ASMName, 
                                sm.SalesmanName
                                FROM SPOs s
                                LEFT JOIN NSM n ON s.NSMid = n.NSMid
                                LEFT JOIN ASMs a ON s.NSMid = a.NSMid AND s.ASMID = a.ASMId
                                LEFT JOIN Salesmen sm ON s.SalesManId = sm.SalesmanID
                                ORDER BY s.SPOName";

                spoDataTable = GetData(query);
                dataGridView1.DataSource = spoDataTable;

                // Hide SPOId column
                if (dataGridView1.Columns.Contains("SPOId"))
                    dataGridView1.Columns["SPOId"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading SPO grid: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Form Operations
        private void ClearForm()
        {
            selectedSPOId = -1;
            salesofficertxtbox.Text = "";
            mbltxtbox.Text = "";
            isactivechkbox.Checked = false;

            if (nsmcombobox1.Items.Count > 0)
                nsmcombobox1.SelectedIndex = 0;
            if (rmcombobox1.Items.Count > 0)
                rmcombobox1.SelectedIndex = 0;
            if (salesmancombobox.Items.Count > 0)
                salesmancombobox.SelectedIndex = 0;

            salesofficertxtbox.Focus();
        }

        private void SaveSPO()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(salesofficertxtbox.Text))
                {
                    MessageBox.Show("Please enter Sales Officer Name", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (nsmcombobox1.SelectedValue == null)
                {
                    MessageBox.Show("Please select NSM", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string nsmId = nsmcombobox1.SelectedValue.ToString();
                string asmId = rmcombobox1.SelectedValue != null ? rmcombobox1.SelectedValue.ToString() : "NULL";
                string salesmanId = salesmancombobox.SelectedValue != null ? salesmancombobox.SelectedValue.ToString() : "NULL";

                string query = @"INSERT INTO SPOs (NSMid, SPOName, MobileNo, IsActive, ASMID, SalesManId) 
                                VALUES (@NSMid, @SPOName, @MobileNo, @IsActive, @ASMID, @SalesManId)";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@NSMid", nsmId),
                    new SqlParameter("@SPOName", salesofficertxtbox.Text.Trim()),
                    new SqlParameter("@MobileNo", mbltxtbox.Text.Trim()),
                    new SqlParameter("@IsActive", isactivechkbox.Checked),
                    new SqlParameter("@ASMID", string.IsNullOrEmpty(asmId) || asmId == "NULL" ? DBNull.Value : (object)int.Parse(asmId)),
                    new SqlParameter("@SalesManId", string.IsNullOrEmpty(salesmanId) || salesmanId == "NULL" ? DBNull.Value : (object)salesmanId)
                };

                int result = ExecuteNonQuery(query, parameters);
                if (result > 0)
                {
                    MessageBox.Show("Sales Officer saved successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSPOGrid();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Failed to save Sales Officer", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving SPO: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateSPO()
        {
            try
            {
                if (selectedSPOId == -1)
                {
                    MessageBox.Show("Please select a record to update", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string nsmId = nsmcombobox1.SelectedValue.ToString();
                string asmId = rmcombobox1.SelectedValue != null ? rmcombobox1.SelectedValue.ToString() : "NULL";
                string salesmanId = salesmancombobox.SelectedValue != null ? salesmancombobox.SelectedValue.ToString() : "NULL";

                string query = @"UPDATE SPOs SET 
                                NSMid = @NSMid,
                                SPOName = @SPOName,
                                MobileNo = @MobileNo,
                                IsActive = @IsActive,
                                ASMID = @ASMID,
                                SalesManId = @SalesManId
                                WHERE SPOId = @SPOId";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@NSMid", nsmId),
                    new SqlParameter("@SPOName", salesofficertxtbox.Text.Trim()),
                    new SqlParameter("@MobileNo", mbltxtbox.Text.Trim()),
                    new SqlParameter("@IsActive", isactivechkbox.Checked),
                    new SqlParameter("@ASMID", string.IsNullOrEmpty(asmId) || asmId == "NULL" ? DBNull.Value : (object)int.Parse(asmId)),
                    new SqlParameter("@SalesManId", string.IsNullOrEmpty(salesmanId) || salesmanId == "NULL" ? DBNull.Value : (object)salesmanId),
                    new SqlParameter("@SPOId", selectedSPOId)
                };

                int result = ExecuteNonQuery(query, parameters);
                if (result > 0)
                {
                    MessageBox.Show("Sales Officer updated successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSPOGrid();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Failed to update Sales Officer", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating SPO: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteSPO()
        {
            try
            {
                if (selectedSPOId == -1)
                {
                    MessageBox.Show("Please select a record to delete", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this Sales Officer?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    string query = "DELETE FROM SPOs WHERE SPOId = @SPOId";
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@SPOId", selectedSPOId)
                    };

                    int result = ExecuteNonQuery(query, parameters);
                    if (result > 0)
                    {
                        MessageBox.Show("Sales Officer deleted successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadSPOGrid();
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete Sales Officer", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting SPO: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Event Handlers
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //private void nsmcombobox1_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    LoadASMComboBox();
          
           
        //}

        private void rmcombobox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ASM selection changed - aapko kuch special karna ho to
        }

        private void salesmancombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Salesman selection changed - aapko kuch special karna ho to
        }

        private void salesofficertxtbox_TextChanged(object sender, EventArgs e)
        {
            // Sales Officer text changed - aapko kuch validation karna ho to
        }

        private void mbltxtbox_TextChanged(object sender, EventArgs e)
        {
            // Mobile text changed - aapko kuch validation karna ho to
        }

        private void isactivechkbox_CheckedChanged(object sender, EventArgs e)
        {
            // Active checkbox changed
        }

        //private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    // Yeh CellContentClick event hai, ise CellClick mein change karna better hai
        //    // Lekin aap ise hi use karna chahte hain to yeh code add karein:
        //    if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
        //    {
        //        DataGridViewCellClick(sender, e);
        //    }
        //}

        // Alag method for cell click
        private void DataGridViewCellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                    if (row.Cells["SPOId"].Value != null)
                    {
                        selectedSPOId = Convert.ToInt32(row.Cells["SPOId"].Value);
                    }

                    if (row.Cells["SPOName"].Value != null)
                    {
                        salesofficertxtbox.Text = row.Cells["SPOName"].Value.ToString();
                    }

                    if (row.Cells["MobileNo"].Value != null)
                    {
                        mbltxtbox.Text = row.Cells["MobileNo"].Value.ToString();
                    }

                    if (row.Cells["IsActive"].Value != null)
                    {
                        isactivechkbox.Checked = Convert.ToBoolean(row.Cells["IsActive"].Value);
                    }

                    // Set NSM
                    if (row.Cells["NSMName"].Value != null && row.Cells["NSMName"].Value != DBNull.Value)
                    {
                        nsmcombobox1.Text = row.Cells["NSMName"].Value.ToString();
                    }

                    // Set ASM
                    if (row.Cells["ASMName"].Value != null && row.Cells["ASMName"].Value != DBNull.Value)
                    {
                        rmcombobox1.Text = row.Cells["ASMName"].Value.ToString();
                    }

                    // Set Salesman
                    if (row.Cells["SalesmanName"].Value != null && row.Cells["SalesmanName"].Value != DBNull.Value)
                    {
                        salesmancombobox.Text = row.Cells["SalesmanName"].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading selected record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       
        // Button Click Events
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveSPO();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            UpdateSPO();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            DeleteSPO();
        }

        // Form Load Event (agar aapko chahiye to)
        private void SO_s_ASM_Wise_Load(object sender, EventArgs e)
        {
            LoadNSMComboBox();

        }
        private void LoadNSMComboBox()
        {
            try
            {
                DataTable dt = GetData("SELECT NSMid, NSMName FROM NSM ORDER BY NSMName");

                nsmcombobox1.DataSource = dt;
                nsmcombobox1.DisplayMember = "NSMName";
                nsmcombobox1.ValueMember = "NSMid";
                nsmcombobox1.SelectedIndex = -1;

                rmcombobox1.DataSource = null; // clear ASM until NSM select
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0) return;

                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // SPOId
                selectedSPOId = Convert.ToInt32(row.Cells["SPOId"].Value);

                // TextBoxes
                salesofficertxtbox.Text = row.Cells["SPOName"].Value?.ToString();
                mbltxtbox.Text = row.Cells["MobileNo"].Value?.ToString();
                isactivechkbox.Checked = Convert.ToBoolean(row.Cells["IsActive"].Value);

                // ComboBoxes (Text set karo – Value nahi)
                nsmcombobox1.Text = row.Cells["NSMName"].Value?.ToString();
                rmcombobox1.Text = row.Cells["ASMName"].Value?.ToString();
                salesmancombobox.Text = row.Cells["SalesmanName"].Value?.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void nsmcombobox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (nsmcombobox1.SelectedValue == null)
                return;

            if (nsmcombobox1.SelectedValue is DataRowView)
                return;

            LoadASMComboBox(Convert.ToInt32(nsmcombobox1.SelectedValue));
        }
        //private void LoadASMComboBox(int nsmId)
        //{
        //    try
        //    {
        //        string query = "SELECT ASMId, ASMName FROM ASMs WHERE NSMid = @NSMid ORDER BY ASMName";

        //        using (SqlConnection con = new SqlConnection(connectionString))
        //        {
        //            SqlCommand cmd = new SqlCommand(query, con);
        //            cmd.Parameters.AddWithValue("@NSMid", nsmId);

        //            SqlDataAdapter da = new SqlDataAdapter(cmd);
        //            DataTable dt = new DataTable();
        //            da.Fill(dt);

        //            rmcombobox1.DataSource = dt;
        //            rmcombobox1.DisplayMember = "ASMName";
        //            rmcombobox1.ValueMember = "ASMId";
        //            rmcombobox1.SelectedIndex = -1;

        //            //rmcombobox.DataSource = dt;
        //            //rmcombobox.DisplayMember = "ASMName";
        //            //rmcombobox.ValueMember = "ASMId";
        //            //rmcombobox.SelectedIndex = -1;

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}
        private void LoadASMComboBox(int nsmId)
        {
            try
            {
                string query = "SELECT ASMId, ASMName FROM ASMs WHERE NSMid = @NSMid ORDER BY ASMName";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@NSMid", nsmId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    rmcombobox1.DataSource = dt;
                    rmcombobox1.DisplayMember = "ASMName";
                    rmcombobox1.ValueMember = "ASMId";
                    rmcombobox1.SelectedIndex = -1;

                    rmcombobox.DataSource = dt;
                    rmcombobox.DisplayMember = "ASMName";
                    rmcombobox.ValueMember = "ASMId";
                    rmcombobox.SelectedIndex = -1;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void nsmcombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (nsmcombobox.SelectedValue == null)
                return;

            if (nsmcombobox.SelectedValue is DataRowView)
                return;

            LoadASMComboBox(Convert.ToInt32(nsmcombobox.SelectedValue));
        }


        private void rmcombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rmcombobox.SelectedValue == null)
                return;

            if (rmcombobox.SelectedValue is DataRowView)
                return;

            LoadASMComboBox(Convert.ToInt32(rmcombobox.SelectedValue));
        }

        private void rmcombobox_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void rmcombobox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}








