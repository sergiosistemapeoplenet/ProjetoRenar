using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Entities
{
    public class TipoProduto
    {
        public int IDTipoProduto { get; set; }
        public string NomeTipoProduto { get; set; }
        public bool? FlagAtivo { get; set; }
        public short? IDLayoutEtiqueta { get; set; }
        public virtual LayoutEtiqueta LayoutEtiqueta { get; set; }

        public int? FlagAltoAcucar { get; set; }
        public int? FlagAltoGorduraSaturada { get; set; }
    }
}
