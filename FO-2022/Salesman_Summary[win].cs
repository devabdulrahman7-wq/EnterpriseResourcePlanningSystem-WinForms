//using System;
//using System.Data;
//using System.Windows.Forms;
//using Microsoft.Reporting.WinForms;

//namespace FO_2022
//{
//    public partial class Salesman_Summary_win_ : Form
//    {
//        public Salesman_Summary_win_()
//        {
//            InitializeComponent();
//        }

//        // ========== ADD THIS METHOD ==========
//        public void SetReportData(DataTable dt, DateTime fromDate, DateTime toDate, string salesmanName)
//        {
//            try
//            {
//                // Clear previous data sources
//                reportViewer1.LocalReport.DataSources.Clear();

//                // IMPORTANT: Update this path to match your RDLC file location
//                reportViewer1.LocalReport.ReportEmbeddedResource = "FO_2022.SalesmanSummaryReport.rdlc";

//                // Connect DataTable to Report
//                ReportDataSource rds = new ReportDataSource("SalesmanSummary", dt);
//                reportViewer1.LocalReport.DataSources.Add(rds);

//                // Set report parameters
//                ReportParameter[] parameters =
//                {
//                    new ReportParameter("FromDate", fromDate.ToString("dd/MM/yyyy")),
//                    new ReportParameter("ToDate", toDate.ToString("dd/MM/yyyy")),
//                    new ReportParameter("SalesmanName", salesmanName),
//                    new ReportParameter("TownName", "All Towns"),
//                    new ReportParameter("SectorName", "All Sectors")
//                };
//                reportViewer1.LocalReport.SetParameters(parameters);

//                // Set window title
//                this.Text = $"Salesman Summary - {fromDate:dd/MM/yyyy} to {toDate:dd/MM/yyyy}";
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error setting report data: " + ex.Message,
//                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void Salesman_Summary_win__Load(object sender, EventArgs e)
//        {
//            // Setup report viewer
//            reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
//            reportViewer1.ZoomMode = ZoomMode.Percent;
//            reportViewer1.ZoomPercent = 100;

//            // Refresh report
//            this.reportViewer1.RefreshReport();
//        }

//        private void reportViewer1_Load(object sender, EventArgs e)
//        {
//            // Optional: Additional setup
//        }
//    }
//}
using System;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace FO_2022
{
    public partial class Salesman_Summary_win_ : Form
    {
        public Salesman_Summary_win_()
        {
            InitializeComponent();
        }

        public void SetReportData(DataTable dt, DateTime fromDate, DateTime toDate, string salesmanName)
        {
            try
            {
                // Clear existing data sources
                reportViewer1.LocalReport.DataSources.Clear();

                // METHOD 1: Try to find the resource automatically
                string reportResource = FindReportResource();

                if (!string.IsNullOrEmpty(reportResource))
                {
                    // Use embedded resource
                    reportViewer1.LocalReport.ReportEmbeddedResource = reportResource;
                }
                else
                {
                    // METHOD 2: Try file path as fallback
                    string reportPath = FindReportFilePath();
                    if (!string.IsNullOrEmpty(reportPath))
                    {
                        reportViewer1.LocalReport.ReportPath = reportPath;
                    }
                    else
                    {
                        MessageBox.Show("Record file not found");
                        return;
                    }
                }

                // Add data source
                ReportDataSource rds = new ReportDataSource("DataSet1", dt);
                reportViewer1.LocalReport.DataSources.Add(rds);

                // Add parameters
                ReportParameter[] parameters =
                {
                    //new ReportParameter("FromDate", fromDate.ToString("dd/MM/yyyy")),
                    //new ReportParameter("ToDate", toDate.ToString("dd/MM/yyyy")),
                    //new ReportParameter("SalesmanName", salesmanName),
                    //new ReportParameter("ReportDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm"))
                };

                reportViewer1.LocalReport.SetParameters(parameters);

                // Refresh report
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Report error: " + ex.Message);
            }
        }

        private string FindReportResource()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resources = assembly.GetManifestResourceNames();

                // Look for your report with different possible names
                string[] possibleNames =
                {
                    "Salesman_Summary[report].rdlc",
                    "Salesman_Summary_report.rdlc",
                    "SalesmanSummaryReport.rdlc",
                    "Salesman_Summary.rdlc",
                    "SalesmanSummary.rdlc"
                };

                foreach (var resource in resources)
                {
                    foreach (var name in possibleNames)
                    {
                        if (resource.EndsWith(name, StringComparison.OrdinalIgnoreCase))
                        {
                            return resource;
                        }
                    }
                }

                // If not found, show available resources for debugging
                string allResources = "Available embedded resources:\n";
                foreach (var resource in resources)
                {
                    allResources += resource + "\n";
                }

                //MessageBox.Show(allResources, "Debug Info");
                return null;
            }
            catch
            {
                return null;
            }
        }
        private string FindReportFilePath()
        {
            // Try different possible locations
            string[] possiblePaths =
            {
                @"Salesman_Summary[report].rdlc",
                @"Reports\Salesman_Summary[report].rdlc",
                @"..\..\Salesman_Summary[report].rdlc",
                @"..\..\Reports\Salesman_Summary[report].rdlc",
                Application.StartupPath + @"\Salesman_Summary[report].rdlc",
                Application.StartupPath + @"\Reports\Salesman_Summary[report].rdlc"
            };

            foreach (var path in possiblePaths)
            {
                if (System.IO.File.Exists(path))
                {
                    return path;
                }
            }

            return null;
        }

        //private void Salesman_Summary_win__Load(object sender, EventArgs e)
        //{
        //    // Initialize the report viewer
        //    if (!reportViewer1.LocalReport.IsReadyForRendering)
        //    {
        //        reportViewer1.RefreshReport();
        //    }
        //}
        private void Salesman_Summary_win__Load(object sender, EventArgs e)
        {
            try
            {
                this.reportViewer1.RefreshReport();
                this.reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
                //this.reportViewer1.ZoomMode = ZoomMode.PageWidth;
                //this.reportViewer1.ZoomPercent = 100;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Report viewer load error: " + ex.Message);
            }
        }

        private void reportViewer1_Load(object sender, EventArgs e) { }
    }
}