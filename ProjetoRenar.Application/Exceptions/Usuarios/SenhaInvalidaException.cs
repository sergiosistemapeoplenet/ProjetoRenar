using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.Exceptions.Usuarios
{
    public class SenhaInvalidaException : Exception
    {
        int? qtdErros = null;

        public SenhaInvalidaException()
        {

        }

        public SenhaInvalidaException(int qtdErros)
        {
            this.qtdErros = qtdErros;
        }

        public override string Message
        {
            get
            {
                if (qtdErros != null)
                {
                    return "Senha de acesso inválida. Você tem mais " + qtdErros + " tentativa(s) até o seu usuário ser bloqueado.";
                }
                else
                {
                    return "Credenciais não conferem ou usuário não tem acesso ao sistema.";
                }
            }
        }

        //public override string Message
        //    => $"<strong>Acesso Negado</strong>. Senha do usuário inválida.";
    }
}