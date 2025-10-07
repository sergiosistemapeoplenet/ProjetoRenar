using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Repositories
{
    public interface IImpettusControleImpressaoRepository
    {
        List<ImpettusControleImpressao> GetAll();
        ImpettusControleImpressao GetById(int id);
        void Insert(ImpettusControleImpressao controleImpressao);
        void Update(ImpettusControleImpressao controleImpressao);
        void Delete(int id);
    }
}
