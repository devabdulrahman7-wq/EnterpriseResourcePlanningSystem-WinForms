using Microsoft.Reporting.WinForms;
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
    public partial class Daybook_report : Form
    {
        string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        //public Daybook_report()
        //{
        //    InitializeComponent();
        //}
        DateTime _fromDate;
        DateTime _toDate;

        public Daybook_report(DateTime fromDate, DateTime toDate)
        {
            InitializeComponent();
            _fromDate = fromDate;
            _toDate = toDate;
        }

        private void Daybook_report_Load(object sender, EventArgs e)
        {
            LoadReport();
            this.reportViewer1.RefreshReport();
        }

        private void LoadReport()
        {
            string query = @"SELECT VoucherDate, VoucherNo, VoucherType, AccountNo, Debit, Credit, Narration 
                             FROM AllVouchers 
                             WHERE VoucherDate BETWEEN @FromDate AND @ToDate
                             ORDER BY VoucherDate, VoucherNo";

            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = _fromDate;
                cmd.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = _toDate;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            reportViewer1.LocalReport.DataSources.Clear();

            ReportDataSource rds = new ReportDataSource("DayBookDataSet", dt);
            reportViewer1.LocalReport.DataSources.Add(rds);

            // Use one of these
            //reportViewer1.LocalReport.ReportPath = @"Reports\DayBook.rdlc";
             reportViewer1.LocalReport.ReportEmbeddedResource = "FO_2022.Reports.DayBook.rdlc";

            ReportParameter[] parameters =
            {
                new ReportParameter("FromDate", _fromDate.ToString("dd-MM-yyyy")),
                new ReportParameter("ToDate", _toDate.ToString("dd-MM-yyyy"))
            };

            reportViewer1.LocalReport.SetParameters(parameters);
            reportViewer1.RefreshReport();
        }

      
    }
}