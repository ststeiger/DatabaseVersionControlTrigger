
namespace DatabaseVersionControl
{


    public class SequenceValueGetter
    {


        // CREATE FUNCTION dbo.GetXmlTag(@xmlInput xml, @tagname nvarchar(255)) 
        // RETURNS nvarchar(MAX) 
        // AS EXTERNAL NAME DatabaseVersionControl.[DatabaseVersionControl.SequenceValueGetter].GetXmlTag 
        [Microsoft.SqlServer.Server.SqlFunction]
        public static System.Data.SqlTypes.SqlString GetXmlTag(
            System.Data.SqlTypes.SqlXml xmlInput,
            System.Data.SqlTypes.SqlString tagName
        )
        {
            if (xmlInput.IsNull)
                return System.Data.SqlTypes.SqlString.Null;

            // None of this is allowed: 
            // CREATE FUNCTION dbo.GetXmlTag(@xmlInput xml, @tagName nvarchar(256)) 
            //     RETURNS nvarchar(MAX) 
            // BEGIN 
            //     -- RETURN @xmlInput.value('(//U)[1]', 'nvarchar(MAX)'); 
            //     -- RETURN @xmlInput.value('(//{sql:variable("@foo")})[1]', 'nvarchar(MAX)'); 
            //     RETURN @xmlInput.value('(//' + @tagName + ')[1]', 'nvarchar(MAX)'); 
            // END 

            try
            {
                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                xmlDoc.XmlResolver = null;
                xmlDoc.LoadXml(xmlInput.Value);

                // System.Xml.XmlNode node = xmlDoc.SelectSingleNode("//U");
                System.Xml.XmlNode node = xmlDoc.SelectSingleNode($"//{tagName.Value}");

                if (node != null)
                {
                    System.Xml.XmlAttribute xsiNilAttribute = node.Attributes["xsi:nil"];

                    if (xsiNilAttribute != null && string.Equals( xsiNilAttribute.Value , "true", System.StringComparison.InvariantCultureIgnoreCase))
                        return System.Data.SqlTypes.SqlString.Null;

                    return new System.Data.SqlTypes.SqlString(node.InnerText);
                }
                else
                    return System.Data.SqlTypes.SqlString.Null;
            }
            catch (System.Exception ex)
            {
                // Handle any exceptions here
                // System.Console.WriteLine(ex.Message);
                // throw new System.Exception("Your custom error message.", ex);
                // throw;
                return new System.Data.SqlTypes.SqlString("Error:  " + ex.Message + "\r\n\r\n" + ex.StackTrace);
            }

            return System.Data.SqlTypes.SqlString.Null;
        } // End Function GetXmlTag 


        private static string QuoteObject(string objectName)
        {
            if (string.IsNullOrEmpty(objectName))
                return objectName;

            return "\"" + objectName.Replace("\"", "\"\"") + "\"";
        } // End Function QuoteObject 


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
