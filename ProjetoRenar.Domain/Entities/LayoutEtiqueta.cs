using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Entities
{
    public class LayoutEtiqueta
    {
        public short IDLayoutEtiqueta { get; set; }
        public string NomeLayoutEtiqueta { get; set; }
        public string CodLayoutEtiqueta { get; set; }
        public byte NumeroColunasImpressao { get; set; }
        public bool FlagAtivo { get; set; }
    }
}
