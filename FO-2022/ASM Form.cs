using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class ASM_Form : Form
    {
        // Connection string
        string connectionString = "Data Source=SQL5110.site4now.net;Initial Catalog=db_ac0035_pos;User Id=db_ac0035_pos_admin;Password=PTCL0614581188!";

        int currentASMId = 0;     // Selected ASMId
        int currentNSMId = 0;     // Selected NSMId
        bool isEditMode = false;  // Edit mode flag

        public ASM_Form()
        {
            InitializeComponent();
        }

        private void ASM_Form_Load(object sender, EventArgs e)
        {
            LoadNSMCombo();

            if (nsmcombobox1.Items.Count > 0)
                nsmcombobox1.SelectedIndex = 0;

            GenerateASMId(); // ensure new ASMId is generated on load

            LoadGrid();
        }

        // ---------------- LOAD NSM TABLE INTO COMBOBOX ----------------
        private void LoadNSMCombo()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT NSMid, NSMName FROM NSM";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                nsmcombobox1.DataSource = dt.Copy();
                nsmcombobox1.DisplayMember = "NSMName";
                nsmcombobox1.ValueMember = "NSMid";

                nsmcombobox.DataSource = dt.Copy();
                nsmcombobox.DisplayMember = "NSMName";
                nsmcombobox.ValueMember = "NSMid";
            }
        }

        // ---------------- LOAD ASMs INTO GRID ----------------
        private void LoadGrid()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string q = "SELECT NSMid, ASMId, ASMName, MobileNo, IsActive FROM ASMs ORDER BY NSMid, ASMId";

                SqlDataAdapter da = new SqlDataAdapter(q, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
            }
        }

        // ---------------- GENERATE NEW ASMId ----------------
        private void GenerateASMId()
        {
            if (nsmcombobox1.SelectedValue == null)
            {
                currentASMId = 0;
                //lblASMId.Text = "0";
                return;
            }

            int nsmId;
            if (!int.TryParse(nsmcombobox1.SelectedValue.ToString(), out nsmId))
            {
                currentASMId = 0;
                //lblASMId.Text = "0";
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string q = "SELECT ISNULL(MAX(ASMId),0) + 1 FROM ASMs WHERE NSMid=@id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@id", nsmId);

                object result = cmd.ExecuteScalar();
                currentASMId = (result != null) ? Convert.ToInt32(result) : 1;
            }

            //lblASMId.Text = currentASMId.ToString(); // show ASMId
        }

        // Auto-generate ASMId when NSM selection changes
        private void nsmcombobox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isEditMode)
                GenerateASMId();
        }

        // ---------------- NEW BUTTON ----------------
        private void newbtn_Click(object sender, EventArgs e)
        {
            ResetFields();
            GenerateASMId();
            MessageBox.Show("You can add a new ASM now.");
        }
        // ---------------- GRID ROW SELECT ----------------
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            if (row.Cells[1].Value == null) return;

            currentASMId = Convert.ToInt32(row.Cells[1].Value);
            currentNSMId = Convert.ToInt32(row.Cells[0].Value);

            nsmcombobox1.SelectedValue = currentNSMId;
            asmnametxtbox.Text = row.Cells[2].Value.ToString();
            mbltxtbox.Text = row.Cells[3].Value.ToString();
            isactivechkbox.Checked = Convert.ToBoolean(row.Cells[4].Value);

            isEditMode = true;
        }

        // ---------------- DELETE ASM ----------------
        private void removebtn_Click(object sender, EventArgs e)
        {
            if (currentASMId <= 0)
            {
                MessageBox.Show("Please select a row first!");
                return;
            }

            DialogResult dr = MessageBox.Show("Do you really want to delete?", "Confirm", MessageBoxButtons.YesNo);
            if (dr != DialogResult.Yes) return;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string q = "DELETE FROM ASMs WHERE NSMid=@nid AND ASMId=@id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@nid", currentNSMId);
                cmd.Parameters.AddWithValue("@id", currentASMId);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Deleted Successfully!");
            LoadGrid();
            ResetFields();
            GenerateASMId();
        }

       

        // ---------------- RESET FIELDS ----------------
        private void ResetFields()
        {
            asmnametxtbox.Clear();
            asmnametxtbox1.Clear();
            mbltxtbox.Clear();
            isactivechkbox.Checked = false;

            currentASMId = 0;
            currentNSMId = 0;

            isEditMode = false;
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Close();
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            asmnametxtbox.Clear();
            asmnametxtbox1.Clear();
            mbltxtbox.Clear();
            isactivechkbox.Checked = false;

        }

        private void editbtn_Click(object sender, EventArgs e)
        {
            if (currentASMId <= 0)
            {
                MessageBox.Show("Please select a row first!");
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string q = "UPDATE ASMs SET ASMName=@n, MobileNo=@m, IsActive=@a WHERE NSMid=@nid AND ASMId=@id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@n", asmnametxtbox1.Text);
                cmd.Parameters.AddWithValue("@m", mbltxtbox.Text);
                cmd.Parameters.AddWithValue("@a", isactivechkbox.Checked);
                cmd.Parameters.AddWithValue("@nid", currentNSMId);
                cmd.Parameters.AddWithValue("@id", currentASMId);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("ASM Updated Successfully!");
            LoadGrid();
            ResetFields();
            GenerateASMId();
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            if (nsmcombobox1.SelectedValue == null || asmnametxtbox.Text.Trim() == "")
            {
                MessageBox.Show("Please fill all necessary fields!");
                return;
            }

            if (currentASMId <= 0)
            {
                GenerateASMId();
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string q = "INSERT INTO ASMs (NSMid, ASMId, ASMName, IsActive, MobileNo) VALUES (@n, @id, @name, @ac, @mob)";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@n", nsmcombobox1.SelectedValue);
                cmd.Parameters.AddWithValue("@id", currentASMId);
                cmd.Parameters.AddWithValue("@name", asmnametxtbox.Text);
                cmd.Parameters.AddWithValue("@ac", isactivechkbox.Checked);
                cmd.Parameters.AddWithValue("@mob", mbltxtbox.Text);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error inserting ASM: " + ex.Message);
                    return;
                }
            }

            MessageBox.Show("ASM Saved Successfully!");
            LoadGrid();
            ResetFields();
            GenerateASMId();
        }
    }
}
