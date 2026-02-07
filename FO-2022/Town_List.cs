using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Town_List : Form
    {
        public Town_List()
        {
            InitializeComponent();
        }

        private void Town_List_Load(object sender, EventArgs e)
        {
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                Reports.Town_rv reportViewer = new Reports.Town_rv();
                reportViewer.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening report: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            try
            {
                Reports.Town_rv reportViewer = new Reports.Town_rv();
                reportViewer.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening report for printing: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
            //Mainform1 mf = new Mainform1();
            //mf.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}