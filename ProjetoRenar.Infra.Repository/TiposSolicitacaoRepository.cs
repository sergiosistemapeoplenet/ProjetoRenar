using Dapper;
using PeopleNetRH.Domain.Contracts.Repositories;
using PeopleNetRH.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace PeopleNetRH.Infra.Repository
{
    public class TiposSolicitacaoRepository : ITipoSolicitacaoRepository
    {
        private readonly SqlConnection connection;

        public TiposSolicitacaoRepository(SqlConnection connection)
        {
            this.connection = connection;
        }

        public void Alterar(TipoSolicitacao obj)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            connection.Dispose();
        }

        public void Excluir(int obj)
        {
            throw new NotImplementedException();
        }

        public void Inserir(TipoSolicitacao obj)
        {
            var query = "insert into PeopleNetRH.TipoSolicitacao (IdTipoSolicitacao, Nome) " +
                        "values (@IdTipoSolicitacao, @Nome)";

            connection.Execute(query, obj);
        }

        public TipoSolicitacao ObterPorId(int id)
        {
            throw new NotImplementedException();
        }

        public List<TipoSolicitacao> ObterTodos()
        {
            throw new NotImplementedException();
        }
    }
}
