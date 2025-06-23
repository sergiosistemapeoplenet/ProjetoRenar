using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Entities;

namespace ProjetoRenar.Infra.Repository
{
    public class OperacaoBloqueioRepository : IOperacaoBloqueioRepository
    {
        private readonly SqlConnection _connection;

        public OperacaoBloqueioRepository(SqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public List<OperacaoBloqueio> GetAll()
        {
            return _connection.Query<OperacaoBloqueio>("SELECT * FROM Acesso.OperacaoBloqueio").ToList();
        }

        public OperacaoBloqueio GetById(int id)
        {
            return _connection.QueryFirstOrDefault<OperacaoBloqueio>("SELECT * FROM Acesso.OperacaoBloqueio WHERE IDOperacaoBloqueio = @IDOperacaoBloqueio", new { IDOperacaoBloqueio = id });
        }

        public void Insert(OperacaoBloqueio operacaoBloqueio)
        {
            string sql = @"INSERT INTO Acesso.OperacaoBloqueio (NomeOperacaoBloqueio, URL) VALUES (@NomeOperacaoBloqueio, @URL)";
            _connection.Execute(sql, operacaoBloqueio);
        }

        public void Update(OperacaoBloqueio operacaoBloqueio)
        {
            string sql = @"UPDATE Acesso.OperacaoBloqueio SET NomeOperacaoBloqueio = @NomeOperacaoBloqueio, URL = @URL WHERE IDOperacaoBloqueio = @IDOperacaoBloqueio";
            _connection.Execute(sql, operacaoBloqueio);
        }

        public void Delete(int id)
        {
            string sql = @"DELETE FROM Acesso.OperacaoBloqueio WHERE IDOperacaoBloqueio = @IDOperacaoBloqueio";
            _connection.Execute(sql, new { IDOperacaoBloqueio = id });
        }
    }
}