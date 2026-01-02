//using Microsoft.Reporting.WinForms;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace FO_2022.Reports
//{
//    public partial class Purchase_Summary_rv : Form
//    {
//        private DataTable _reportData;
//        private DateTime _fromDate;
//        private DateTime _toDate;
//        private string _vendorName;

//        public Purchase_Summary_rv(DataTable reportData, DateTime fromDate, DateTime toDate, string vendorName = "")
//        {
//            InitializeComponent();
//            _reportData = reportData;
//            _fromDate = fromDate;
//            _toDate = toDate;
//            _vendorName = vendorName;
//        }

//        //private void Purchase_Summary_rv_Load(object sender, EventArgs e)
//        //{
//        //    try
//        //    {
//        //        // Set processing mode
//        //        reportViewer1.ProcessingMode = ProcessingMode.Local;

//        //        // Find and set report path
//        //        string reportPath = GetReportPath();

//        //        if (!File.Exists(reportPath))
//        //        {
//        //            MessageBox.Show($"Report file not found: {reportPath}",
//        //                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//        //            return;
//        //        }

//        //        // Set the report path
//        //        reportViewer1.LocalReport.ReportPath = reportPath;

//        //        // Set up report data source
//        //        ReportDataSource rds = new ReportDataSource("PurchaseSummaryDS", _reportData);
//        //        reportViewer1.LocalReport.DataSources.Clear();
//        //        reportViewer1.LocalReport.DataSources.Add(rds);

//        //        // **COMMENTED OUT - Add report parameters later when RDLC is ready**
//        //        // SetReportParameters();

//        //        // Refresh report
//        //        reportViewer1.RefreshReport();
//        //        reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
//        //        reportViewer1.ZoomMode = ZoomMode.Percent;
//        //        reportViewer1.ZoomPercent = 100;
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        MessageBox.Show("Error loading report: " + ex.Message,
//        //            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//        //        this.Close();
//        //    }
//        //}
//        private void Purchase_Summary_rv_Load(object sender, EventArgs e)
//        {
//            try
//            {
//                // Set processing mode
//                reportViewer1.ProcessingMode = ProcessingMode.Local;

//                // Find and set report path
//                string reportPath = GetReportPath();

//                if (!File.Exists(reportPath))
//                {
//                    MessageBox.Show($"Report file not found: {reportPath}",
//                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                    return;
//                }

//                // Set the report path
//                reportViewer1.LocalReport.ReportPath = reportPath;

//                // Set up report data source (using DataSet name from your RDLC)
//                ReportDataSource rds = new ReportDataSource("DataSet1", _reportData);
//                reportViewer1.LocalReport.DataSources.Clear();
//                reportViewer1.LocalReport.DataSources.Add(rds);

//                // **NO PARAMETERS - Report will work without them**

//                // Refresh report
//                reportViewer1.RefreshReport();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading report: " + ex.Message +
//                    "\n\nMake sure:\n1. RDLC file exists\n2. DataSet name matches in RDLC\n3. Report path is correct",
//                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                this.Close();
//            }
//        }
//        private string GetReportPath()
//        {
//            // Try different possible locations for the RDLC file
//            string[] possiblePaths =
//            {
//                Path.Combine(Application.StartupPath, "Reports", "Purchase_Summary.rdlc"),
//                Path.Combine(Application.StartupPath, "..", "..", "Reports", "Purchase_Summary.rdlc"),
//                Path.Combine(Application.StartupPath, "..", "..", "..", "Reports", "Purchase_Summary.rdlc"),
//                Path.Combine(Application.StartupPath, "Purchase_Summary.rdlc"),
//                Path.Combine(Application.StartupPath, "..", "..", "Purchase_Summary.rdlc"),
//            };

//            foreach (string path in possiblePaths)
//            {
//                if (File.Exists(path))
//                {
//                    return path;
//                }
//            }

//            // If not found, use the first path
//            return possiblePaths[0];
//        }

//        // **COMMENTED OUT - Will enable when RDLC parameters are ready**
//        /*
//        private void SetReportParameters()
//        {
//            try
//            {
//                // Add report parameters
//                ReportParameter[] parameters = new ReportParameter[3];
//                parameters[0] = new ReportParameter("FromDate", _fromDate.ToString("dd/MM/yyyy"));
//                parameters[1] = new ReportParameter("ToDate", _toDate.ToString("dd/MM/yyyy"));
//                parameters[2] = new ReportParameter("VendorName",
//                    string.IsNullOrWhiteSpace(_vendorName) ? "All Vendors" : _vendorName);

//                reportViewer1.LocalReport.SetParameters(parameters);

//                // Calculate totals
//                decimal totalGross = 0, totalDiscount = 0, totalSalesTax = 0, totalNet = 0;
//                foreach (DataRow row in _reportData.Rows)
//                {
//                    totalGross += row["GrossValue"] != DBNull.Value ? Convert.ToDecimal(row["GrossValue"]) : 0;
//                    totalDiscount += row["Discount"] != DBNull.Value ? Convert.ToDecimal(row["Discount"]) : 0;
//                    totalSalesTax += row["SalesTax"] != DBNull.Value ? Convert.ToDecimal(row["SalesTax"]) : 0;
//                    totalNet += row["NetValue"] != DBNull.Value ? Convert.ToDecimal(row["NetValue"]) : 0;
//                }

//                // Add total parameters
//                ReportParameter[] totalParams = new ReportParameter[4];
//                totalParams[0] = new ReportParameter("TotalGross", totalGross.ToString("N2"));
//                totalParams[1] = new ReportParameter("TotalDiscount", totalDiscount.ToString("N2"));
//                totalParams[2] = new ReportParameter("TotalSalesTax", totalSalesTax.ToString("N2"));
//                totalParams[3] = new ReportParameter("TotalNet", totalNet.ToString("N2"));

//                reportViewer1.LocalReport.SetParameters(totalParams);
//            }
//            catch (Exception ex)
//            {
//                // Log parameter error but don't crash the report
//                Console.WriteLine("Parameter error (non-critical): " + ex.Message);
//            }
//        }
//        */

//        private void reportViewer1_Load(object sender, EventArgs e)
//        {
//            // This can be left empty
//        }
//    }
//}
using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace FO_2022.Reports
{
    public partial class Purchase_Summary_rv : Form
    {
        private DataTable _reportData;
        
        public Purchase_Summary_rv(DataTable reportData)
        {
            InitializeComponent();
            _reportData = reportData;
        }
        
        private void Purchase_Summary_rv_Load(object sender, EventArgs e)
        {
            try
            {
                // Check if we have data
                if (_reportData == null || _reportData.Rows.Count == 0)
                {
                    MessageBox.Show("No data to display in report!", "Empty Report",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                    return;
                }

                // Set processing mode
                reportViewer1.ProcessingMode = ProcessingMode.Local;
                
                // Find RDLC file - check multiple locations
                string reportPath = FindReportFile();
                
                if (string.IsNullOrEmpty(reportPath) || !File.Exists(reportPath))
                {
                    MessageBox.Show("Report file (Purchase_Summary.rdlc) not found!\n" +
                        "Please make sure the RDLC file is in the correct folder.", 
                        "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }
                
                // Set report path
                reportViewer1.LocalReport.ReportPath = reportPath;
                
                // Clear existing data sources
                reportViewer1.LocalReport.DataSources.Clear();
                
                // Try to add data source with common dataset names
                bool dataSourceAdded = AddDataSource();
                
                if (!dataSourceAdded)
                {
                    MessageBox.Show("Could not bind data to report. Check dataset name in RDLC file.", 
                        "Data Binding Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
                // Refresh and show report
                reportViewer1.RefreshReport();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading report:\n" + ex.Message, 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private string FindReportFile()
        {
            string[] possiblePaths = 
            {
                // Check in current directory
                Path.Combine(Application.StartupPath, "Purchase_Summary.rdlc"),
                // Check in Reports folder
                Path.Combine(Application.StartupPath, "Reports", "Purchase_Summary.rdlc"),
                // Check in parent directories
                Path.Combine(Application.StartupPath, "..", "..", "Purchase_Summary.rdlc"),
                Path.Combine(Application.StartupPath, "..", "..", "Reports", "Purchase_Summary.rdlc"),
                Path.Combine(Application.StartupPath, "..", "..", "..", "Purchase_Summary.rdlc"),
            };
            
            foreach (string path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }
            
            return null;
        }

        private bool AddDataSource()
        {
            string[] datasetNames = 
            { 
                //"PurchaseSummaryDS",  // Most likely
                "DataSet1",           // Common default
                //"DS_Purchase",        // Alternative
                //"DataSet",            // Generic
                //"PurchaseDS"          // Another alternative
            };
            
            foreach (string datasetName in datasetNames)
            {
                try
                {
                    ReportDataSource rds = new ReportDataSource(datasetName, _reportData);
                    reportViewer1.LocalReport.DataSources.Add(rds);
                    return true;
                }
                catch
                {
                    // Try next name
                    reportViewer1.LocalReport.DataSources.Clear();
                }
            }
            
            return false;
        }
    }
}