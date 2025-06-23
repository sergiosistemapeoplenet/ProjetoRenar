using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Entities;

namespace ProjetoRenar.Infra.Repository
{
    public class RegiaoRepository : IRegiaoRepository
    {
        private readonly SqlConnection _connection;

        public RegiaoRepository(SqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public List<Regiao> GetAll()
        {
            return _connection.Query<Regiao>("SELECT * FROM Renar.Regiao").ToList();
        }

        public Regiao GetById(int id)
        {
            string sql = "SELECT * FROM Renar.Regiao WHERE IDRegiao = @IDRegiao";
            return _connection.QueryFirstOrDefault<Regiao>(sql, new { IDRegiao = id });
        }

        public Regiao GetByName(string nome)
        {
            string sql = "SELECT * FROM Renar.Regiao WHERE NomeRegiao = @NomeRegiao";
            return _connection.QueryFirstOrDefault<Regiao>(sql, new { NomeRegiao = nome });
        }

        public void Insert(Regiao regiao)
        {
            string sql = @"INSERT INTO Renar.Regiao (NomeRegiao, FlagAtivo) 
                           VALUES (@NomeRegiao, @FlagAtivo)";
            _connection.Execute(sql, regiao);
        }

        public void Update(Regiao regiao)
        {
            string sql = @"UPDATE Renar.Regiao 
                           SET NomeRegiao = @NomeRegiao, FlagAtivo = @FlagAtivo
                           WHERE IDRegiao = @IDRegiao";
            _connection.Execute(sql, regiao);
        }

        public void Delete(short id)
        {
            string sql = @"UPDATE Renar.Regiao SET FLAGATIVO = 0 WHERE IDRegiao = @IDRegiao";
            _connection.Execute(sql, new { IDRegiao = id });
        }

        public void UnDelete(short id)
        {
            string sql = @"UPDATE Renar.Regiao SET FLAGATIVO = 1 WHERE IDRegiao = @IDRegiao";
            _connection.Execute(sql, new { IDRegiao = id });
        }
    }
}
