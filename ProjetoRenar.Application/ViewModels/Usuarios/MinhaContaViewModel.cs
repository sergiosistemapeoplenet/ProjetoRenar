using ProjetoRenar.Application.ViewModels.Unidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.ViewModels.Usuarios
{
    public class MinhaContaViewModel
    {
        public virtual int IdUsuario { get; set; }
        public virtual string SenhaUsuario { get; set; }
        public virtual string EmailUsuario { get; set; }
        public virtual DateTime DataUltimoAcesso { get; set; }
        public virtual int FlagAtivo { get; set; }
        public virtual int FlagBloqueado { get; set; }
        public virtual int IDPerfil { get; set; }
        public virtual int IDUsuarioInclusao { get; set; }
        public virtual DateTime DataUsuarioInclusao { get; set; }
        public virtual int IDUsuarioAlteracao { get; set; }
        public bool? FlagPrimeiroAcesso { get; set; }
        public virtual DateTime DataUsuarioAlteracao { get; set; }
        public virtual List<ConsultaUnidadeViewModel> Unidades { get; set; }
    }
}
