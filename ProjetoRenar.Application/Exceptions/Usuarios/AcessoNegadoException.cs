using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.Exceptions.Usuarios
{
    public class AcessoNegadoException : Exception
    {
        public override string Message
            => $"<strong>Acesso Negado</strong>. Usuário inválido.";
    }

    public class AcessoNegadoSituacaoException : Exception
    {
        public override string Message
            => $"<strong>Acesso Negado</strong>. Verifique a situação do funcionário.";
    }
}
