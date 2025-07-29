using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Entities;

namespace ProjetoRenar.Infra.Repository
{
    public class ImpettusPreparacoesRepository : IImpettusPreparacaoRepository
    {
        private readonly SqlConnection _connection;

        public ImpettusPreparacoesRepository(SqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public List<ImpettusPreparacao> GetAll()
        {
            return _connection.Query<ImpettusPreparacao>(
                "SELECT * FROM Impettus.Preparacao"
            ).ToList();
        }

        public ImpettusPreparacao GetById(int id)
        {
            var sql = "SELECT * FROM Impettus.Preparacao WHERE IDPreparacao = @IDPreparacao";
            return _connection.QueryFirstOrDefault<ImpettusPreparacao>(sql, new { IDPreparacao = id });
        }

        public ImpettusPreparacao GetByName(string nome)
        {
            var sql = "SELECT * FROM Impettus.Preparacao WHERE NomePreparacao = @NomePreparacao";
            return _connection.QueryFirstOrDefault<ImpettusPreparacao>(sql, new { NomePreparacao = nome });
        }

        public void Insert(ImpettusPreparacao preparacao)
        {
            var sql = @"
                INSERT INTO Impettus.Preparacao (
                    NomePreparacao,
                    FlagResfriado,
                    TipoValidadeResfriado,
                    ValidadeResfriado,
                    FlagCongelado,
                    ValidadeCongelado,
                    FlagTemperaturaAmbiente,
                    TipoValidadeTemperaturaAmbiente,
                    ValidadeTemperaturaAmbiente,
                    IDGrupoPreparacao,
                    FlagAtivo,
                    Sif,
                    FlagFavorito
                )
                VALUES (
                    @NomePreparacao,
                    @FlagResfriado,
                    @TipoValidadeResfriado,
                    @ValidadeResfriado,
                    @FlagCongelado,
                    @ValidadeCongelado,
                    @FlagTemperaturaAmbiente,
                    @TipoValidadeTemperaturaAmbiente,
                    @ValidadeTemperaturaAmbiente,
                    @IDGrupoPreparacao,
                    @FlagAtivo,
                    @Sif,
                    @FlagFavorito
                )";
            _connection.Execute(sql, preparacao);
        }

        public void Update(ImpettusPreparacao preparacao)
        {
            var sql = @"
                UPDATE Impettus.Preparacao
                SET 
                    NomePreparacao = @NomePreparacao,
                    FlagResfriado = @FlagResfriado,
                    TipoValidadeResfriado = @TipoValidadeResfriado,
                    ValidadeResfriado = @ValidadeResfriado,
                    FlagCongelado = @FlagCongelado,
                    ValidadeCongelado = @ValidadeCongelado,
                    FlagTemperaturaAmbiente = @FlagTemperaturaAmbiente,
                    TipoValidadeTemperaturaAmbiente = @TipoValidadeTemperaturaAmbiente,
                    ValidadeTemperaturaAmbiente = @ValidadeTemperaturaAmbiente,
                    IDGrupoPreparacao = @IDGrupoPreparacao,
                    FlagAtivo = @FlagAtivo,
                    Sif = @Sif,
                    FlagFavorito = @FlagFavorito
                WHERE IDPreparacao = @IDPreparacao";
            _connection.Execute(sql, preparacao);
        }

        public void Delete(int id)
        {
            var sql = "UPDATE Impettus.Preparacao SET FlagAtivo = 0 WHERE IDPreparacao = @IDPreparacao";
            _connection.Execute(sql, new { IDPreparacao = id });
        }

        public void UnDelete(int id)
        {
            var sql = "UPDATE Impettus.Preparacao SET FlagAtivo = 1 WHERE IDPreparacao = @IDPreparacao";
            _connection.Execute(sql, new { IDPreparacao = id });
        }
    }
}
