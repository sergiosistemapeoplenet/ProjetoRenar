using ProjetoRenar.Domain.Contracts.Cryptographies;
using System;
using System.Security.Cryptography;
using System.Text;

namespace ProjetoRenar.Infra.Cryptography
{
    public class MD5Cryptography : IMD5Cryptography
    {
        public string Encrypt(string value)
        {
            var md5 = new MD5CryptoServiceProvider();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(value));

            var result = new StringBuilder();

            foreach (var item in hash)
                result.Append(item.ToString("X2"));

            return result.ToString().ToUpper();
        }
    }
}
