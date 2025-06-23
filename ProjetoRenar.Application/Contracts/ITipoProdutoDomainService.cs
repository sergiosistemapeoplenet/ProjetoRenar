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
    public interface ITipoProdutoApplicationService
    {
        void Cadastrar(CadastroTipoProdutoViewModel model);
        void Atualizar(EdicaoTipoProdutoViewModel model);
        void Excluir(int id);
        void Reativar(int id);

        List<ConsultaTipoProdutoViewModel> Consultar(string nomeTipoProduto, int flagAtivo);
        ConsultaTipoProdutoViewModel Obter(int id);
    }
}
