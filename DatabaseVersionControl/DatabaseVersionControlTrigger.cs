
namespace DatabaseVersionControl
{


    // CLR triggers are supported in Microsoft SQL Server 2005 and later versions.
    public class DatabaseVersionControlTrigger
    {


        [Microsoft.SqlServer.Server.SqlTrigger(
            Name = "DatabaseVersionControl",
            Target = "DATABASE",
            Event = "DDL_DATABASE_LEVEL_EVENTS")]
        public static void OnDatabaseChange()
        {
            const string connString = "context connection=true";

            Microsoft.SqlServer.Server.SqlTriggerContext context =
                Microsoft.SqlServer.Server.SqlContext.TriggerContext;

            bool isCreate = context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.CreateTable ||
                context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.CreateIndex ||
                context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.CreateSynonym ||
                context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.CreateView ||
                context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.CreateProcedure ||
                context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.CreateFunction ||
                context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.CreateTrigger ||
                context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.CreateEventNotification ||
                context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.CreateType ||
                context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.CreateAssembly;

            bool isAlter = context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.AlterTable ||
                           context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.AlterIndex ||
                           context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.AlterView ||
                           context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.AlterProcedure ||
                           context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.AlterFunction ||
                           context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.AlterTrigger ||
                           context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.AlterAssembly;

            bool isDrop = context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.DropTable ||
                          context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.DropIndex ||
                          context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.DropSynonym ||
                          context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.DropView ||
                          context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.DropProcedure ||
                          context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.DropFunction ||
                          context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.DropTrigger ||
                          context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.DropEventNotification ||
                          context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.DropType ||
                          context.TriggerAction == Microsoft.SqlServer.Server.TriggerAction.DropAssembly;

            if (isCreate || isAlter || isDrop)
            {
                string objectType = "";
                string objectName = "";
                string schemaName = "";
                string sql = "";


                string db_name = "";
                byte compatibility_level = 0;
                string server_version = "";
                string service_name = "";
                string machine_name = "";
                string server_name = "";
                string netbios_name = "";
                string instance = "";
                string edition = "";
                string product_level = "";
                string product_version = "";
                bool fulltext_installed = false;




                // Get the object type, name, and schema
                using (System.Data.Common.DbConnection conn =
                    new System.Data.SqlClient.SqlConnection(connString))
                {
                    bool haveToOpen = conn.State != System.Data.ConnectionState.Open;
                    if (haveToOpen)
                        conn.Open();

                    using (System.Data.Common.DbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT 
     EVENTDATA().value('(/EVENT_INSTANCE/ObjectType)[1]', 'nvarchar(100)') AS object_type 
    ,EVENTDATA().value('(/EVENT_INSTANCE/ObjectName)[1]', 'nvarchar(100)') AS object_name 
    ,EVENTDATA().value('(/EVENT_INSTANCE/SchemaName)[1]', 'nvarchar(100)') AS schema_name 
    ,EVENTDATA().value('(/EVENT_INSTANCE/TSQLCommand/CommandText)[1]', 'nvarchar(max)')
	,DB_NAME() AS db 
	,(SELECT compatibility_level FROM sys.databases WHERE name = DB_NAME()) AS compatibility_level 
	,@@VERSION AS server_version 
	,@@SERVICENAME AS service_name  
	,SERVERPROPERTY('MachineName') AS machine_name 
	,SERVERPROPERTY('ServerName')AS server_name 
	,SERVERPROPERTY('ComputerNamePhysicalNetBIOS') AS netbios_name 
	,SERVERPROPERTY('InstanceName') AS instance 
	,SERVERPROPERTY('Edition') AS edition 
	,SERVERPROPERTY('ProductLevel') AS product_level 
	,SERVERPROPERTY('ProductVersion') AS product_version 
	,SERVERPROPERTY('IsFullTextInstalled') AS fulltext_installed 
";
                        using (System.Data.Common.DbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                objectType = System.Convert.ToString(reader.GetValue(0), System.Globalization.CultureInfo.InvariantCulture);
                                objectName = System.Convert.ToString(reader.GetValue(1), System.Globalization.CultureInfo.InvariantCulture);
                                schemaName = System.Convert.ToString(reader.GetValue(2), System.Globalization.CultureInfo.InvariantCulture);
                                sql = System.Convert.ToString(reader.GetValue(3), System.Globalization.CultureInfo.InvariantCulture);

                                db_name = System.Convert.ToString(reader.GetValue(4), System.Globalization.CultureInfo.InvariantCulture);
                                compatibility_level = reader.GetByte(5); // tinyint
                                server_version = System.Convert.ToString(reader.GetValue(6), System.Globalization.CultureInfo.InvariantCulture);
                                service_name = System.Convert.ToString(reader.GetValue(7), System.Globalization.CultureInfo.InvariantCulture);
                                machine_name = System.Convert.ToString(reader.GetValue(8), System.Globalization.CultureInfo.InvariantCulture);
                                server_name = System.Convert.ToString(reader.GetValue(9), System.Globalization.CultureInfo.InvariantCulture);
                                netbios_name = System.Convert.ToString(reader.GetValue(10), System.Globalization.CultureInfo.InvariantCulture);
                                instance = System.Convert.ToString(reader.GetValue(11), System.Globalization.CultureInfo.InvariantCulture);
                                edition = System.Convert.ToString(reader.GetValue(12), System.Globalization.CultureInfo.InvariantCulture);
                                product_level = System.Convert.ToString(reader.GetValue(13), System.Globalization.CultureInfo.InvariantCulture);
                                product_version = System.Convert.ToString(reader.GetValue(14), System.Globalization.CultureInfo.InvariantCulture);
                                fulltext_installed = System.Convert.ToBoolean(reader.GetInt32(15));

                                // object obj = reader.GetValue(15);
                                // System.Convert.ToString(obj.GetType().FullName);
                            } // End if (reader.Read()) 

                        } // End Using reader 

                    } // End Using cmd 

                    if (haveToOpen && conn.State != System.Data.ConnectionState.Closed)
                        conn.Close();
                } // End Using conn 

                if (sql == null)
                    sql = "";

                if (objectName == null)
                    objectName = "";

                if (schemaName == null)
                    schemaName = "";

                try
                {
                    // Receiving-Endpoint in RestInterfaceCore
                    PostData(objectType, 
                        objectName, 
                        schemaName, 
                        context.TriggerAction.ToString(), 
                        sql.Replace("\"", "\\\""),
                        db_name,
                        compatibility_level,
                        server_version,
                        service_name,
                        machine_name,
                        server_name,
                        netbios_name,
                        instance,
                        edition,
                        product_level,
                        product_version,
                        fulltext_installed
                    );
                }
                catch (System.Exception ex)
                {
                    string errorMessage = "Error in CLR trigger: " + ex.Message;
                    Microsoft.SqlServer.Server.SqlContext.Pipe.Send(errorMessage);

                    // forward the exception to the client
                    int severity = 16; // use severity level greater than 10 to forward the exception
                    string state = "00000"; // state must be "00000" for user-defined errors
                    string message = ex.Message;
                    string procedure = "DatabaseVersionControl";
                    int line = 0; // use 0 for line number

                    // use the format string 'Error: {0}' to mimic a SQL Server error message format
                    string errorFormat = "Error: " + ex.Message;

                    // use the format string 'Error {0}, Level {1}, State {2}: {3}'
                    // to mimic a SQL Server error message format with severity and state
                    string errorFormatWithSeverity = "Error " + ex.Message
                        + ", Level " + severity.ToString(System.Globalization.CultureInfo.InvariantCulture)
                        + ", State " + state + ": " + procedure;


                    // Connect through the context connection.
                    using (System.Data.SqlClient.SqlConnection connection = 
                        new System.Data.SqlClient.SqlConnection("context connection=true"))
                    {
                        bool haveToOpen = connection.State != System.Data.ConnectionState.Open;
                        if (haveToOpen)
                            connection.Open();

                        using (System.Data.SqlClient.SqlCommand cmd = connection.CreateCommand())
                        {
                            // use the RAISERROR statement to forward the exception to the client
                            if (severity > 10)
                                cmd.CommandText = $"RAISERROR ('{errorFormatWithSeverity.Replace("'", "''")}', {severity.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {state}, '{message.Replace("'", "''")}', '{procedure.Replace("'", "''")}', {line.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
                            else
                                cmd.CommandText = $"RAISERROR ('{errorFormat.Replace("'", "''")}', 0, {state})";

                            // Execute the command and send the results directly to the client.
                            Microsoft.SqlServer.Server.SqlContext.Pipe.ExecuteAndSend(cmd);
                        } // End Using cmd 

                        if (haveToOpen && connection.State != System.Data.ConnectionState.Closed)
                            connection.Close();
                    } // End Using connection 

                } // End Catch 

            } // End if (isCreate || isAlter || isDrop)

        } // End Sub OnDatabaseChange 

        private enum TextualReturnFormat
        { 
            XML, 
            JSON, 
            PLAIN
        }


        private static string EncodeResult(
            TextualReturnFormat returnFormat,
            System.Net.HttpStatusCode statusCode, 
            string statusDescription, 
            string responseText
        )
        {
            string retValue = null;

            if (returnFormat == TextualReturnFormat.XML)
            {
                retValue = "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\" ?>"
                    + "<responseData xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">"
                    + "<statusCode>" + ((int)statusCode).ToString(System.Globalization.CultureInfo.InvariantCulture) + "</statusCode>"
                    + "<statusDescription>" + XmlEscape(statusDescription) + "</statusDescription>"
                    + "<responseText>" + XmlEscape(responseText) + "</responseText>"
                    // + "<someValue xsi:nil=\"true\" />"
                    + "</responseData>";

                return retValue;
            }
            
            if (returnFormat == TextualReturnFormat.JSON)
            {
                retValue = "{"
                    + " \"statusCode\":" + ((int)statusCode).ToString(System.Globalization.CultureInfo.InvariantCulture)
                    + ",\"statusDescription\":" + JsonEscapeString(statusDescription, true)
                    + ",\"responseText\":" + JsonEscapeString(responseText, true)
                    + "}";

                return retValue;
            }


            retValue = "StatusCode: " + ((int)statusCode).ToString(System.Globalization.CultureInfo.InvariantCulture) + System.Environment.NewLine
                + "StatusDescription: " + XmlEscape(statusDescription) + System.Environment.NewLine
                + "ResponseText: " + XmlEscape(responseText) + System.Environment.NewLine
                ;

            return retValue;
        }



        private static string InternalHttpRequest(
            bool isInternal,
            string method,
            string url,
            string contentType,
            string payLoad,  
            int timeout, 
            string accept)
        {
            try
            {
                byte[] dataBytes = new System.Text.UTF8Encoding(false).GetBytes(payLoad);
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)
                    System.Net.WebRequest.Create(url);
                request.Method = method;
                request.ContentType = contentType;

                // Ignore SSL certificate errors
                System.Net.ServicePointManager.ServerCertificateValidationCallback += 
                    CertificateCallback.SelfSignedCertificateValidationCallBack;

                if (accept != null)
                {
                    accept = accept.Trim();
                    if (!accept.EndsWith("charset=utf-8", System.StringComparison.OrdinalIgnoreCase))
                        accept += ";charset=utf-8";

                    request.Accept = accept;
                } // End if (accept != null)

                request.ContentLength = dataBytes.Length;
                request.Timeout = timeout;

                using (System.IO.Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(dataBytes, 0, dataBytes.Length);
                } // End Using requestStream 

                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
                
                string responseText = null;

                using (System.IO.StreamReader reader =
                    new System.IO.StreamReader(response.GetResponseStream()))
                {
                    responseText = reader.ReadToEnd();
                } // End Using reader

                return EncodeResult(TextualReturnFormat.XML, response.StatusCode, response.StatusDescription, responseText);
            }
            catch (System.Net.WebException wex)
            {
                if (wex.Response != null)
                {
                    using (System.Net.HttpWebResponse errorResponse =
                        (System.Net.HttpWebResponse)wex.Response)
                    {
                        string errorText = null;

                        using (System.IO.StreamReader reader =
                            new System.IO.StreamReader(errorResponse.GetResponseStream()))
                        {
                            errorText = reader.ReadToEnd();
                            // Do something with the error text
                            System.Console.WriteLine(errorText);
                        } // End Using reader 

                        string retValue = EncodeResult(TextualReturnFormat.XML,
                            errorResponse.StatusCode, 
                            errorResponse.StatusDescription,
                            errorText
                        );

                        if (isInternal)
                            throw new System.Exception(
                                ((int)errorResponse.StatusCode)
                                .ToString(System.Globalization.CultureInfo.InvariantCulture)
                                + " - "
                                + errorResponse.StatusDescription
                                + System.Environment.NewLine
                                + System.Environment.NewLine 
                                + errorText)
                            ;

                        return retValue;
                    } // End Using errorResponse 

                }
                else
                {
                    if (isInternal)
                        throw;

                    string retValue = EncodeResult(
                        TextualReturnFormat.XML,
                        System.Net.HttpStatusCode.InternalServerError,
                        wex.Message,
                        wex.StackTrace
                    );

                    return retValue;
                }

            }
            catch (System.Exception ex)
            {
                if (isInternal)
                    throw;

                string retValue = EncodeResult(
                    TextualReturnFormat.XML,
                    System.Net.HttpStatusCode.BadGateway,
                    ex.Message,
                    ex.StackTrace
                );

                return retValue;
            }

        } // End Sub HttpRequest 


        private static System.Data.SqlTypes.SqlXml CreateSqlXmlFromString(string xmlString)
        {
            System.Data.SqlTypes.SqlXml sqlXml = null;

            using (System.IO.TextReader stringReader = new System.IO.StringReader(xmlString))
            {
                using (System.Xml.XmlTextReader xmlReader = 
                    new System.Xml.XmlTextReader(stringReader))
                {
                    sqlXml = new System.Data.SqlTypes.SqlXml(xmlReader);
                }
            }
            
            return sqlXml;
        }


        [Microsoft.SqlServer.Server.SqlFunction]
        public static 
            // System.Data.SqlTypes.SqlString 
            System.Data.SqlTypes.SqlXml
            HttpRequest(
            System.Data.SqlTypes.SqlString method,
            System.Data.SqlTypes.SqlString url,
            System.Data.SqlTypes.SqlString contentType,
            System.Data.SqlTypes.SqlString payload,
            System.Data.SqlTypes.SqlInt32 timeout,
            System.Data.SqlTypes.SqlString accept
        )
        {
            string ret= InternalHttpRequest(false, method.Value, url.Value, contentType.Value, payload.Value, timeout.Value, accept.Value);

            // return new System.Data.SqlTypes.SqlString(ret);
            return CreateSqlXmlFromString(ret);
        } // End Function HttpRequest 



        private static string ToJson(
          string objectType,
          string objectName,
          string schemaName,
          string action,
          string sql,
          string db_name,
          byte compatibility_level,
          string server_version,
          string service_name,
          string machine_name,
          string server_name,
          string netbios_name,
          string instance,
          string edition,
          string product_level,
          string product_version,
          bool fulltext_installed
      )
        {
            // Post the information to the external URL
            string json = "{"
                + " \"event_type\":" + JsonEscapeString(action, true)
                + ",\"object_type\":" + JsonEscapeString(objectType, true)
                + ",\"object_name\":" + JsonEscapeString(objectName, true)
                + ",\"schema_name\":" + JsonEscapeString(schemaName, true)
                + ",\"sql\":" + JsonEscapeString(sql, true)
                + ",\"db_name\":" + JsonEscapeString(db_name, true)
                + ",\"compatibility_level\":" + compatibility_level.ToString(System.Globalization.CultureInfo.InvariantCulture)
                + ",\"server_version\":" + JsonEscapeString(server_version, true)
                + ",\"service_name\":" + JsonEscapeString(service_name, true)
                + ",\"machine_name\":" + JsonEscapeString(machine_name, true)
                + ",\"server_name\":" + JsonEscapeString(server_name, true)
                + ",\"netbios_name\":" + JsonEscapeString(netbios_name, true)
                + ",\"instance\":" + JsonEscapeString(instance, true)
                + ",\"edition\":" + JsonEscapeString(edition, true)
                + ",\"product_level\":" + JsonEscapeString(product_level, true)
                + ",\"product_version\":" + JsonEscapeString(product_version, true)
                + ",\"fulltext_installed\":" + fulltext_installed.ToString(System.Globalization.CultureInfo.InvariantCulture).ToLowerInvariant()
                + "}";

            return json;
        }



        private static string ToXml(
          string objectType,
          string objectName,
          string schemaName,
          string action,
          string sql,
          string db_name,
          byte compatibility_level,
          string server_version,
          string service_name,
          string machine_name,
          string server_name,
          string netbios_name,
          string instance,
          string edition,
          string product_level,
          string product_version,
          bool fulltext_installed
        )
        {
            // Post the information to the external URL
            string xml = "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\" ?>"
                + "<event_data xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">"
                + "<event_type>" + XmlEscape(action) + "</event_type>"
                + "<object_type>" + XmlEscape(objectType) + "</object_type>"
                + "<object_name>" + XmlEscape(objectName) + "</object_name>"
                + "<schema_name>" + XmlEscape(schemaName) + "</schema_name>"
                + "<sql>" + XmlEscape(sql) + "</sql>"
                + "<db_name>" + XmlEscape(db_name) + "</db_name>"
                + "<compatibility_level>" + compatibility_level.ToString(System.Globalization.CultureInfo.InvariantCulture) + "</compatibility_level>"
                + "<server_version>" + XmlEscape(server_version) + "</server_version>"
                + "<service_name>" + XmlEscape(service_name) + "</service_name>"
                + "<machine_name>" + XmlEscape(machine_name) + "</machine_name>"
                + "<server_name>" + XmlEscape(server_name) + "</server_name>"
                + "<netbios_name>" + XmlEscape(netbios_name) + "</netbios_name>"
                + "<instance>" + XmlEscape(instance) + "</instance>"
                + "<edition>" + XmlEscape(edition) + "</edition>"
                + "<product_level>" + XmlEscape(product_level) + "</product_level>"
                + "<product_version>" + XmlEscape(product_version) + "</product_version>"
                + "<fulltext_installed>" + fulltext_installed.ToString(System.Globalization.CultureInfo.InvariantCulture).ToLowerInvariant() + "</fulltext_installed>"
                + "</event_data>"
                ;


            return xml;
        }




        private static void PostData(
            string objectType,
            string objectName,
            string schemaName,
            string action,
            string sql,
            string db_name,
            byte compatibility_level,
            string server_version,
            string service_name,
            string machine_name,
            string server_name,
            string netbios_name,
            string instance,
            string edition,
            string product_level,
            string product_version,
            bool fulltext_installed
        )
        {
            // Post the information to the external URL
            string payLoad = ToJson(
                      objectType,
                      objectName,
                      schemaName,
                      action,
                      sql,
                      db_name,
                      compatibility_level,
                      server_version,
                      service_name,
                      machine_name,
                      server_name,
                      netbios_name,
                      instance,
                      edition,
                      product_level,
                      product_version,
                      fulltext_installed
                  );


            InternalHttpRequest(
                true,
                "POST",
                "https://localhost:44302/reportSchemaChange",
                "application/json; charset=utf-8", 
                payLoad, 
                System.Threading.Timeout.Infinite, 
                "application/xml"
            );
        } // End Sub PostData 



        private static char[] disallowedCharacters = new char[] { 
            '\u0000', // NUL 
            '\u0009', // horizontal tab 
            '\u000A', // line feed 
            '\u000D', // carriage return
            '\u000B', // vertical tab
            '\u000C' // form-feed
        };


        public static string XmlEscape(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i < input.Length; ++i)
            {
                char c = input[i];

                if (c == '&')
                {
                    sb.Append("&amp;");
                }
                else if (c == '<')
                {
                    sb.Append("&lt;");
                }
                else if (c == '>')
                {
                    sb.Append("&gt;");
                }
                else if (c == '\'')
                {
                    sb.Append("&apos;");
                }
                else if (c == '\"')
                {
                    sb.Append("&quot;");
                }
                else if(System.Array.IndexOf(disallowedCharacters, c) >= 0)
                {
                    sb.AppendFormat("&#{0};", (int)c);
                }
                else
                {
                    sb.Append(c);
                }
            } // Next i 

            return sb.ToString();
        } // End Function XmlEscape 


        public static string SimpleXmlEscape(string input)
        {
            string strXmlText = "";

            if (string.IsNullOrEmpty(input))
                return input;


            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i < input.Length; ++i)
            {
                char c = input[i];

                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                {
                    // Append characters A-Z and a-z directly without escaping
                    sb.Append(c);
                }
                else
                    sb.AppendFormat("&#{0};", (int)c);
            }

            strXmlText = sb.ToString();
            sb.Clear();
            sb = null;

            return strXmlText;
        } // End Function SimpleXmlEscape 


        private static bool NeedEscape(string src, int i)
        {
            char c = src[i];
            return c < 32 || c == '"' || c == '\\'
                // Broken lead surrogate
                || (c >= '\uD800' && c <= '\uDBFF' &&
                    (i == src.Length - 1 || src[i + 1] < '\uDC00' || src[i + 1] > '\uDFFF'))
                // Broken tail surrogate
                || (c >= '\uDC00' && c <= '\uDFFF' &&
                    (i == 0 || src[i - 1] < '\uD800' || src[i - 1] > '\uDBFF'))
                // To produce valid JavaScript
                || c == '\u2028' || c == '\u2029'
                // Escape "</" for <script> tags
                || (c == '/' && i > 0 && src[i - 1] == '<');
        } // End Function NeedEscape 


        private static string JsonEscapeString(string src, bool addQuotes)
        {
            if (addQuotes)
            {
                if (src == null)
                    return "null";
                else
                    return ("\"" + JsonEscapeString(src) + "\"");
            } // End if (addQuotes) 

            return JsonEscapeString(src);
        } // End Function JsonEscapeString 


        private static string JsonEscapeString(string src)
        {
            if (null == src)
                return null;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            int start = 0;
            for (int i = 0; i < src.Length; i++)
                if (NeedEscape(src, i))
                {
                    sb.Append(src, start, i - start);
                    switch (src[i])
                    {
                        case '\b': sb.Append("\\b"); break;
                        case '\f': sb.Append("\\f"); break;
                        case '\n': sb.Append("\\n"); break;
                        case '\r': sb.Append("\\r"); break;
                        case '\t': sb.Append("\\t"); break;
                        case '\"': sb.Append("\\\""); break;
                        case '\\': sb.Append("\\\\"); break;
                        case '/': sb.Append("\\/"); break;
                        default:
                            sb.Append("\\u");
                            sb.Append(((int)src[i]).ToString("x04"));
                            break;
                    }
                    start = i + 1;
                }
            sb.Append(src, start, src.Length - start);
            return sb.ToString();
        } // End Function EscapeString 


    } // End Class DatabaseVersionControlTrigger 


} // End Namespace DatabaseVersionControl 
