using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Repositories
{
    public interface IOperacaoBloqueioRepository
    {
        List<OperacaoBloqueio> GetAll();
        OperacaoBloqueio GetById(int id);
        void Insert(OperacaoBloqueio operacaoBloqueio);
        void Update(OperacaoBloqueio operacaoBloqueio);
        void Delete(int id);
    }
}
