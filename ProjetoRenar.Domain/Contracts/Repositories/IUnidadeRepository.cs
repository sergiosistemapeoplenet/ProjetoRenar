using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Repositories
{
    public interface IUnidadeRepository
    {
        List<Unidade> GetAll();
        Unidade GetById(int id);
        void Insert(Unidade unidade);
        void Update(Unidade unidade);
        void Delete(short id);
        void UnDelete(short id);
    }
}
