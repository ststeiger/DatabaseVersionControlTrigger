
namespace DatabaseVersionControl
{


    public class SequenceValueGetter
    {


        private static string QuoteObject(string objectName)
        {
            if (string.IsNullOrEmpty(objectName))
                return objectName;

            return "\"" + objectName.Replace("\"", "\"\"") + "\"";
        } // End Function QuoteObject 


        [Microsoft.SqlServer.Server.SqlFunction(DataAccess = Microsoft.SqlServer.Server.DataAccessKind.Read)]
        public static System.Data.SqlTypes.SqlInt64 GetNextSequenceValue(
             System.Data.SqlTypes.SqlString sequence_schema
            ,System.Data.SqlTypes.SqlString sequence_name
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
                    cmd.CommandText = "SELECT NEXT VALUE FOR ";
                    string sn = sequence_schema.Value;

                    if (sn != null)
                        cmd.CommandText += QuoteObject(sn) + ".";
                    cmd.CommandText +=  QuoteObject(sequence_name.Value) + " AS seq ";
                    result = cmd.ExecuteScalar();
                } // End Using cmd 

                if (haveToOpen && connection.State != System.Data.ConnectionState.Closed)
                    connection.Close();
            } // End Using connection 

            if (result == null || result == System.DBNull.Value)
                return new System.Data.SqlTypes.SqlInt64(0L);

            return new System.Data.SqlTypes.SqlInt64((long)System.Convert.ChangeType(result, typeof(long)));
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

            if (result == null || result == System.DBNull.Value)
                return new System.Data.SqlTypes.SqlInt64(0L);

            return new System.Data.SqlTypes.SqlInt64((long)System.Convert.ChangeType(result, typeof(long)));
        } // End Function GetHighestValue 


    } // End Class SequenceValueGetter 


} // End Namespace DatabaseVersionControl 
