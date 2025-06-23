using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Dtos
{
    public class ConsultarQuantidadeMediaImpressaoUltimosMeses_TipoDeProduto
    {
        public string NomeTipoProduto { get; set; }
        public decimal Media { get; set; }
    }
}
