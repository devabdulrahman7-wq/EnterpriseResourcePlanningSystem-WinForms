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

namespace FO_2022.Reports
{
    public partial class Town_rv : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Town_rv()
        {
            InitializeComponent();
        }

        private void Town_rv_Load(object sender, EventArgs e)
        {
            try
            {
                DataSet townsDataSet = GetTownsData();

                //this.reportViewer1.LocalReport.ReportEmbeddedResource = "FO_2022.Reports.TownReport.rdlc";
                this.reportViewer1.LocalReport.ReportPath = "Reports\\Town_report.rdlc";
                this.reportViewer1.LocalReport.DataSources.Clear();

                this.reportViewer1.LocalReport.DataSources.Add(
                    new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", townsDataSet.Tables[0]));

                this.reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading report: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataSet GetTownsData()
        {
            DataSet dataSet = new DataSet();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT TownID, TownName FROM Towns ORDER BY TownName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataSet, "Towns");
                    }
                }
            }

            return dataSet;
        }
    }
}