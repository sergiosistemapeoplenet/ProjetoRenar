using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.ViewModels.Usuarios
{
    public class ConsultaUsuarioViewModel
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
        public virtual ConsultaPerfilViewModel Perfil { get; set; }

        public short IDUnidade { get; set; }
        public string NomeUnidade { get; set; }
        public string CNPJ { get; set; }
        public short IDRegiao { get; set; }
        public string Endereco { get; set; }
        public string Cep { get; set; }
        public string HorarioFuncionamento { get; set; }
        public string QuantidadeUso { get; set; }
        public string SerialImpressora { get; set; }
        public string NomeContato { get; set; }
        public string NumeroContato { get; set; }
        public string EmailContato { get; set; }
    }
}
