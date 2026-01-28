using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Define_Packings : Form
    {
        //string con = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        //SqlConnection con = new SqlConnection(@"Data Source=SQL5110.site4now.net;Initial Catalog=db_ac0035_pos;User Id=db_ac0035_pos_admin;Password=PTCL0614581188!");
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

        public Define_Packings()
        {
            InitializeComponent();
            LoadGrid(); // Load DataGrid when form opens
        }

        // ========== CLOSE BUTTON ==========
        private void closebtn_Click(object sender, EventArgs e)
        {
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
        }

        // ========== LOAD DATA INTO GRID ==========
        private void LoadGrid()
        {
            try
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Packings", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
        }

        // ========== NEW BUTTON ==========
        private void newbtn_Click(object sender, EventArgs e)
        {
            pkngidtxtbox.Enabled = true;
            pkngnametxtbox.Enabled = true;
            Abbrevationtxt.Enabled = true;
            pkngidtxtbox.Clear();
            pkngnametxtbox.Clear();
            Abbrevationtxt.Clear();
            pkngidtxtbox.Focus();
        }

        // ========== SAVE BUTTON ==========
        private void savebtn_Click(object sender, EventArgs e)
        {
            if (pkngidtxtbox.Text == "" || pkngnametxtbox.Text == "")
            {
                MessageBox.Show("Please enter Packing ID and Packing Name!");
                return;
            }

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Packings (PackingID, PackingName, Abbrevation) VALUES (@id, @name, @abbrevation)", con);
                cmd.Parameters.AddWithValue("@id", pkngidtxtbox.Text);
                cmd.Parameters.AddWithValue("@name", pkngnametxtbox.Text);
                cmd.Parameters.AddWithValue("@abbrevation", Abbrevationtxt.Text);
                cmd.ExecuteNonQuery();
                con.Close();

                MessageBox.Show("Packing saved successfully!");
                LoadGrid();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
        }

        // ========== CLEAR BUTTON ==========
        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            pkngidtxtbox.Clear();
            pkngnametxtbox.Clear();
            Abbrevationtxt.Clear();
            pkngidtxtbox.Focus();
        }

        private void editbtn_Click(object sender, EventArgs e)
        {
            if (pkngidtxtbox.Text == "")
            {
                MessageBox.Show("Please select a record to edit!");
                return;
            }

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "UPDATE Packings SET PackingName=@name, Abbrevation=@abb WHERE PackingID=@id", con);

                cmd.Parameters.AddWithValue("@id", pkngidtxtbox.Text);
                cmd.Parameters.AddWithValue("@name", pkngnametxtbox.Text);
                cmd.Parameters.AddWithValue("@abb", Abbrevationtxt.Text);

                cmd.ExecuteNonQuery();
                con.Close();

                MessageBox.Show("Packing updated successfully!");

                LoadGrid();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            if (pkngidtxtbox.Text == "")
            {
                MessageBox.Show("Please select a record to delete!");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this packing?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.No)
                return;

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Packings WHERE PackingID=@id", con);
                cmd.Parameters.AddWithValue("@id", pkngidtxtbox.Text);
                cmd.ExecuteNonQuery();
                con.Close();

                MessageBox.Show("Packing deleted successfully!");

                LoadGrid();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // avoid header row
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                pkngidtxtbox.Text = row.Cells["PackingID"].Value.ToString();
                pkngnametxtbox.Text = row.Cells["PackingName"].Value.ToString();
                Abbrevationtxt.Text = row.Cells["Abbrevation"].Value.ToString();

                pkngidtxtbox.Enabled = false; // ID edit nahi hoti
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            //Mainform1 mainform1 = new Mainform1();
            //mainform1.Show();
            //this.Hide();
            this.Close();
        }
    }
}
