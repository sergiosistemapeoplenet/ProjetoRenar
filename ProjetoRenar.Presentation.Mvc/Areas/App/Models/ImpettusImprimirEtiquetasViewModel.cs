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

        public bool CheckFavorito { get; set; } = false;

        public List<ImpettusProdutoModel> ListagemProdutos { get; set; }
    }

    public class ImpettusProdutoModel
    {
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public bool FlagResfriado { get; set; }
        public string TipoValidadeResfriado { get; set; }
        public int? ValidadeResfriado { get; set; }
        public bool FlagCongelado { get; set; }
        public int? ValidadeCongelado { get; set; }
        public bool FlagTemperaturaAmbiente { get; set; }
        public string TipoValidadeTemperaturaAmbiente { get; set; }
        public int? ValidadeTemperaturaAmbiente { get; set; }
        public int? IdGrupoProduto { get; set; }
        public string NomeGrupoProduto { get; set; }
        public bool FlagAtivo { get; set; }
        public string Sif { get; set; }
        public string DataValidadeCongelado { get; set; }
        public string DataValidadeResfriado { get; set; }
        public string DataValidadeTemperaturaAmbiente { get; set; }
        public string Tipo { get; set; }
        public string PreparacaoProduto { get; set; }
        public int FlagFavorito { get; set; }
    }
}
