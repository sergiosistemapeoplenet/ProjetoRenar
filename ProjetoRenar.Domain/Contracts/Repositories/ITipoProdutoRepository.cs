using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Repositories
{
    public interface ITipoProdutoRepository
    {
        List<TipoProduto> GetAll();
        List<TipoProduto> GetAll(string nomeTipoProduto, int flagAtivo);
        TipoProduto GetById(int id);
        void Insert(TipoProduto tipoProduto);
        void Update(TipoProduto tipoProduto);
        void Delete(int id);
        void UnDelete(int id);
    }
}
