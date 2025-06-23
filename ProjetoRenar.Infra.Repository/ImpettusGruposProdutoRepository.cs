using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Entities;

namespace ProjetoRenar.Infra.Repository
{
    public class ImpettusGruposProdutoRepository : IImpettusGruposProdutoRepository
    {
        private readonly SqlConnection _connection;

        public ImpettusGruposProdutoRepository(SqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public List<ImpettusGruposProduto> GetAll()
        {
            return _connection.Query<ImpettusGruposProduto>("SELECT * FROM Impettus.GrupoProduto").ToList();
        }

        public ImpettusGruposProduto GetById(int id)
        {
            string sql = "SELECT * FROM Impettus.GrupoProduto WHERE IDGrupoProduto = @IDGrupoProduto";
            return _connection.QueryFirstOrDefault<ImpettusGruposProduto>(sql, new { IDGrupoProduto = id });
        }

        public ImpettusGruposProduto GetByName(string nome)
        {
            string sql = "SELECT * FROM Impettus.GrupoProduto WHERE NomeGrupoProduto = @NomeGrupoProduto";
            return _connection.QueryFirstOrDefault<ImpettusGruposProduto>(sql, new { NomeGrupoProduto = nome });
        }

        public void Insert(ImpettusGruposProduto GrupoProduto)
        {
            string sql = @"INSERT INTO Impettus.GrupoProduto (NomeGrupoProduto, FlagSituacao) 
                           VALUES (@NomeGrupoProduto, @FlagSituacao)";
            _connection.Execute(sql, GrupoProduto);
        }

        public void Update(ImpettusGruposProduto GrupoProduto)
        {
            string sql = @"UPDATE Impettus.GrupoProduto 
                           SET NomeGrupoProduto = @NomeGrupoProduto, FlagSituacao = @FlagSituacao
                           WHERE IDGrupoProduto = @IDGrupoProduto";
            _connection.Execute(sql, GrupoProduto);
        }

        public void Delete(int id)
        {
            string sql = @"UPDATE Impettus.GrupoProduto SET FlagSituacao = 0 WHERE IDGrupoProduto = @IDGrupoProduto";
            _connection.Execute(sql, new { IDGrupoProduto = id });
        }

        public void UnDelete(int id)
        {
            string sql = @"UPDATE Impettus.GrupoProduto SET FlagSituacao = 1 WHERE IDGrupoProduto = @IDGrupoProduto";
            _connection.Execute(sql, new { IDGrupoProduto = id });
        }
    }
}
