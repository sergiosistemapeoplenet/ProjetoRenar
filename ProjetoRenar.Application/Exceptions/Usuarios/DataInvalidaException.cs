using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.Exceptions.Usuarios
{
    public class DataInvalidaException : Exception
    {
        private string message;

        public DataInvalidaException()
        {

        }

        public DataInvalidaException(string message)
        {
            this.message = message;
        }

        public override string Message
            => string.IsNullOrEmpty(message) ? "Data inválida!" : message;
    }
}
