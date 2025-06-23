using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Repositories
{
    public interface IControleImpressaoRepository
    {
        List<ControleImpressao> GetAll();
        ControleImpressao GetById(int id);
        void Insert(ControleImpressao controleImpressao);
        void Update(ControleImpressao controleImpressao);
        void Delete(int id);
    }
}
