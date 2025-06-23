using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Entities;

namespace ProjetoRenar.Infra.Repository
{
    public class ImpettusGruposPreparacoesRepository : IImpettusGruposPreparacoesRepository
    {
        private readonly SqlConnection _connection;

        public ImpettusGruposPreparacoesRepository(SqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public List<ImpettusGruposPreparacoes> GetAll()
        {
            return _connection.Query<ImpettusGruposPreparacoes>("SELECT * FROM Impettus.GrupoPreparacao").ToList();
        }

        public ImpettusGruposPreparacoes GetById(int id)
        {
            string sql = "SELECT * FROM Impettus.GrupoPreparacao WHERE IDGrupoPreparacao = @IDGrupoPreparacao";
            return _connection.QueryFirstOrDefault<ImpettusGruposPreparacoes>(sql, new { IDGrupoPreparacao = id });
        }

        public ImpettusGruposPreparacoes GetByName(string nome)
        {
            string sql = "SELECT * FROM Impettus.GrupoPreparacao WHERE NomeGrupoPreparacao = @NomeGrupoPreparacao";
            return _connection.QueryFirstOrDefault<ImpettusGruposPreparacoes>(sql, new { NomeGrupoPreparacao = nome });
        }

        public void Insert(ImpettusGruposPreparacoes GrupoPreparacao)
        {
            string sql = @"INSERT INTO Impettus.GrupoPreparacao (NomeGrupoPreparacao, FlagSituacao) 
                           VALUES (@NomeGrupoPreparacao, @FlagSituacao)";
            _connection.Execute(sql, GrupoPreparacao);
        }

        public void Update(ImpettusGruposPreparacoes GrupoPreparacao)
        {
            string sql = @"UPDATE Impettus.GrupoPreparacao 
                           SET NomeGrupoPreparacao = @NomeGrupoPreparacao, FlagSituacao = @FlagSituacao
                           WHERE IDGrupoPreparacao = @IDGrupoPreparacao";
            _connection.Execute(sql, GrupoPreparacao);
        }

        public void Delete(int id)
        {
            string sql = @"UPDATE Impettus.GrupoPreparacao SET FlagSituacao = 0 WHERE IDGrupoPreparacao = @IDGrupoPreparacao";
            _connection.Execute(sql, new { IDGrupoPreparacao = id });
        }

        public void UnDelete(int id)
        {
            string sql = @"UPDATE Impettus.GrupoPreparacao SET FlagSituacao = 1 WHERE IDGrupoPreparacao = @IDGrupoPreparacao";
            _connection.Execute(sql, new { IDGrupoPreparacao = id });
        }
    }
}
