using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.Exceptions.Usuarios
{
    public class DataExpiracaoContaException : Exception
    {
        public override string Message
            => "Informe a data de expiração da conta.";
    }
}
