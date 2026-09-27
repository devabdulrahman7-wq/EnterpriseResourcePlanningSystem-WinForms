using Microsoft.Reporting.WinForms;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022.Reports
{
    public class LedgerReportService
    {
        private readonly string _connectionString;

        public LedgerReportService()
            : this(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString)
        {
        }

        public LedgerReportService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable GetPartyLedgerDetailedReportTable(string accountNo, DateTime fromDate, DateTime toDate)
        {
            decimal openingBalance;
            DataTable rawTransactions = LoadPartyLedgerTransactions(accountNo, fromDate, toDate, out openingBalance);
            return LedgerReportDataTableBuilder.BuildPartyLedgerDetailed(rawTransactions, openingBalance, fromDate);
        }

        public void ShowPartyLedgerDetailedReport(
            IWin32Window owner,
            string accountNo,
            string accountName,
            DateTime fromDate,
            DateTime toDate,
            string printedBy)
        {
            DataTable reportTable = GetPartyLedgerDetailedReportTable(accountNo, fromDate, toDate);

            using (FrmReportViewer viewer = new FrmReportViewer())
            {
                viewer.LoadReport(
                    "FO_2022.Reports.PartyLedgerDetailed.rdlc",
                    new[]
                    {
                        new ReportDataSource(LedgerReportDataTableBuilder.DataSetName, reportTable)
                    },
                    CreateStandardParameters("PARTY LEDGER DETAILED", fromDate, toDate, accountName, printedBy));

                viewer.ShowDialog(owner);
            }
        }

        public ReportParameter[] CreateStandardParameters(
            string reportTitle,
            DateTime fromDate,
            DateTime toDate,
            string accountName,
            string printedBy)
        {
            return new[]
            {
                new ReportParameter("ReportTitle", reportTitle),
                new ReportParameter("FromDate", fromDate.ToString("dd-MMM-yyyy")),
                new ReportParameter("ToDate", toDate.ToString("dd-MMM-yyyy")),
                new ReportParameter("AccountName", accountName ?? string.Empty),
                new ReportParameter("PrintedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt")),
                new ReportParameter("PrintedBy", printedBy ?? string.Empty)
            };
        }

        private DataTable LoadPartyLedgerTransactions(
            string accountNo,
            DateTime fromDate,
            DateTime toDate,
            out decimal openingBalance)
        {
            DataTable table = new DataTable("RawPartyLedger");
            openingBalance = 0m;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand("dbo.usp_Report_PartyLedgerDetailed", connection))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 120;
                command.Parameters.AddWithValue("@AccountNo", accountNo);
                command.Parameters.AddWithValue("@FromDate", fromDate.Date);
                command.Parameters.AddWithValue("@ToDate", toDate.Date);

                SqlParameter openingBalanceParameter = new SqlParameter("@OpeningBalance", SqlDbType.Decimal)
                {
                    Precision = 18,
                    Scale = 2,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(openingBalanceParameter);

                connection.Open();
                adapter.Fill(table);

                if (openingBalanceParameter.Value != null && openingBalanceParameter.Value != DBNull.Value)
                {
                    openingBalance = Convert.ToDecimal(openingBalanceParameter.Value);
                }
            }

            return table;
        }
    }
}
