using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Repositories
{
    public interface IVersaoRepository
    {
        List<Versao> GetAll();
        Versao GetByBuild(string build);
        void Insert(Versao versao);
        void Update(Versao versao);
        void Delete(string build);
    }
}
