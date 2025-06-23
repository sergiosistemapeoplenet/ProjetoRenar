using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Repositories
{
    public interface IImpettusGruposPreparacoesRepository
    {
        List<ImpettusGruposPreparacoes> GetAll();
        ImpettusGruposPreparacoes GetById(int id);
        ImpettusGruposPreparacoes GetByName(string nome);
        void Insert(ImpettusGruposPreparacoes regiao);
        void Update(ImpettusGruposPreparacoes regiao);
        void Delete(int id);
        void UnDelete(int id);
    }
}
