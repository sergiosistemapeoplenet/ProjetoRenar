using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Entities;

namespace ProjetoRenar.Infra.Repository
{
    public class PerfilOperacaoBloqueioRepository : IPerfilOperacaoBloqueioRepository
    {
        private readonly SqlConnection _connection;

        public PerfilOperacaoBloqueioRepository(SqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public List<PerfilOperacaoBloqueio> GetAll()
        {
            return _connection.Query<PerfilOperacaoBloqueio>("SELECT * FROM Acesso.PerfilOperacaoBloqueio").ToList();
        }

        public PerfilOperacaoBloqueio GetByKey(byte idPerfil, int idOperacaoBloqueio)
        {
            string sql = "SELECT * FROM Acesso.PerfilOperacaoBloqueio WHERE IDPerfil = @IDPerfil AND IDOperacaoBloqueio = @IDOperacaoBloqueio";
            return _connection.QueryFirstOrDefault<PerfilOperacaoBloqueio>(sql, new { IDPerfil = idPerfil, IDOperacaoBloqueio = idOperacaoBloqueio });
        }

        public void Insert(PerfilOperacaoBloqueio perfilOperacaoBloqueio)
        {
            string sql = @"INSERT INTO Acesso.PerfilOperacaoBloqueio (IDPerfil, IDOperacaoBloqueio) VALUES (@IDPerfil, @IDOperacaoBloqueio)";
            _connection.Execute(sql, perfilOperacaoBloqueio);
        }

        public void Delete(byte idPerfil, int idOperacaoBloqueio)
        {
            string sql = @"DELETE FROM Acesso.PerfilOperacaoBloqueio WHERE IDPerfil = @IDPerfil AND IDOperacaoBloqueio = @IDOperacaoBloqueio";
            _connection.Execute(sql, new { IDPerfil = idPerfil, IDOperacaoBloqueio = idOperacaoBloqueio });
        }
    }
}
