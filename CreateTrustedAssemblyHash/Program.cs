
namespace CreateTrustedAssemblyHash
{


    internal class Program
    {


        static string ComputeAssemblyHash(byte[] inputBytes, string assembyName)
        {
            // DatabaseVersionControl.DatabaseVersionControlTrigger.OnDatabaseChange();
            // ALTER TABLE dbo.T_Benutzer ADD xxx int NULL;
            // ALTER TABLE dbo.T_Benutzer DROP COLUMN xxx;

            string sql = null;

            using (System.Security.Cryptography.SHA512 sha512 =
                System.Security.Cryptography.SHA512.Create())
            {
                byte[] hashBytes = sha512.ComputeHash(inputBytes);
                string formattedHash = "0x" + System.BitConverter.ToString(hashBytes).Replace("-", "");

                // sql = "EXEC sp_add_trusted_assembly " + formattedHash + ", N'DatabaseVersionControl'; ";
                sql = "EXEC sp_add_trusted_assembly " + formattedHash + ", N'" + assembyName.Replace("'", "''") + "'; ";
                sql += System.Environment.NewLine;
                sql += "-- EXEC sp_drop_trusted_assembly " + formattedHash + "; ";

                System.Console.WriteLine(sql);
            } // End Using sha512 

            return sql;
        } // End Sub ComputeAssemblyHash 


        static string ComputeAssemblyHash(string assemblyLocation)
        {
            string assemblyName = System.IO.Path.GetFileNameWithoutExtension(assemblyLocation);
            byte[] inputBytes = System.IO.File.ReadAllBytes(assemblyLocation);

            return ComputeAssemblyHash(inputBytes, assemblyName);
        } // End Function ComputeAssemblyHash 


        public static string CreateAssemblyStatement(string assemblyLocation)
        {
            string assemblyName = System.IO.Path.GetFileNameWithoutExtension(assemblyLocation);
            byte[] inputBytes = System.IO.File.ReadAllBytes(assemblyLocation);

            string hash = ComputeAssemblyHash(inputBytes, assemblyName);
            string hexString = ExampleClrTrigger.ByteArrayHelper.ByteArrayToHex(inputBytes);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine(hash);
            sb.AppendLine(System.Environment.NewLine);
            sb.AppendLine("CREATE ASSEMBLY " + assemblyName + " ");

            // sb.AppendLine(@"-- FROM N'D:\username\Documents\Visual Studio 2022\github\DatabaseVersionControlTrigger\DatabaseVersionControl\bin\Debug\DatabaseVersionControl.dll' ");
            sb.Append(@"-- FROM N'");
            sb.Append(assemblyLocation.Replace("'", "''"));
            sb.AppendLine("' ");

            sb.Append("FROM 0x"); sb.Append(hexString); sb.AppendLine(" ");
            sb.AppendLine("-- WITH PERMISSION_SET = SAFE ");
            sb.AppendLine("-- WITH PERMISSION_SET = EXTERNAL_ACCESS ");
            sb.AppendLine("WITH PERMISSION_SET = UNSAFE ");
            sb.AppendLine("; ");

            string sql = sb.ToString();
            System.Console.WriteLine(sql);

            return sql;
        } // End Function CreateAssemblyStatement 


        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [System.STAThread]
        public static async Task<int> Main(string[] args)
        {
            string loc = typeof(CorEncryption.DES).Assembly.Location;
            // loc = @"D:\username\Documents\Visual Studio 2017\TFS\Tools\EncryptionUtility\ClrEncryptDecrypt\bin\Debug\ClrEncryptDecrypt.dll";
            
            string assemblyHash = ComputeAssemblyHash(loc);
            string assemblyCreate = CreateAssemblyStatement(loc);


            await System.Console.Out.WriteLineAsync(assemblyCreate);
            await System.Console.Out.WriteLineAsync("Finished");
            System.Console.ReadKey();

            return 0;
        } // End Sub Main 


    } // End Class 


}
