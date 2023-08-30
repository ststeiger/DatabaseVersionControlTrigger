
namespace DatabaseVersionControl
{


    public class XmlFunctionRepository 
    {


        // CREATE FUNCTION dbo.GetXmlTag(@xmlInput xml, @tagname nvarchar(255)) 
        // RETURNS nvarchar(MAX) 
        // AS EXTERNAL NAME DatabaseVersionControl.[DatabaseVersionControl.XmlFunctionRepository].GetXmlTag 
        [Microsoft.SqlServer.Server.SqlFunction(IsDeterministic = true)]
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

                    if (xsiNilAttribute != null && string.Equals(xsiNilAttribute.Value, "true", System.StringComparison.InvariantCultureIgnoreCase))
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


    }
}
