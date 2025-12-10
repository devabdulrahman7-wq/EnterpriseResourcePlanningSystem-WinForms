using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Define_Customer_Category : Form
    {
        string connectionString = "Data Source=SQL5110.site4now.net;Initial Catalog=db_ac0035_pos;User Id=db_ac0035_pos_admin;Password=PTCL0614581188!";

        int selectedID = 0;   // internal use only

        public Define_Customer_Category()
        {
            InitializeComponent();
        }

        private void Define_Customer_Category_Load(object sender, EventArgs e)
        {
            LoadGrid();
        }

        // =====================================================
        // LOAD GRID
        // =====================================================
        private void LoadGrid()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string q = "SELECT CategoryId, CategoryName FROM Category ORDER BY CategoryId";
                SqlDataAdapter da = new SqlDataAdapter(q, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;

                // safe way: remove if columns already exist
                if (dataGridView1.Columns["CategoryId"] != null)
                    dataGridView1.Columns["CategoryId"].HeaderText = "Category ID";
                if (dataGridView1.Columns["CategoryName"] != null)
                    dataGridView1.Columns["CategoryName"].HeaderText = "Category Name";
            }
        }


        // =====================================================
        // CLEAR
        // =====================================================
        private void clearbtn_Click(object sender, EventArgs e)
        {
            sctrnametxtbox.Text = "";
            selectedID = 0;
        }

        // =====================================================
        // NEW BUTTON
        // =====================================================
        private void newbtn_Click(object sender, EventArgs e)
        {
            clearbtn.PerformClick();
            sctrnametxtbox.Focus();
        }

        // =====================================================
        // GET NEXT ID
        // =====================================================
        private int GetNextID()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string q = "SELECT ISNULL(MAX(CategoryId), 0) + 1 FROM Category";
                SqlCommand cmd = new SqlCommand(q, conn);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // =====================================================
        // SAVE = INSERT
        // =====================================================
        private void savebtn_Click(object sender, EventArgs e)
        {
            if (sctrnametxtbox.Text == "")
            {
                MessageBox.Show("Please enter category name.");
                return;
            }

            int newID = GetNextID(); // auto generate

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string q = "INSERT INTO Category (CategoryId, CategoryName) VALUES (@id, @name)";
                SqlCommand cmd = new SqlCommand(q, conn);
                cmd.Parameters.AddWithValue("@id", newID);
                cmd.Parameters.AddWithValue("@name", sctrnametxtbox.Text);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Saved Successfully!");
            LoadGrid();
            clearbtn.PerformClick();
        }

        // =====================================================
        // EDIT = UPDATE
        // =====================================================
        private void editbtn_Click(object sender, EventArgs e)
        {
            if (selectedID == 0)
            {
                MessageBox.Show("Select a record first!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string q = "UPDATE Category SET CategoryName=@name WHERE CategoryId=@id";
                SqlCommand cmd = new SqlCommand(q, conn);
                cmd.Parameters.AddWithValue("@name", sctrnametxtbox.Text);
                cmd.Parameters.AddWithValue("@id", selectedID);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Updated Successfully!");
            LoadGrid();
        }

        // =====================================================
        // REMOVE = DELETE
        // =====================================================
        private void removebtn_Click(object sender, EventArgs e)
        {
            if (selectedID == 0)
            {
                MessageBox.Show("Select a record first!");
                return;
            }

            DialogResult dr = MessageBox.Show(
                "Are you sure you want to delete this category?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (dr == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string q = "DELETE FROM Category WHERE CategoryId=@id";
                    SqlCommand cmd = new SqlCommand(q, conn);
                    cmd.Parameters.AddWithValue("@id", selectedID);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Deleted Successfully!");
                LoadGrid();
                clearbtn.PerformClick();
            }
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            Mainform1 mainform1 = new Mainform1();
            mainform1.Show();
            this.Hide();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                selectedID = Convert.ToInt32(row.Cells[0].Value);   // CategoryId
                sctrnametxtbox.Text = row.Cells[1].Value.ToString(); // CategoryName
            }
        }
    }
}
