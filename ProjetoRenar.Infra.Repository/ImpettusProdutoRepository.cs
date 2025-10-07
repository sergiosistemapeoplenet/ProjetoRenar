using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Entities;

namespace ProjetoRenar.Infra.Repository
{
    public class ImpettusProdutosRepository : IImpettusProdutoRepository
    {
        private readonly SqlConnection _connection;

        public ImpettusProdutosRepository(SqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public List<ImpettusProduto> GetAll()
        {
            return _connection.Query<ImpettusProduto>(
                "SELECT * FROM Impettus.Produto"
            ).ToList();
        }

        public ImpettusProduto GetById(int id)
        {
            var sql = "SELECT * FROM Impettus.Produto WHERE IDProduto = @IDProduto";
            return _connection.QueryFirstOrDefault<ImpettusProduto>(sql, new { IDProduto = id });
        }

        public ImpettusProduto GetByName(string nome)
        {
            var sql = "SELECT * FROM Impettus.Produto WHERE NomeProduto = @NomeProduto";
            return _connection.QueryFirstOrDefault<ImpettusProduto>(sql, new { NomeProduto = nome });
        }

        public void Insert(ImpettusProduto produto)
        {
            var sql = @"
                INSERT INTO Impettus.Produto (
                    NomeProduto,
                    FlagResfriado,
                    TipoValidadeResfriado,
                    ValidadeResfriado,
                    FlagCongelado,
                    ValidadeCongelado,
                    FlagTemperaturaAmbiente,
                    TipoValidadeTemperaturaAmbiente,
                    ValidadeTemperaturaAmbiente,
                    IDGrupoProduto,
                    FlagAtivo,
                    Sif,
                    FlagFavorito
                )
                VALUES (
                    @NomeProduto,
                    @FlagResfriado,
                    @TipoValidadeResfriado,
                    @ValidadeResfriado,
                    @FlagCongelado,
                    @ValidadeCongelado,
                    @FlagTemperaturaAmbiente,
                    @TipoValidadeTemperaturaAmbiente,
                    @ValidadeTemperaturaAmbiente,
                    @IDGrupoProduto,
                    @FlagAtivo,
                    @Sif,
                    @FlagFavorito
                )";
            _connection.Execute(sql, produto);
        }

        public void Update(ImpettusProduto produto)
        {
            var sql = @"
                UPDATE Impettus.Produto
                SET 
                    NomeProduto = @NomeProduto,
                    FlagResfriado = @FlagResfriado,
                    TipoValidadeResfriado = @TipoValidadeResfriado,
                    ValidadeResfriado = @ValidadeResfriado,
                    FlagCongelado = @FlagCongelado,
                    ValidadeCongelado = @ValidadeCongelado,
                    FlagTemperaturaAmbiente = @FlagTemperaturaAmbiente,
                    TipoValidadeTemperaturaAmbiente = @TipoValidadeTemperaturaAmbiente,
                    ValidadeTemperaturaAmbiente = @ValidadeTemperaturaAmbiente,
                    IDGrupoProduto = @IDGrupoProduto,
                    FlagAtivo = @FlagAtivo,
                    Sif = @Sif,
                    FlagFavorito = @FlagFavorito
                WHERE IDProduto = @IDProduto";
            _connection.Execute(sql, produto);
        }

        public void Delete(int id)
        {
            var sql = "UPDATE Impettus.Produto SET FlagAtivo = 0 WHERE IDProduto = @IDProduto";
            _connection.Execute(sql, new { IDProduto = id });
        }

        public void UnDelete(int id)
        {
            var sql = "UPDATE Impettus.Produto SET FlagAtivo = 1 WHERE IDProduto = @IDProduto";
            _connection.Execute(sql, new { IDProduto = id });
        }

        public void AdicionarControleEtiqueta(DateTime dataImpressao, string conteudoEtiqueta)
        {
            var sql = @"
                INSERT INTO Impettus.ControleEtiqueta (ConteudoEtiqueta, DataImpressao)
                VALUES (@ConteudoEtiqueta, @DataImpressao)";

            _connection.Execute(sql, new
            {
                ConteudoEtiqueta = conteudoEtiqueta,
                DataImpressao = dataImpressao
            });
        }


        public void BaixarControleEtiqueta(int id)
        {
            var sql = "UPDATE Impettus.ControleEtiqueta SET FlagAtivo = 0 WHERE Id = @Id";
            _connection.Execute(sql, new { Id = id });
        }

        public List<ControleEtiqueta> ListarControleEtiqueta()
        {
            var sql = "SELECT * FROM Impettus.ControleEtiqueta WHERE FlagAtivo = 1";
            return _connection.Query<ControleEtiqueta>(sql).ToList();
        }

        public void IncluirControleImpressao(int idUnidade, int idProduto, int idPreparacao, int quantidadeEtiqueta, int idUsuario)
        {
            if(idProduto != 0)
            {
                _connection.Execute
                  ("Impettus.SP_IncluirControleImpressao", new
                  {
                      IDUnidade = idUnidade,
                      IDProduto = idProduto,
                      IDPreparacao = (int?) null,
                      QuantidadeEtiqueta = quantidadeEtiqueta,
                      IDUsuario = idUsuario
                  }, commandType: System.Data.CommandType.StoredProcedure);
            }
            if(idPreparacao != 0)
            {
                _connection.Execute
                  ("Impettus.SP_IncluirControleImpressao", new
                  {
                      IDUnidade = idUnidade,
                      IDProduto = (int?)null,
                      IDPreparacao = idPreparacao,
                      QuantidadeEtiqueta = quantidadeEtiqueta,
                      IDUsuario = idUsuario
                  }, commandType: System.Data.CommandType.StoredProcedure);
            }
        }
    }
}
