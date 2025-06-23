using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Cryptographies
{
    public interface IMD5Cryptography
    {
        string Encrypt(string value);
    }
}
