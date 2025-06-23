using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetoRenar.Application.ViewModels.Produtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProjetoRenar.Application.ViewModels.Etiquetas
{
    public class ImprimirEtiquetasViewModel
    {        
        public int? IdTipoProduto { get; set; }
        public string NomeProduto { get; set; }

        public int[] IdsProdutos { get; set; }
        public int[] QtdProdutos { get; set; }

        public List<ConsultaProdutoViewModel> ListagemProdutos { get; set; }
        public List<SelectListItem> ListagemTiposDeProduto { get; set; }
        public ConsultaLayoutEtiquetaViewModel LayoutEtiqueta { get; set; }
    }
}
