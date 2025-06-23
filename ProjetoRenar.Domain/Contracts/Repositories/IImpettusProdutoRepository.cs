using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Repositories
{
    public interface IImpettusProdutoRepository
    {
        List<ImpettusProduto> GetAll();
        ImpettusProduto GetById(int id);
        ImpettusProduto GetByName(string nome);
        void Insert(ImpettusProduto produto);
        void Update(ImpettusProduto produto);
        void Delete(int id);
        void UnDelete(int id);
    }
}
