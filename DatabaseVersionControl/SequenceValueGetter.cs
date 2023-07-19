
namespace DatabaseVersionControl
{


    public class SequenceValueGetter
    {


        private static string QuoteObject(string objectName)
        {
            if (string.IsNullOrEmpty(objectName))
                return objectName;

            return "\"" + objectName.Replace("\"", "\"\"") + "\"";
        }


        [Microsoft.SqlServer.Server.SqlFunction(DataAccess = Microsoft.SqlServer.Server.DataAccessKind.Read)]
        public static System.Data.SqlTypes.SqlInt64 GetNextSequenceValue(
            System.Data.SqlTypes.SqlString name
        )
        {
            object result = null;

            using (System.Data.Common.DbConnection connection =
                 new System.Data.SqlClient.SqlConnection("context connection=true"))
            {
                bool haveToOpen = connection.State != System.Data.ConnectionState.Open;
                if (haveToOpen)
                    connection.Open();

                using (System.Data.Common.DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT NEXT VALUE FOR " + QuoteObject(name.Value) + " AS seq ";
                    result = cmd.ExecuteScalar();
                } // End Using cmd 

                if (haveToOpen && connection.State != System.Data.ConnectionState.Closed)
                    connection.Close();
            } // End Using connection 

            if (result == null || result == System.DBNull.Value)
                return new System.Data.SqlTypes.SqlInt64(0);

            return new System.Data.SqlTypes.SqlInt64((long)result);
        } // End Function GetNextSequenceValue 


        [Microsoft.SqlServer.Server.SqlFunction(DataAccess = Microsoft.SqlServer.Server.DataAccessKind.Read)]
        public static System.Data.SqlTypes.SqlInt64 GetHighestValue(
            System.Data.SqlTypes.SqlString schema_name,
            System.Data.SqlTypes.SqlString table_name,
            System.Data.SqlTypes.SqlString column_name 
        )
        {
            object result = null;

            using (System.Data.Common.DbConnection connection =
                 new System.Data.SqlClient.SqlConnection("context connection=true"))
            {
                bool haveToOpen = connection.State != System.Data.ConnectionState.Open;
                if (haveToOpen)
                    connection.Open();

                using (System.Data.Common.DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT MAX("+ QuoteObject(column_name.Value) + ") AS val FROM " + QuoteObject(schema_name.Value) + "." + QuoteObject(table_name.Value) + "; ";
                    result = cmd.ExecuteScalar();
                } // End Using cmd 

                if (haveToOpen && connection.State != System.Data.ConnectionState.Closed)
                    connection.Close();
            } // End Using connection 

            // if (result == null || result == System.DBNull.Value)
                // return new System.Data.SqlTypes.SqlInt64(0);

            return new System.Data.SqlTypes.SqlInt64((long)result);
        } // End Function GetHighestValue 


    } // End Class SequenceValueGetter 


} // End Namespace DatabaseVersionControl 
