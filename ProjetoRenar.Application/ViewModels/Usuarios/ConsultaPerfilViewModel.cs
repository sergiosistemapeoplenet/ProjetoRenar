using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.ViewModels.Usuarios
{
    public class ConsultaPerfilViewModel
    {
        public byte IDPerfil { get; set; }
        public string NomePerfil { get; set; }
        public short DiasExpiracaoSenha { get; set; }
        public byte ErrosSenha { get; set; }
        public bool FlagSituacao { get; set; }
        public bool? FlagUsuarioMaster { get; set; }
    }
}
