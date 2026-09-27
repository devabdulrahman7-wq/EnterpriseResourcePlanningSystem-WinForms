using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace FO_2022
{
    public static class SearchService
    {
        private static string connStr =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public static DataTable Search(string tableName, string columnName, string text)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = $@"
                        SELECT TOP 50000 {columnName}
                        FROM {tableName}
                        WHERE {columnName} LIKE @text + '%'";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@text", text);
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Search Error: " + ex.Message);
            }

            return dt;
        }
    }
}