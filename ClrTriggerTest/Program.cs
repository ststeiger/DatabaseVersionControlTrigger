
namespace ClrTriggerTest
{


    public static class Program
    {


        public static void TestQuery()
        {
            System.Data.SqlClient.SqlConnectionStringBuilder csb = new System.Data.SqlClient.SqlConnectionStringBuilder();
            csb.DataSource = System.Environment.MachineName;
            csb.InitialCatalog = "COR_Basic_Demo_V4";
            csb.IntegratedSecurity = true;

            string connString = csb.ConnectionString;
        } // End Function TestQuery 


        public static void TestRequest()
        {
            // System.Data.SqlTypes.SqlString ret = 
            System.Data.SqlTypes.SqlXml ret =
            DatabaseVersionControl.DatabaseVersionControlTrigger.HttpRequest(
                "POST",
                "https://localhost:44302/reportSchemaChange",
                "application/json",
                "{ \"hello\": \"kitty\" }",
                -1,
                // "application/xml"
                // "application/json"
                // "text/plain"
                // "text/csv"
                "text/html"
            );

            string json = ret.Value;
            System.Console.WriteLine(json);
        } // End Sub TestRequest 


        public static System.Data.SqlTypes.SqlXml CreateSqlXmlFromString(string xmlString)
        {
            try
            {
                // Create an XmlReader from the XML string
                using (System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(new System.IO.StringReader(xmlString)))
                {
                    // Create an SqlXml from the XmlReader
                    System.Data.SqlTypes.SqlXml sqlXml = new System.Data.SqlTypes.SqlXml(xmlReader);
                    return sqlXml;
                }
            }
            catch (System.Exception ex)
            {
                // Handle any exceptions here, e.g., log the error
                throw ex;
            }
        }
    

        public static string TestGetXmlTag()
        {
            string plain = @"<?xml version=""1.0"" encoding=""UTF-16""?>
<table xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" routine_schema=""NULL"" routine_name=""UNKOWN"">
  <row>
    <U>MyU</U>
    <U2>My2</U2>
    <P>MyP</P>
    <P2>MyP2</P2>
    <R>MyR</R>
    <LN>MyLN</LN>
    <FN>MyFN</FN>
    <E>MyE</E>
    <Provider>shibb</Provider>
    <Rabbit xsi:nil=""true"" />
    <Empty1></Empty1>
    <Empty2 />
  </row>
</table>";


            System.Data.SqlTypes.SqlXml xml = CreateSqlXmlFromString(plain);

            System.Data.SqlTypes.SqlString x = DatabaseVersionControl.XmlFunctionRepository.GetXmlTag(xml, "U");
            // System.Data.SqlTypes.SqlString x = DatabaseVersionControl.XmlFunctionRepository.GetXmlTag(xml, "Rabbit");
            // x = DatabaseVersionControl.XmlFunctionRepository.GetXmlTag(xml, "E");
            // x = DatabaseVersionControl.XmlFunctionRepository.GetXmlTag(xml, "F");
            // x = DatabaseVersionControl.XmlFunctionRepository.GetXmlTag(xml, "Empty1");
            // x = DatabaseVersionControl.XmlFunctionRepository.GetXmlTag(xml, "Empty2");
            if (x.IsNull)
                return null;
            else 
                System.Console.WriteLine(x.Value);

            return x.Value;
        }


        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [System.STAThread]
        public static int Main(string[] args)
        {
            TestRequest();
            
            TestQuery();
            
            TestGetXmlTag();


            System.Console.WriteLine("Finished");
            System.Console.ReadKey();

            return 0;
        } // End Sub Main 


    } // End Class Program 


} // End Namespace 
