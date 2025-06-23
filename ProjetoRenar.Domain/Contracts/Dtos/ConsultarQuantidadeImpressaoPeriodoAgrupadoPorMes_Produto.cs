using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Dtos
{
    public class ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Produto
    {
        public string NomeProduto { get; set; }
        public string MesAno { get; set; }
        public int Quantidade { get; set; }
    }
}
