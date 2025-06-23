using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Entities
{
    public class Perfil
    {
        public byte IDPerfil { get; set; }
        public string NomePerfil { get; set; }
        public short DiasExpiracaoSenha { get; set; }
        public byte ErrosSenha { get; set; }
        public bool FlagSituacao { get; set; }
        public bool? FlagUsuarioMaster { get; set; }
    }
}
