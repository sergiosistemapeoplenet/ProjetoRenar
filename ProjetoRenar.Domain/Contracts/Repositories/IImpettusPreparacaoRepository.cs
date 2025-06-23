using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Repositories
{
    public interface IImpettusPreparacaoRepository
    {
        List<ImpettusPreparacao> GetAll();
        ImpettusPreparacao GetById(int id);
        ImpettusPreparacao GetByName(string nome);
        void Insert(ImpettusPreparacao preparacao);
        void Update(ImpettusPreparacao preparacao);
        void Delete(int id);
        void UnDelete(int id);
    }
}
