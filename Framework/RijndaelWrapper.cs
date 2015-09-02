using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Framework
{
    public class RijndaelWrapper
    {
        public string Encrypt(string clearText, string password, string saltStr)
        {
            byte[] clearBytes =
              System.Text.Encoding.Unicode.GetBytes(clearText);

            byte[] salt = Convert.FromBase64String(saltStr);

            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, salt);

            byte[] encryptedData = Encrypt(clearBytes, pdb.GetBytes(32), pdb.GetBytes(16));

            return Convert.ToBase64String(encryptedData);
        }

        public byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            byte[] encryptedData;
            using (MemoryStream ms = new MemoryStream())
            {
                using (Rijndael alg = Rijndael.Create())
                {
                    alg.Key = Key;
                    alg.IV = IV;
                    using (CryptoStream cs = new CryptoStream(ms,
                       alg.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearData, 0, clearData.Length);
                    }
                    encryptedData = ms.ToArray();
                }
            }

            return encryptedData;
        }

        public string Decrypt(string cipherText, string password, string saltStr)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            byte[] salt = Convert.FromBase64String(saltStr);

            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, salt);
            byte[] decryptedData = Decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16));

            return Encoding.Unicode.GetString(decryptedData);
        }

        public byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
        {
            byte[] decryptedData;
            using (MemoryStream ms = new MemoryStream())
            {
                using (Rijndael alg = Rijndael.Create())
                {
                    alg.Key = Key;
                    alg.IV = IV;
                    using (CryptoStream cs = new CryptoStream(ms,
                        alg.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherData, 0, cipherData.Length);
                    }
                    decryptedData = ms.ToArray();
                }
            }
            return decryptedData;
        }
    }
}