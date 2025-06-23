using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Entities;

namespace ProjetoRenar.Infra.Repository
{
    public class VersaoRepository : IVersaoRepository
    {
        private readonly SqlConnection _connection;

        public VersaoRepository(SqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public List<Versao> GetAll()
        {
            return _connection.Query<Versao>("SELECT * FROM Configuracao.Versao").ToList();
        }

        public Versao GetByBuild(string build)
        {
            string sql = "SELECT * FROM Configuracao.Versao WHERE Build = @Build";
            return _connection.QueryFirstOrDefault<Versao>(sql, new { Build = build });
        }

        public void Insert(Versao versao)
        {
            string sql = @"INSERT INTO Configuracao.Versao (Build, DataVersao) VALUES (@Build, @DataVersao)";
            _connection.Execute(sql, versao);
        }

        public void Update(Versao versao)
        {
            string sql = @"UPDATE Configuracao.Versao SET DataVersao = @DataVersao WHERE Build = @Build";
            _connection.Execute(sql, versao);
        }

        public void Delete(string build)
        {
            string sql = @"DELETE FROM Configuracao.Versao WHERE Build = @Build";
            _connection.Execute(sql, new { Build = build });
        }
    }
}
