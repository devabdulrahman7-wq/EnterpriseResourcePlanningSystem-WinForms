using System;
using System.Data;
using System.Globalization;

namespace FO_2022.Reports
{
    public static class PartyProductSalesLedgerProfessionalBuilder
    {
        public const string DataSetName = "DataSet1";

        public static DataTable CreateSchema()
        {
            DataTable table = new DataTable(DataSetName);

            table.Columns.Add("RowType", typeof(string));
            table.Columns.Add("SaleDate", typeof(DateTime));
            table.Columns.Add("DisplaySaleDate", typeof(string));
            table.Columns.Add("CustomerName", typeof(string));
            table.Columns.Add("ProductDetail", typeof(string));
            table.Columns.Add("TotalQty", typeof(decimal));
            table.Columns.Add("TotalValue", typeof(decimal));
            table.Columns.Add("TotalQtyText", typeof(string));
            table.Columns.Add("TotalValueText", typeof(string));

            return table;
        }

        public static DataTable Build(DataTable rawData)
        {
            DataTable table = CreateSchema();

            if (rawData != null)
            {
                foreach (DataRow sourceRow in rawData.Rows)
                {
                    decimal rowQty = GetDecimal(sourceRow, "TotalQty");
                    decimal rowValue = GetDecimal(sourceRow, "NetAmount");

                    DataRow row = table.NewRow();
                    row["RowType"] = "DETAIL";
                    row["SaleDate"] = GetDate(sourceRow, "SaleDate");
                    row["DisplaySaleDate"] = GetDate(sourceRow, "SaleDate") == DateTime.MinValue
                        ? string.Empty
                        : GetDate(sourceRow, "SaleDate").ToString("dd-MMM-yyyy");
                    row["CustomerName"] = GetString(sourceRow, "CustomerName");
                    row["ProductDetail"] = BuildProductDetail(sourceRow);
                    row["TotalQty"] = rowQty;
                    row["TotalValue"] = rowValue;
                    row["TotalQtyText"] = rowQty == 0m ? string.Empty : rowQty.ToString("#,##0.##", CultureInfo.InvariantCulture);
                    row["TotalValueText"] = rowValue == 0m ? string.Empty : rowValue.ToString("#,##0.00", CultureInfo.InvariantCulture);

                    table.Rows.Add(row);
                }
            }

            return table;
        }

        private static string BuildProductDetail(DataRow row)
        {
            string productName = GetString(row, "ProductName");
            string packingName = GetString(row, "PackingName");
            string multiplier = GetDecimal(row, "Multiplier") == 0m
                ? string.Empty
                : GetDecimal(row, "Multiplier").ToString("#,##0.##", CultureInfo.InvariantCulture);
            string qtyPack = GetDecimal(row, "QtyPack") == 0m
                ? string.Empty
                : GetDecimal(row, "QtyPack").ToString("#,##0.##", CultureInfo.InvariantCulture);

            string detail = productName;

            if (!string.IsNullOrWhiteSpace(packingName))
            {
                detail += " " + packingName;
            }

            if (!string.IsNullOrWhiteSpace(multiplier))
            {
                detail += " " + multiplier + " Pcs";
            }

            if (!string.IsNullOrWhiteSpace(qtyPack))
            {
                detail += " " + qtyPack + " Pack";
            }

            return detail.Trim();
        }

        private static string GetString(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
            {
                return string.Empty;
            }

            return Convert.ToString(row[columnName]).Trim();
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
