using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.Exceptions.Usuarios
{
    public class EmailNaoEncontradoException : Exception
    {
        private readonly string email;

        public EmailNaoEncontradoException(string email)
        {
            this.email = email;
        }

        public override string Message 
            => "Credenciais não conferem ou usuário não tem acesso ao sistema";

        //public override string Message 
        //    => $"O email <strong>'{email}'</strong> não foi encontrado.";
    }
}
