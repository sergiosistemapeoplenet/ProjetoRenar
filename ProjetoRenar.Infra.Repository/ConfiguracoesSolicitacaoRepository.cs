using Dapper;
using PeopleNetRH.Domain.Contracts.Repositories;
using PeopleNetRH.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace PeopleNetRH.Infra.Repository
{
    public class ConfiguracoesSolicitacaoRepository : IConfiguracoesSolicitacaoRepository
    {
        private readonly SqlConnection _connection;

        public ConfiguracoesSolicitacaoRepository(SqlConnection connection)
        {
            _connection = connection;
        }

        public void Alterar(ConfiguracoesSolicitacao obj)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Excluir(int obj)
        {
            throw new NotImplementedException();
        }

        public void Inserir(ConfiguracoesSolicitacao obj)
        {
            using (SqlCommand cmd = new SqlCommand("PeopleNetRH.ConfiguracoesSolicitacaoFuncionario", _connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdConfiguracoesSolicitacaoFuncionario", obj.IdConfiguracoesSolicitacaoFuncionario);
                cmd.Parameters.AddWithValue("IdEmpresa", obj.IdEmpresa);
                cmd.Parameters.AddWithValue("IdFuncionario", obj.IdFuncionario);
                cmd.ExecuteNonQuery();
            }
        }

        public ConfiguracoesSolicitacao ObterPorId(int id)
        {
            throw new NotImplementedException();
        }

        public List<ConfiguracoesSolicitacao> ObterTodos()
        {
            throw new NotImplementedException();
        }
    }
}
