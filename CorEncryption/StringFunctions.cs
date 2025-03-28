
namespace CorEncryption
{


    // SQL String is a sqltype in .net framework.It represents a variable-length stream of characters to be stored in or retrieved from the database.

    // -- SQLChar is a is a mutable reference type that wraps a Char array or a SqlString instance.
    // SqlChar is a class. And what u found is right that SqlString is limited to nvarchar(4000) and SqlChars is nvarchar(max).
    // (But we can define the size, precision and scale of parameters and return types by using SqlFacet attribute.)
    public class StringFunctions
    {
        // SqlString is limited to nvarchar(4000) and SqlChars is nvarchar(max) ??? 


        [Microsoft.SqlServer.Server.SqlFunction(DataAccess = Microsoft.SqlServer.Server.DataAccessKind.None, IsDeterministic = true, IsPrecise = true)]
        [return: Microsoft.SqlServer.Server.SqlFacet(MaxSize = -1, IsFixedLength = false)] // nvarchar(MAX)
        public static System.Data.SqlTypes.SqlString Encrypt(System.Data.SqlTypes.SqlString input)
        {
            if (input.IsNull)
                return System.Data.SqlTypes.SqlString.Null;

            if (input.Value == string.Empty)
                return new System.Data.SqlTypes.SqlString(string.Empty);

            string result = null;

            try
            {
                result = DES.Crypt(input.Value);
            }
            catch (System.Exception ex)
            {
                result = "ERR: " + ex.Message;
            }

            // System.Data.SqlTypes.SqlChars charsResult = new System.Data.SqlTypes.SqlChars(result.ToCharArray());
            // Return as SqlString which will be mapped to nvarchar in SQL Server
            return new System.Data.SqlTypes.SqlString(result);
        }



        [Microsoft.SqlServer.Server.SqlFunction(DataAccess = Microsoft.SqlServer.Server.DataAccessKind.None, IsDeterministic = true, IsPrecise = true)]
        [return: Microsoft.SqlServer.Server.SqlFacet(MaxSize = -1, IsFixedLength = false)] // nvarchar(MAX)
        public static System.Data.SqlTypes.SqlString Decrypt(System.Data.SqlTypes.SqlString input)
        {
            if (input.IsNull)
                return System.Data.SqlTypes.SqlString.Null;

            if (input.Value == string.Empty)
                return new System.Data.SqlTypes.SqlString(string.Empty);

            string result = null;

            try 
            {
                result = DES.DeCrypt(input.Value);
            }
            catch (System.Exception ex)
            {
                result = "ERR: " + ex.Message;
            }
            
            // Return as SqlString which will be mapped to nvarchar in SQL Server 
            return new System.Data.SqlTypes.SqlString(result);
        }


        [Microsoft.SqlServer.Server.SqlFunction(DataAccess = Microsoft.SqlServer.Server.DataAccessKind.None
            , IsDeterministic = false // Important: Password generation should NOT be deterministic
            , IsPrecise = true
        )]
        public static System.Data.SqlTypes.SqlString GeneratePassword(
            System.Data.SqlTypes.SqlInt32 length,
            System.Data.SqlTypes.SqlInt32 numUppercase,
            System.Data.SqlTypes.SqlInt32 numNumbers,
            System.Data.SqlTypes.SqlInt32 numSpecials
        )
        {
            // Validate input parameters
            if (length.IsNull || numUppercase.IsNull || numNumbers.IsNull || numSpecials.IsNull)
                return System.Data.SqlTypes.SqlString.Null;

            if(length.Value < 1 || numUppercase.Value < 0 || numNumbers.Value < 0 || numSpecials.Value < 0)
                return new System.Data.SqlTypes.SqlString("ERR: Invalid");

            string result = SecurePasswordGenerator.GeneratePassword(length.Value, numUppercase.Value, numNumbers.Value, numSpecials.Value);
            return new System.Data.SqlTypes.SqlString(result);
        }


        [Microsoft.SqlServer.Server.SqlFunction(
             DataAccess = Microsoft.SqlServer.Server.DataAccessKind.None,
             IsDeterministic = false, // Important for random generation
             IsPrecise = true
         )]
        public static System.Data.SqlTypes.SqlString GenerateCorPassword()
        {
            string result = SecurePasswordGenerator.GenerateCorPassword();
            return new System.Data.SqlTypes.SqlString(result);
        }


        [Microsoft.SqlServer.Server.SqlFunction(FillRowMethodName = "FillRow", TableDefinition = "pw nvarchar(MAX)") ]
        public static System.Collections.IEnumerable tfu_GenerateCorPasswords(System.Data.SqlTypes.SqlInt32 num)
        {
            if (num.IsNull || num.Value < 1)
                return new string[0];

            // Your logic to generate the string array based on the input num
            string[] result = SecurePasswordGenerator.GenerateCorPasswords(num.Value);
            return result;
        }


        private static void FillRow(object obj, out System.Data.SqlTypes.SqlString pw)
        {
            pw = new System.Data.SqlTypes.SqlString(obj.ToString());
        }


    } // End Class StringFunctions 


} // End Namspace 
