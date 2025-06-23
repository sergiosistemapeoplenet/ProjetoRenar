using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.Exceptions.Usuarios
{
    public class UsuarioBloqueadoException : Exception
    {
        public override string Message => $"<strong>Usuário Bloqueado!</strong> Entre em contato com o Administrador do Sistema.";
    }
}
