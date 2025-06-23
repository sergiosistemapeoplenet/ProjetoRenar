using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Entities
{
    public class Usuario
    {
        public short IDUsuario { get; set; }
        public string SenhaUsuario { get; set; }
        public string EmailUsuario { get; set; }
        public DateTime? DataUltimoAcesso { get; set; }
        public bool FlagAtivo { get; set; }
        public bool FlagBloqueado { get; set; }
        public byte IDPerfil { get; set; }
        public short? IDUsuarioInclusao { get; set; }
        public DateTime? DataUsuarioInclusao { get; set; }
        public short? IDUsuarioAlteracao { get; set; }
        public DateTime? DataUsuarioAlteracao { get; set; }
        public bool? FlagPrimeiroAcesso { get; set; }
        public virtual Perfil Perfil { get; set; }
        public virtual Usuario UsuarioInclusao { get; set; }
        public virtual Usuario UsuarioAlteracao { get; set; }
    }
}
