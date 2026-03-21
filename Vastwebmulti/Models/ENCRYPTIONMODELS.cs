using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Vastwebmulti.Models
{
    /// <summary>
    /// Utility class providing AES/RSA encryption and decryption for sensitive data
    /// </summary>
    public class ENCRYPTIONMODELS
    {

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }


        public string convertdatatoALL(string jsondataall, string keyssss, string IVIDSss)
        {
            using (RijndaelManaged myRijndael = new RijndaelManaged())
            {
                // string original = "PRIYA Madam Sunday ko aayegi";


                byte[] Key = ASCIIEncoding.ASCII.GetBytes(keyssss);

                byte[] IV = ASCIIEncoding.ASCII.GetBytes(IVIDSss);

                // Encrypt the string to an array of bytes.
                byte[] encrypted = EncryptStringToBytes(jsondataall, Key, IV);
                string temp_inBase64 = Convert.ToBase64String(encrypted);
                //   byte[] encrypted = ASCIIEncoding.ASCII.GetBytes("B9lcwNJDWuhypnsNQ28WwA==");
                // Decrypt the bytes to a string.
                //  var chkk= "vntEFzDyiLY0Ew4WkP0Uzw==";
                // var chkk = "20BmypUhj+P425NMU71MiZ7PXLRNgmouGW36yebug1/FJRhB6ymZF9wZB8ei+7ws";

                return temp_inBase64;
                //  byte[] bytes = System.Convert.FromBase64String(temp_inBase64);
                //   string roundtrip = DecryptStringFromBytes(bytes, Key, IV);
                // return roundtrip;
                //Display the original data and the decrypted data.
                // Console.WriteLine("Original:   {0}", original);
                //  Console.WriteLine("Round Trip: {0}", roundtrip);
            }
        }

        public string DecryptdatatoALL(string jsondataall, string keyssss, string IVIDSss)
        {
            using (RijndaelManaged myRijndael = new RijndaelManaged())
            {
                byte[] Key = ASCIIEncoding.ASCII.GetBytes(keyssss);

                byte[] IV = ASCIIEncoding.ASCII.GetBytes(IVIDSss);
                byte[] bytes = System.Convert.FromBase64String(jsondataall);
                string roundtrip = DecryptStringFromBytes(bytes, Key, IV);
                return roundtrip;
            }


        }


        static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;


                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }


    }
}