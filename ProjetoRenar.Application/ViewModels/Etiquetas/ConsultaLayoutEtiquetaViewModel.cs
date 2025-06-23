using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.ViewModels.Etiquetas
{
    public class ConsultaLayoutEtiquetaViewModel
    {
        public short IDLayoutEtiqueta { get; set; }
        public string NomeLayoutEtiqueta { get; set; }
        public string CodLayoutEtiqueta { get; set; }
        public byte NumeroColunasImpressao { get; set; }
        public bool FlagAtivo { get; set; }
    }
}
