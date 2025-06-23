using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Repositories
{
    public interface IPerfilOperacaoBloqueioRepository
    {
        List<PerfilOperacaoBloqueio> GetAll();
        PerfilOperacaoBloqueio GetByKey(byte idPerfil, int idOperacaoBloqueio);
        void Insert(PerfilOperacaoBloqueio perfilOperacaoBloqueio);
        void Delete(byte idPerfil, int idOperacaoBloqueio);
    }
}
