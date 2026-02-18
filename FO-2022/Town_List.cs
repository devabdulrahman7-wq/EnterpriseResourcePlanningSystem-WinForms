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
using FO_2022.Reports;

namespace FO_2022
{
    public partial class Town_List : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Town_List()
        {
            InitializeComponent();
        }

        private void Town_List_Load(object sender, EventArgs e)
        {
            // No initialization needed
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            GenerateReport();
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            GenerateReport();
        }

        private void GenerateReport()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Get data for report
                DataTable dt = GetTownsData();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No towns data found.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare report data sources
                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", dt);

                // Prepare report parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    //{ "ReportTitle", "Town List Report" },
                    //{ "GeneratedDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") }
                };

                // Show report viewer (universal one)
                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Town_report", dataSources);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private DataTable GetTownsData()
        {
            DataTable dt = new DataTable();

            // Define columns matching RDLC dataset
            dt.Columns.Add("TownID", typeof(int));
            dt.Columns.Add("TownName", typeof(string));
            dt.Columns.Add("SectorCount", typeof(int));
            dt.Columns.Add("PartyCount", typeof(int));

            try
            {
                string query = @"
                    SELECT 
                        t.TownID,
                        t.TownName,
                        COUNT(DISTINCT s.SectorID) as SectorCount,
                        COUNT(DISTINCT p.PartyID) as PartyCount
                    FROM Towns t
                    LEFT JOIN Sectors s ON t.TownID = s.TownID
                    LEFT JOIN Parties p ON s.SectorID = p.SectorID
                    GROUP BY t.TownID, t.TownName
                    ORDER BY t.TownName";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching towns data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}