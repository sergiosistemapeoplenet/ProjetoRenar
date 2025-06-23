using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Entities;

namespace ProjetoRenar.Infra.Repository
{
    public class ControleImpressaoRepository : IControleImpressaoRepository
    {
        private readonly SqlConnection _connection;

        public ControleImpressaoRepository(SqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public List<ControleImpressao> GetAll()
        {
            return _connection.Query<ControleImpressao>("SELECT * FROM Renar.ControleImpressao").ToList();
        }

        public ControleImpressao GetById(int id)
        {
            string sql = "SELECT * FROM Renar.ControleImpressao WHERE IDControleImpressao = @IDControleImpressao";
            return _connection.QueryFirstOrDefault<ControleImpressao>(sql, new { IDControleImpressao = id });
        }

        public void Insert(ControleImpressao controleImpressao)
        {
            string sql = @"INSERT INTO Renar.ControleImpressao (IDUnidade, IDProduto, QuantidadeEtiqueta, IDUsuario, DataInclusao) 
                           VALUES (@IDUnidade, @IDProduto, @QuantidadeEtiqueta, @IDUsuario, @DataInclusao)";
            _connection.Execute(sql, controleImpressao);
        }

        public void Update(ControleImpressao controleImpressao)
        {
            string sql = @"UPDATE Renar.ControleImpressao 
                           SET IDUnidade = @IDUnidade, IDProduto = @IDProduto, QuantidadeEtiqueta = @QuantidadeEtiqueta, 
                               IDUsuario = @IDUsuario, DataInclusao = @DataInclusao 
                           WHERE IDControleImpressao = @IDControleImpressao";
            _connection.Execute(sql, controleImpressao);
        }

        public void Delete(int id)
        {
            string sql = @"DELETE FROM Renar.ControleImpressao WHERE IDControleImpressao = @IDControleImpressao";
            _connection.Execute(sql, new { IDControleImpressao = id });
        }
    }
}
