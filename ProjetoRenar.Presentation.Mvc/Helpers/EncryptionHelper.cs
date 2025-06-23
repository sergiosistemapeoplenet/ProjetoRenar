using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoRenar.Presentation.Mvc.Helpers
{
    public static class EncryptionHelper
    {
        private static string secretKey = "27D2E221BA3C45DAA646A7F2AD3D9695";

        public static string Encrypt(string stringToEncrypt)
        {
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(secretKey);
                    aesAlg.IV = new byte[16]; // Usando IV nulo para simplificação, considere usar um IV adequado em um cenário real

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    byte[] inputBytes = Encoding.UTF8.GetBytes(stringToEncrypt);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

                    string base64Result = Convert.ToBase64String(encryptedBytes);
                    return SafeBase64Encode(base64Result);
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static string Decrypt(string EncryptedText)
        {
            try
            {
                string base64Input = SafeBase64Decode(EncryptedText);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(secretKey);
                    aesAlg.IV = new byte[16]; // Use o mesmo IV que foi usado durante a criptografia

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    byte[] encryptedBytes = Convert.FromBase64String(base64Input);
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private static string SafeBase64Encode(string input)
        {
            if(input != null)
            {
                return input.Replace("/", "_").Replace("+", "-").Replace("=", "");
            }
            else
            {
                return string.Empty;
            }
        }

        private static string SafeBase64Decode(string input)
        {
            if(input != null)
            {
                input = input.Replace("_", "/").Replace("-", "+");
                int padding = input.Length % 4;
                if (padding > 0)
                {
                    input += new string('=', 4 - padding);
                }
                return input;
            }

            return string.Empty;
        }
    }
}
