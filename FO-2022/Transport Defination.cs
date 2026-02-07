using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Transport_Defination : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


        string SelectedName = ""; // For Edit/Delete

        public Transport_Defination()
        {
            InitializeComponent();
            LoadGridData();
        }
        private int SelectedID = 0; // Store selected TransportID

        // LOAD GRID DATA
        //private void LoadGridData()
        //{
        //    using (SqlConnection con = new SqlConnection(connectionString))
        //    {
        //        string query = "SELECT TransportName FROM Transport";
        //        SqlDataAdapter da = new SqlDataAdapter(query, con);
        //        DataTable dt = new DataTable();
        //        da.Fill(dt);

        //        dataGridView1.DataSource = dt;
        //        dataGridView1.Columns[0].HeaderText = "Transport Name";
        //        dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        //    }
        //}
        // LOAD GRID DATA
        private void LoadGridData()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Select both TransportID and TransportName
                string query = "SELECT TransportID, TransportName FROM Transport ORDER BY TransportID";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;

                // Set column headers
                dataGridView1.Columns[0].HeaderText = "Transport ID";
                dataGridView1.Columns[1].HeaderText = "Transport Name";

                // Adjust column width
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        // NEW BUTTON
        private void newbtn_Click(object sender, EventArgs e)
        {
            trnsprtnametxtbox.Clear();
            SelectedName = "";
            trnsprtnametxtbox.Focus();
        }

       

        // SAVE BUTTON (INSERT)
        private void savebtn_Click(object sender, EventArgs e)
        {
            if (trnsprtnametxtbox.Text.Trim() == "")
            {
                MessageBox.Show("Enter transport name");
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // 1. Get new unique TransportID
                int newId = 1;
                string maxIdQuery = "SELECT ISNULL(MAX(TransportID), 0) + 1 FROM Transport";
                SqlCommand maxCmd = new SqlCommand(maxIdQuery, con);
                newId = (int)maxCmd.ExecuteScalar();

                // 2. Insert new record with TransportID and TransportName
                string insertQuery = "INSERT INTO Transport (TransportID, TransportName) VALUES (@id, @name)";
                SqlCommand cmd = new SqlCommand(insertQuery, con);
                cmd.Parameters.AddWithValue("@id", newId);
                cmd.Parameters.AddWithValue("@name", trnsprtnametxtbox.Text.Trim());

                cmd.ExecuteNonQuery();
                con.Close();
            }

            MessageBox.Show("Transport saved!");
            LoadGridData();
            newbtn_Click(sender, e);
        }


        // GRID CLICK → LOAD INTO TEXTBOX
        private void gridview_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        // EDIT BUTTON (UPDATE)
        //private void editbtn_Click(object sender, EventArgs e)
        //{
        //    if (SelectedName == "")
        //    {
        //        MessageBox.Show("Please select record to edit");
        //        return;
        //    }

        //    using (SqlConnection con = new SqlConnection(connectionString))
        //    {
        //        string query = "UPDATE Transport SET TransportName=@newName WHERE TransportName=@oldName";
        //        SqlCommand cmd = new SqlCommand(query, con);

        //        cmd.Parameters.AddWithValue("@newName", trnsprtnametxtbox.Text.Trim());
        //        cmd.Parameters.AddWithValue("@oldName", SelectedName);

        //        con.Open();
        //        cmd.ExecuteNonQuery();
        //        con.Close();
        //    }

        //    MessageBox.Show("Updated successfully!");
        //    LoadGridData();
        //    newbtn_Click(sender, e);
        //}
        private void editbtn_Click(object sender, EventArgs e)
        {
            if (SelectedID == 0)
            {
                MessageBox.Show("Please select a record to edit");
                return;
            }

            if (trnsprtnametxtbox.Text.Trim() == "")
            {
                MessageBox.Show("Enter transport name");
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "UPDATE Transport SET TransportName=@newName WHERE TransportID=@id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@newName", trnsprtnametxtbox.Text.Trim());
                cmd.Parameters.AddWithValue("@id", SelectedID);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            MessageBox.Show("Updated successfully!");
            LoadGridData();
            newbtn_Click(sender, e); // Clear selection
            SelectedID = 0;
        }


        //// REMOVE BUTTON (DELETE)
        //private void removebtn_Click(object sender, EventArgs e)
        //{
        //    if (SelectedName == "")
        //    {
        //        MessageBox.Show("Select record to delete");
        //        return;
        //    }

        //    DialogResult dr = MessageBox.Show("Are you sure to delete?", "Confirm", MessageBoxButtons.YesNo);
        //    if (dr == DialogResult.Yes)
        //    {
        //        using (SqlConnection con = new SqlConnection(connectionString))
        //        {
        //            string query = "DELETE FROM Transport WHERE TransportName=@name";
        //            SqlCommand cmd = new SqlCommand(query, con);
        //            cmd.Parameters.AddWithValue("@name", SelectedName);

        //            con.Open();
        //            cmd.ExecuteNonQuery();
        //            con.Close();
        //        }

        //        MessageBox.Show("Record deleted!");
        //        LoadGridData();
        //        newbtn_Click(sender, e);
        //    }
        //}

        private void removebtn_Click(object sender, EventArgs e)
        {
            if (SelectedID == 0)
            {
                MessageBox.Show("Select a record to delete");
                return;
            }

            DialogResult dr = MessageBox.Show("Are you sure to delete?", "Confirm", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Transport WHERE TransportID=@id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", SelectedID);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                MessageBox.Show("Record deleted!");
                LoadGridData();
                newbtn_Click(sender, e); // Clear selection
                SelectedID = 0;
            }
        }


        // CLEAR BUTTON
        private void clearbtn_Click(object sender, EventArgs e)
        {
            newbtn_Click(sender, e);
        }

        // CLOSE
        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //SelectedName = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                //trnsprtnametxtbox.Text = SelectedName;
                SelectedID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["TransportID"].Value);
                trnsprtnametxtbox.Text = dataGridView1.Rows[e.RowIndex].Cells["TransportName"].Value.ToString();
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            //Mainform1 frm = new Mainform1();
            //frm.Show();
            //this.Hide();
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
    }


