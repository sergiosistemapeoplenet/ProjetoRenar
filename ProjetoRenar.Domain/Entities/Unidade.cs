using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Entities
{
    public class Unidade
    {
        public short IDUnidade { get; set; }
        public string NomeUnidade { get; set; }
        public string RazaoSocial { get; set; }
        public string CNPJ { get; set; }
        public short IDRegiao { get; set; }
        public string Endereco { get; set; }
        public bool FlagAtivo { get; set; }
        public string Cep { get; set; }
        public string HorarioFuncionamento { get; set; }
        public string QuantidadeUso { get; set; }
        public string SerialImpressora { get; set; }
        public string NomeContato { get; set; }
        public string NumeroContato { get; set; }
        public string EmailContato { get; set; }
        public bool FlagImprimeCodigoBarra { get; set; }
        public virtual Regiao Regiao { get; set; }
    }
}
