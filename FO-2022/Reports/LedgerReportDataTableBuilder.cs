using System;
using System.Data;
using System.Globalization;
using System.Linq;

namespace FO_2022.Reports
{
    public static class LedgerReportDataTableBuilder
    {
        public const string DataSetName = "LedgerDataSet";

        public static DataTable CreateSchema()
        {
            DataTable table = new DataTable(DataSetName);

            table.Columns.Add("RowType", typeof(string));
            table.Columns.Add("LineNo", typeof(int));
            table.Columns.Add("VoucherDate", typeof(DateTime));
            table.Columns.Add("DisplayDate", typeof(string));
            table.Columns.Add("VoucherNo", typeof(string));
            table.Columns.Add("Reference", typeof(string));
            table.Columns.Add("DocProof", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("DebitAmount", typeof(decimal));
            table.Columns.Add("CreditAmount", typeof(decimal));
            table.Columns.Add("BalanceAmount", typeof(decimal));
            table.Columns.Add("BalanceNature", typeof(string));
            table.Columns.Add("BalanceText", typeof(string));

            return table;
        }

        public static DataTable BuildPartyLedgerDetailed(
            DataTable rawTransactions,
            decimal openingBalance,
            DateTime fromDate)
        {
            DataTable table = CreateSchema();
            decimal runningBalance = openingBalance;
            decimal totalDebit = 0m;
            decimal totalCredit = 0m;
            int lineNo = 1;

            AddSpecialRow(table, "OPENING", 0, fromDate, string.Empty, string.Empty, string.Empty,
                "Opening Balance", 0m, 0m, runningBalance);

            if (rawTransactions != null)
            {
                var orderedRows = rawTransactions.AsEnumerable()
                    .OrderBy(row => GetDate(row, "VoucherDate"))
                    .ThenBy(row => GetString(row, "VoucherNo"))
                    .ThenBy(row => GetString(row, "Reference"))
                    .ThenBy(row => GetString(row, "DocProof"));

                foreach (DataRow sourceRow in orderedRows)
                {
                    decimal debit = GetDecimal(sourceRow, "Debit");
                    decimal credit = GetDecimal(sourceRow, "Credit");

                    totalDebit += debit;
                    totalCredit += credit;
                    runningBalance += debit - credit;

                    DataRow row = table.NewRow();
                    row["RowType"] = "DETAIL";
                    row["LineNo"] = lineNo++;
                    row["VoucherDate"] = GetDate(sourceRow, "VoucherDate");
                    row["DisplayDate"] = GetDate(sourceRow, "VoucherDate") == DateTime.MinValue
                        ? string.Empty
                        : GetDate(sourceRow, "VoucherDate").ToString("dd-MMM-yyyy");
                    row["VoucherNo"] = GetString(sourceRow, "VoucherNo");
                    row["Reference"] = GetString(sourceRow, "Reference");
                    row["DocProof"] = GetString(sourceRow, "DocProof");
                    row["Description"] = NormalizeDescription(
                        FirstNonEmpty(
                            GetString(sourceRow, "Description"),
                            GetString(sourceRow, "Narration"),
                            GetString(sourceRow, "Remarks")));
                    row["DebitAmount"] = debit;
                    row["CreditAmount"] = credit;
                    row["BalanceAmount"] = Math.Abs(runningBalance);
                    row["BalanceNature"] = runningBalance >= 0 ? "Dr" : "Cr";
                    row["BalanceText"] = ToLedgerBalanceText(runningBalance);
                    table.Rows.Add(row);
                }
            }

            AddSpecialRow(table, "TOTAL", 999998, fromDate, string.Empty, string.Empty, string.Empty,
                "Transaction Totals", totalDebit, totalCredit, runningBalance);
            AddSpecialRow(table, "CLOSING", 999999, fromDate, string.Empty, string.Empty, string.Empty,
                "Closing Balance", 0m, 0m, runningBalance);

            return table;
        }

        private static void AddSpecialRow(
            DataTable table,
            string rowType,
            int lineNo,
            DateTime voucherDate,
            string voucherNo,
            string reference,
            string docProof,
            string description,
            decimal debit,
            decimal credit,
            decimal runningBalance)
        {
            DataRow row = table.NewRow();
            row["RowType"] = rowType;
            row["LineNo"] = lineNo;
            row["VoucherDate"] = voucherDate == DateTime.MinValue ? DateTime.Today : voucherDate;
            row["DisplayDate"] = string.Empty;
            row["VoucherNo"] = voucherNo;
            row["Reference"] = reference;
            row["DocProof"] = docProof;
            row["Description"] = description;
            row["DebitAmount"] = debit;
            row["CreditAmount"] = credit;
            row["BalanceAmount"] = Math.Abs(runningBalance);
            row["BalanceNature"] = runningBalance >= 0 ? "Dr" : "Cr";
            row["BalanceText"] = ToLedgerBalanceText(runningBalance);
            table.Rows.Add(row);
        }

        private static string ToLedgerBalanceText(decimal amount)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0:#,##0.00} {1}",
                Math.Abs(amount),
                amount >= 0 ? "Dr" : "Cr");
        }

        private static string NormalizeDescription(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Replace("\r\n", "\n").Replace("\r", "\n");
        }

        private static string FirstNonEmpty(params string[] values)
        {
            return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;
        }

        private static string GetString(DataRow row, string columnName)
        {
            return row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value
                ? Convert.ToString(row[columnName]).Trim()
                : string.Empty;
        }

        private static decimal GetDecimal(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
            {
                return 0m;
            }

            decimal value;
            return decimal.TryParse(Convert.ToString(row[columnName]), out value) ? value : 0m;
        }

        private static DateTime GetDate(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
            {
                return DateTime.MinValue;
            }

            DateTime value;
            return DateTime.TryParse(Convert.ToString(row[columnName]), out value) ? value : DateTime.MinValue;
        }
    }
}
