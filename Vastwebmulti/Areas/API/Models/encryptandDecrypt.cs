using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace Vastwebmulti.Areas.API.Models
{
    public class encryptandDecrypt
    {
       #region Encrypt
        public string EncryptText(string input, string password)
        {
            // Get the bytes of the string
            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(input);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            string result = Convert.ToBase64String(bytesEncrypted);

            return result;
        }
        public byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }
        public  string EncryptTextsalt(string inputValue, string inputsalt)

        {

            string salt = "test123455666666666" + inputsalt;

            byte[] utfData = UTF8Encoding.UTF8.GetBytes(inputValue);

            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

            string encryptedString = string.Empty;

            using (AesManaged aes = new AesManaged())

            {

                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(salt, saltBytes);

                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;

                aes.KeySize = aes.LegalKeySizes[0].MaxSize;

                aes.Key = rfc.GetBytes(aes.KeySize / 8);

                aes.IV = rfc.GetBytes(aes.BlockSize / 8);

                using (ICryptoTransform encryptTransform = aes.CreateEncryptor())

                {

                    using (MemoryStream encryptedStream = new MemoryStream())

                    {

                        using (CryptoStream encryptor =

                        new CryptoStream(encryptedStream, encryptTransform, CryptoStreamMode.Write))

                        {

                            encryptor.Write(utfData, 0, utfData.Length);

                            encryptor.Flush();

                            encryptor.Close();

                            byte[] encryptBytes = encryptedStream.ToArray();

                            encryptedString = Convert.ToBase64String(encryptBytes);

                        }

                    }

                }

            }

            return encryptedString;

        }
        #endregion 


        public string DecryptText(string input, string password)
        {
            // Get the bytes of the string
            byte[] bytesToBeDecrypted = Convert.FromBase64String(input);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            string result = Encoding.UTF8.GetString(bytesDecrypted);

            return result;
        }
        public byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }
        public  string DecryptTextsalt(string inputValue, string inputsalt)

        {

            string salt = "test123455666666666" + inputsalt;

            byte[] encryptedBytes = Convert.FromBase64String(inputValue);

            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

            string decryptedString = string.Empty;

            using (var aes = new AesManaged())

            {

                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(salt, saltBytes);

                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;

                aes.KeySize = aes.LegalKeySizes[0].MaxSize;

                aes.Key = rfc.GetBytes(aes.KeySize / 8);

                aes.IV = rfc.GetBytes(aes.BlockSize / 8);

                using (ICryptoTransform decryptTransform = aes.CreateDecryptor())

                {

                    using (MemoryStream decryptedStream = new MemoryStream())

                    {

                        CryptoStream decryptor = new CryptoStream(decryptedStream, decryptTransform, CryptoStreamMode.Write);

                        decryptor.Write(encryptedBytes, 0, encryptedBytes.Length);

                        decryptor.Flush();

                        decryptor.Close();

                        byte[] decryptBytes = decryptedStream.ToArray();

                        decryptedString = UTF8Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);

                    }

                }

            }

            return decryptedString;

        }

    }
}