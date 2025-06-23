using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.Exceptions.Usuarios
{
    public class EmailCadastradoException : Exception
    {
        public override string Message 
            => "Endereço de e-mail já cadastrado!";

    }
}
