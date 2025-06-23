using ProjetoRenar.Application.ViewModels.Etiquetas;
using ProjetoRenar.Application.ViewModels.LayoutEtiquetas;
using ProjetoRenar.Application.ViewModels.Produtos;
using ProjetoRenar.Application.ViewModels.TiposProduto;
using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.Contracts
{
    public interface IProdutoApplicationService
    {
        void Cadastrar(CadastroProdutoViewModel model);
        void Atualizar(EdicaoProdutoViewModel model);
        void Excluir(int id);
        void Reativar(int id);

        List<ConsultaProdutoViewModel> Consultar(string nomeProduto, int flagAtivo);
        ConsultaProdutoViewModel Obter(int id);

        void IncluirControleImpressao(int idUnidade, int idProduto, int quantidadeEtiqueta, int idUsuario);
    }
}
