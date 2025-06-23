using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Entities
{
    public class ControleImpressao
    {
        public int IDControleImpressao { get; set; }
        public short IDUnidade { get; set; }
        public int IDProduto { get; set; }
        public short QuantidadeEtiqueta { get; set; }
        public short IDUsuario { get; set; }
        public DateTime DataInclusao { get; set; }
        public virtual Unidade Unidade { get; set; }
        public virtual Produto Produto { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
