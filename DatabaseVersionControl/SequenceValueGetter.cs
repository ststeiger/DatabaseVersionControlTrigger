
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

            return new System.Data.SqlTypes.SqlInt64((long)result);
        } // End Function GetNextSequenceValue 


    } // End Class SequenceValueGetter 


} // End Namespace DatabaseVersionControl 
