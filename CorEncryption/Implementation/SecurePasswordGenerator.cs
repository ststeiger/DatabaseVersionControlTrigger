
namespace CorEncryption
{


    internal static class SecurePasswordGenerator
    {
        private const string UppercaseChars = "ABCDEFGHJKMNPQRSTUVXYZ"; // Excludes I, L, O, W
        private const string LowercaseChars = "abcdefghijkmnpqrstuvxyz"; // Keeps lowercase i/l for balance
        private const string NumberChars = "123456789"; // removed zero because 0 is difficult to differentiate with O
        private const string SpecialChars = "!@#$%*()-_=+[]{}:?,"; // removed ^ because peope might not know how to input, removed ; because connectionString
        private const string UpperLower = UppercaseChars + LowercaseChars;
        private const string AllChars = UppercaseChars + LowercaseChars + NumberChars + SpecialChars;
        

#if true 

        private static readonly System.Security.Cryptography.RNGCryptoServiceProvider s_global = new System.Security.Cryptography.RNGCryptoServiceProvider();


        private static void GetBytes(byte[] randomBytes)
        {
            s_global.GetBytes(randomBytes);
        } // End Sub GetBytes 

#else
        
        private static void GetBytes(byte[] randomBytes)
        {
            using (System.Security.Cryptography.RandomNumberGenerator rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
        } // End Sub GetBytes 

#endif


        // https://web.archive.org/web/20160204165658/http://blogs.msdn.com/b/pfxteam/archive/2009/02/19/9434171.aspx
        private static int Next(int minValue, int maxValue)
        {
            byte[] buffer = new byte[4];
            s_global.GetBytes(buffer);
            int result = System.BitConverter.ToInt32(buffer, 0) & int.MaxValue; // Ensure positive value
            return minValue + (result % (maxValue - minValue));
        } // End Function Next 


        private static void SwapWithRandomLetter(char[] array, int index, byte[] randomBytes)
        {
            for (int i = array.Length - 1; i >= 0; i--)
            {

                if (UpperLower.Contains(array[i].ToString())) // Find a letter
                {
                    // (array[index], array[i]) = (array[i], array[index]); // Swap

                    // Swap 
                    char temp = array[index];
                    array[index] = array[i];
                    array[i] = temp;

                    break;
                } // End if (UpperLower.Contains(array[i])) 

            } // Next i 

        } // End Sub SwapWithRandomLetter 


        private static void EnsureFirstLastAreLetters(char[] array, byte[] randomBytes)
        {
            // Find a letter in the array to swap if needed
            int length = array.Length;

            if (!UpperLower.Contains(array[0].ToString()))
                SwapWithRandomLetter(array, 0, randomBytes);

            if (!UpperLower.Contains(array[length - 1].ToString()))
                SwapWithRandomLetter(array, length - 1, randomBytes);
        } // End Sub EnsureFirstLastAreLetters 


        private static void ShuffleArray(char[] array, byte[] randomBytes)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = randomBytes[i] % (i + 1);
                // (array[i], array[j]) = (array[j], array[i]); // Swap
                
                char temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            } // Next i 

        } // End Sub ShuffleArray 


        private static string ShuffleString(string input, byte[] randomBytes)
        {
            char[] array = input.ToCharArray();
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = randomBytes[i] % (i + 1);
                char temp = array[i];
                array[i] = array[j];
                array[j] = temp; // Manual swap for .NET Framework 4.8
            } // Next i 

            return new string(array);
        } // End Function ShuffleString 


        // SecurePasswordGenerator.GeneratePassword
        public static string GeneratePassword(
             int length 
            ,int numUppercase 
            ,int numNumbers 
            ,int numSpecials    
        )
        {
            if (length < 1)
                throw new System.ArgumentException("Password length must be at least 1.", nameof(length));

            byte[] randomBytes = new byte[length];
            GetBytes(randomBytes);

            System.Text.StringBuilder password = new System.Text.StringBuilder(length);

            // Ensure required characters if enabled
            if (numUppercase > 0)
            {
                for(int i = 0; i < numUppercase; ++i)
                    password.Append(UppercaseChars[randomBytes[password.Length] % UppercaseChars.Length]);
            } // End if (numUppercase > 0) 

            if (numNumbers > 0)
            {
                for (int i = 0; i < numNumbers; ++i)
                    password.Append(NumberChars[randomBytes[password.Length] % NumberChars.Length]);
            } // End if (numNumbers > 0) 

            if (numSpecials > 0)
            {
                for (int i = 0; i < numSpecials; ++i)
                    password.Append(SpecialChars[randomBytes[password.Length] % SpecialChars.Length]);
            } // End if (numSpecials > 0) 


            // Fill remaining slots with random characters
            int startIndex = password.Length;
            for (int i = startIndex; i < length; i++)
            {
                password.Append(UpperLower[randomBytes[i] % UpperLower.Length]);
            } // Next i 


            // return ShuffleString(password.ToString(), randomBytes);

            // Shuffle password
            char[] shuffledPassword = password.ToString().ToCharArray();
            ShuffleArray(shuffledPassword, randomBytes);

            // Ensure first and last characters are letters
            EnsureFirstLastAreLetters(shuffledPassword, randomBytes);

            return new string(shuffledPassword);
        } // End Function GeneratePassword 


        // SecurePasswordGenerator.GenerateCorPassword
        public static string GenerateCorPassword()
        {
            return GeneratePassword(8, numUppercase: 1, numNumbers: 1, numSpecials: 1);
        } // End Function GenerateCorPassword 


        public static string[] GenerateCorPasswords(int num)
        {
            string[] passwords = new string[num];

            for(int i = 0; i< num; ++i)
                passwords[i] = GenerateCorPassword();

            return passwords;
        } // End Function GenerateCorPassword 


        internal static void Test()
        {
            // for(int i = 0; i < 100; ++i)
            // System.Console.WriteLine("Generated Password: " + GeneratePassword(8, numUppercase: 1, numNumbers: 1, numSpecials: 1));
            // System.Console.WriteLine("Generated Password: " + GenerateCorPassword());

            System.Console.WriteLine("Generated Passwords: \r\n" + string.Join(System.Environment.NewLine, GenerateCorPasswords(10)));
        } // End Sub Test 


    } // End Class SecurePasswordGenerator 


} // End Namespace 
