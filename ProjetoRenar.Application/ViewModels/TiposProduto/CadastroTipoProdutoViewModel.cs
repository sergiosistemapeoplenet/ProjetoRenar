using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetoRenar.Application.ViewModels.Etiquetas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProjetoRenar.Application.ViewModels.TiposProduto
{
    public class CadastroTipoProdutoViewModel
    {
        public int IDTipoProduto { get; set; }

        [MaxLength(40, ErrorMessage = "Informe no máximo {1} caracteres.")]
        [Required(ErrorMessage = "Por favor, informe o nome do tipo de produto.")]
        public string NomeTipoProduto { get; set; }

        public bool? FlagAtivo { get; set; }

        [Required(ErrorMessage = "Por favor, selecione o layout da etiqueta.")]
        public short? IDLayoutEtiqueta { get; set; }

        public int? FlagAltoAcucar { get; set; }
        public int? FlagAltoGorduraSaturada { get; set; }

        public List<SelectListItem> LayoutsEtiqueta { get; set; }
    }
}
