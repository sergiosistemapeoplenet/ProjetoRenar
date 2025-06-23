using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Entities;

namespace ProjetoRenar.Infra.Repository
{
    public class LayoutEtiquetaRepository : ILayoutEtiquetaRepository
    {
        private readonly SqlConnection _connection;

        public LayoutEtiquetaRepository(SqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public List<LayoutEtiqueta> GetAll()
        {
            return _connection.Query<LayoutEtiqueta>("SELECT * FROM Renar.LayoutEtiqueta").ToList();
        }

        public List<LayoutEtiqueta> GetAll(string nomeLayoutEtiqueta, int flagAtivo)
        {
            return _connection.Query<LayoutEtiqueta>(@"
                SELECT * FROM Renar.LayoutEtiqueta
                WHERE nomeLayoutEtiqueta LIKE @NomeLayoutEtiqueta AND FlagAtivo = @FlagAtivo 
                ORDER BY NomeLayoutEtiqueta
            ", new 
            {
                @NomeLayoutEtiqueta = $"%{nomeLayoutEtiqueta}%",
                @FlagAtivo = flagAtivo
            })
            .ToList();
        }

        public LayoutEtiqueta GetById(int id)
        {
            string sql = "SELECT * FROM Renar.LayoutEtiqueta WHERE IDLayoutEtiqueta = @IDLayoutEtiqueta";
            return _connection.QueryFirstOrDefault<LayoutEtiqueta>(sql, new { IDLayoutEtiqueta = id });
        }

        public LayoutEtiqueta GetByCodLayout(string codLayout)
        {
            string sql = "SELECT * FROM Renar.LayoutEtiqueta WHERE CodLayoutEtiqueta = @CodLayoutEtiqueta";
            return _connection.QueryFirstOrDefault<LayoutEtiqueta>(sql, new { CodLayoutEtiqueta = codLayout });
        }

        public void Insert(LayoutEtiqueta layoutEtiqueta)
        {
            string sql = @"INSERT INTO Renar.LayoutEtiqueta (NomeLayoutEtiqueta, CodLayoutEtiqueta, NumeroColunasImpressao, FlagAtivo) 
                           VALUES (@NomeLayoutEtiqueta, @CodLayoutEtiqueta, @NumeroColunasImpressao, @FlagAtivo)";
            _connection.Execute(sql, layoutEtiqueta);
        }

        public void Update(LayoutEtiqueta layoutEtiqueta)
        {
            string sql = @"UPDATE Renar.LayoutEtiqueta 
                           SET NomeLayoutEtiqueta = @NomeLayoutEtiqueta, CodLayoutEtiqueta = @CodLayoutEtiqueta, 
                               NumeroColunasImpressao = @NumeroColunasImpressao, FlagAtivo = @FlagAtivo 
                           WHERE IDLayoutEtiqueta = @IDLayoutEtiqueta";
            _connection.Execute(sql, layoutEtiqueta);
        }

        public void Delete(short id)
        {
            string sql = @"DELETE FROM Renar.LayoutEtiqueta WHERE IDLayoutEtiqueta = @IDLayoutEtiqueta";
            _connection.Execute(sql, new { IDLayoutEtiqueta = id });
        }
    }
}
