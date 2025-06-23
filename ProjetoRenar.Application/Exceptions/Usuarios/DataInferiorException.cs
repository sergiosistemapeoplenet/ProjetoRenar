using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.Exceptions.Usuarios
{
    public class DataInferiorException : Exception
    {
        public override string Message
            => "A data de expiração da conta não pode ser inferior a data de expiração de senha.";
    }
}
