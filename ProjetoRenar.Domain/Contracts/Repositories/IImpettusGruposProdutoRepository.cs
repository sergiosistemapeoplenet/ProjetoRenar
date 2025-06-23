using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Repositories
{
    public interface IImpettusGruposProdutoRepository
    {
        List<ImpettusGruposProduto> GetAll();
        ImpettusGruposProduto GetById(int id);
        ImpettusGruposProduto GetByName(string nome);
        void Insert(ImpettusGruposProduto regiao);
        void Update(ImpettusGruposProduto regiao);
        void Delete(int id);
        void UnDelete(int id);
    }
}
