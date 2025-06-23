using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Services
{
    public interface ITipoProdutoDomainService
    {
        void Cadastrar(TipoProduto tipoProduto);
        void Atualizar(TipoProduto tipoProduto);
        void Excluir(int id);
        void Reativar(int id);

        List<TipoProduto> Consultar();
        List<TipoProduto> Consultar(string nomeTipoProduto, int flagAtivo);
        TipoProduto ObterPorId(int id);           
    }
}
