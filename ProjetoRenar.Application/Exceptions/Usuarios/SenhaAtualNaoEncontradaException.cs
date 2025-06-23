using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.Exceptions.Usuarios
{
    public class SenhaAtualNaoEncontradaException : Exception
    {
        public override string Message 
            => "A senha atual informada está errada. Por favor, tente novamente.";
    }
}
