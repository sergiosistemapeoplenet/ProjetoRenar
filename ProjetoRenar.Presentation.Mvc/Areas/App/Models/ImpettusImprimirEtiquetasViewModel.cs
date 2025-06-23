using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetoRenar.Application.ViewModels.Etiquetas;
using ProjetoRenar.Application.ViewModels.Produtos;
using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;

namespace ProjetoRenar.Presentation.Mvc.Areas.App.Models
{
    public class ImpettusImprimirEtiquetasViewModel
    {
        public int? IdTipoProduto { get; set; }
        public string NomeProduto { get; set; }

        public int[] IdsProdutos { get; set; }
        public int[] QtdProdutos { get; set; }

        public bool CheckProdutos { get; set; } = true;
        public bool CheckPreparacoes { get; set; } = true;

        public string Tipo { get; set; }

        public List<ImpettusProdutoModel> ListagemProdutos { get; set; }
        public List<SelectListItem> ListagemTipos { get; set; }
    }

    public class ImpettusProdutoModel
    {
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; }
        public string TipoProduto { get; set; }
        public bool FlagProduto { get; set; }
        public bool FlagPreparacao { get; set; }
        public string DataAtual { get; set; } = string.Empty;
        public string DataValidade { get; set; } = string.Empty;
        public string Tipo { get; set; }
    }
}
