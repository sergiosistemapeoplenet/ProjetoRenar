using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Repositories
{
    public interface IRegiaoRepository
    {
        List<Regiao> GetAll();
        Regiao GetById(int id);
        Regiao GetByName(string nome);
        void Insert(Regiao regiao);
        void Update(Regiao regiao);
        void Delete(short id);
        void UnDelete(short id);
    }
}
