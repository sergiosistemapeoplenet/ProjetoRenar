using ProjetoRenar.Application.ViewModels.Etiquetas;
using ProjetoRenar.Application.ViewModels.Produtos;
using ProjetoRenar.Application.ViewModels.TiposProduto;
using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.Contracts
{
    public interface IEtiquetasApplicationService
    {
        List<ConsultaTipoProdutoViewModel> ObterTiposDeProduto();
        List<ConsultaProdutoViewModel> ObterProdutos(int idTipoProduto);
        ConsultaLayoutEtiquetaViewModel ObterLayoutEtiqueta(int idLayoutEtiqueta);
        ConsultaProdutoViewModel ObterProduto(int idProduto);
    }
}
