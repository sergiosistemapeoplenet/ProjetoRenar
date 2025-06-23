using ProjetoRenar.Application.ViewModels.Etiquetas;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.ViewModels.TiposProduto
{
    public class ConsultaTipoProdutoViewModel
    {
        public byte IDTipoProduto { get; set; }
        public string NomeTipoProduto { get; set; }
        public bool? FlagAtivo { get; set; }
        public short? IDLayoutEtiqueta { get; set; }
        public int? FlagAltoAcucar { get; set; }
        public int? FlagAltoGorduraSaturada { get; set; }
        public virtual ConsultaLayoutEtiquetaViewModel LayoutEtiqueta { get; set; }
    }
}
