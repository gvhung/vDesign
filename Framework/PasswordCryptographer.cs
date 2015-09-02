using System;
using System.Security.Cryptography;

namespace Framework
{
    public class PasswordCryptographer
    {
        const int saltLength = 6;
        const string delim = "*";

        private string SaltPassword(string password, string salt)
        {
            SHA512 hashAlgorithm = SHA512.Create();
            return Convert.ToBase64String(hashAlgorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes(salt + password)));
        }
        public virtual string GenerateSaltedPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return password;
            byte[] randomSalt = new byte[saltLength];
            new RNGCryptoServiceProvider().GetBytes(randomSalt);

            string salt = Convert.ToBase64String(randomSalt);
            return salt + delim + SaltPassword(password, salt);
        }

        public virtual bool AreEqual(string saltedPassword, string password)
        {
            if (string.IsNullOrEmpty(saltedPassword))
                return string.IsNullOrEmpty(password);
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }
            int delimPos = saltedPassword.IndexOf(delim);
            if (delimPos <= 0)
            {
                return saltedPassword.Equals(password);
            }
            else
            {
                string calculatedSaltedPassword = SaltPassword(password, saltedPassword.Substring(0, delimPos));
                string expectedSaltedPassword = saltedPassword.Substring(delimPos + delim.Length);
                if (expectedSaltedPassword.Equals(calculatedSaltedPassword))
                {
                    return true;
                }
                return expectedSaltedPassword.Equals(SaltPassword(password, "System.Byte[]"));
            }
        }
    }
}
