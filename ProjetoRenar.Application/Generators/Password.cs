using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ProjetoRenar.Application.Generators
{
    public static class Password
    {
        public static string CreatePassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%&";
            const string specialChars = "!@#$%&";
            using (var rng = new RNGCryptoServiceProvider())
            {
                var result = new char[length];
                var bytes = new byte[length];

                rng.GetBytes(bytes);

                // Pelo menos uma letra maiúscula
                result[0] = validChars[bytes[0] % 26];

                // Pelo menos uma letra minúscula
                result[1] = validChars[26 + bytes[1] % 26];

                // Pelo menos um número
                result[2] = validChars[52 + bytes[2] % 10];

                // Pelo menos dois caracteres especiais
                result[3] = specialChars[bytes[3] % specialChars.Length];
                result[4] = specialChars[bytes[4] % specialChars.Length];

                for (int i = 5; i < length; i++)
                {
                    result[i] = validChars[bytes[i] % validChars.Length];
                }

                // Embaralhe os caracteres na senha
                result = result.OrderBy(c => Guid.NewGuid()).ToArray();

                return new string(result);
            }
        }
    }
}
