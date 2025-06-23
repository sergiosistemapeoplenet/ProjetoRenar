using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Services
{
    public interface IProdutoDomainService
    {
        void Cadastrar(Produto produto);
        void Atualizar(Produto produto);
        void Excluir(int id);
        void Reativar(int id);

        List<Produto> Consultar();
        List<Produto> Consultar(string nomeProduto, int flagAtivo);
        List<Produto> ObterPorTipo(int idTipoProduto);
        Produto ObterPorId(int id);

        void IncluirControleImpressao(int idUnidade, int idProduto, int quantidadeEtiqueta, int idUsuario);
    }
}
