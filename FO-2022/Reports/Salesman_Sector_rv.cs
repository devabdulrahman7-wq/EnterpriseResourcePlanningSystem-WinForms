using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace FO_2022.Reports
{
    public partial class Salesman_Sector_rv : Form
    {
        public DataTable ReportData { get; set; }

        public Salesman_Sector_rv()
        {
            InitializeComponent();
        }

        private void Salesman_Sector_rv_Load(object sender, EventArgs e)
        {

            try
            {
                // 📌 STEP 1: Clear previous data
                reportViewer1.LocalReport.DataSources.Clear();

                // 📌 STEP 2: TRY MULTIPLE APPROACHES TO LOAD REPORT

                bool reportLoaded = false;

                // ----- APPROACH 1: Embedded Resource (as per your project) -----
                string[] possibleResourceNames = new string[]
                {
                    "FO_2022.Reports.Salesman_Sector.rdlc",           // Full namespace + folder
                    "FO_2022.Salesman_Sector.rdlc",                   // Without Reports folder
                    "Salesman_Sector.rdlc"                            // Just file name
                };

                foreach (string resourceName in possibleResourceNames)
                {
                    try
                    {
                        reportViewer1.LocalReport.ReportEmbeddedResource = resourceName;
                        // Try to get parameters - agar exception na aaye to resource exists
                        reportViewer1.LocalReport.GetParameters();
                        reportLoaded = true;
                        break;
                    }
                    catch
                    {
                        // Continue to next resource name
                    }
                }

                // ----- APPROACH 2: File-based if embedded fails -----
                if (!reportLoaded)
                {
                    string filePath = Path.Combine(Application.StartupPath, "Reports", "Salesman_Sector.rdlc");

                    if (!File.Exists(filePath))
                        filePath = Path.Combine(Application.StartupPath, "Salesman_Sector.rdlc");

                    if (File.Exists(filePath))
                    {
                        reportViewer1.LocalReport.ReportPath = filePath;
                        reportLoaded = true;
                    }
                }

                // 📌 STEP 3: If still not loaded, show error with details
                if (!reportLoaded)
                {
                    string errorMsg = "Report could not be loaded. Tried:\n";
                    foreach (string name in possibleResourceNames)
                        errorMsg += $"• Embedded: {name}\n";
                    errorMsg += $"• File: {Path.Combine(Application.StartupPath, "Reports", "Salesman_Sector.rdlc")}\n";
                    errorMsg += $"• File: {Path.Combine(Application.StartupPath, "Salesman_Sector.rdlc")}";

                    MessageBox.Show(errorMsg, "Report Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // 📌 STEP 4: Add data source
                if (ReportData == null || ReportData.Rows.Count == 0)
                {
                    MessageBox.Show("No data available for report.", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                    return;
                }

                ReportDataSource rds = new ReportDataSource("DataSet1", ReportData);
                reportViewer1.LocalReport.DataSources.Add(rds);

                // 📌 STEP 5: Refresh report
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }
    }
}