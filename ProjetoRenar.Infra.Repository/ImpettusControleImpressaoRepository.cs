using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Entities;

namespace ProjetoRenar.Infra.Repository
{
    public class ImpettusControleImpressaoRepository : IImpettusControleImpressaoRepository
    {
        private readonly SqlConnection _connection;

        public ImpettusControleImpressaoRepository(SqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public List<ImpettusControleImpressao> GetAll()
        {
            return _connection.Query<ImpettusControleImpressao>("SELECT * FROM Impettus.ControleImpressao").ToList();
        }

        public ImpettusControleImpressao GetById(int id)
        {
            string sql = "SELECT * FROM Impettus.ControleImpressao WHERE IDControleImpressao = @IDControleImpressao";
            return _connection.QueryFirstOrDefault<ImpettusControleImpressao>(sql, new { IDControleImpressao = id });
        }

        public void Insert(ImpettusControleImpressao controleImpressao)
        {
            string sql = @"INSERT INTO Impettus.ControleImpressao (IDUnidade, IDProduto, QuantidadeEtiqueta, IDUsuario, DataInclusao) 
                           VALUES (@IDUnidade, @IDProduto, @QuantidadeEtiqueta, @IDUsuario, @DataInclusao)";
            _connection.Execute(sql, controleImpressao);
        }

        public void Update(ImpettusControleImpressao controleImpressao)
        {
            string sql = @"UPDATE Impettus.ControleImpressao 
                           SET IDUnidade = @IDUnidade, IDProduto = @IDProduto, QuantidadeEtiqueta = @QuantidadeEtiqueta, 
                               IDUsuario = @IDUsuario, DataInclusao = @DataInclusao 
                           WHERE IDControleImpressao = @IDControleImpressao";
            _connection.Execute(sql, controleImpressao);
        }

        public void Delete(int id)
        {
            string sql = @"DELETE FROM Impettus.ControleImpressao WHERE IDControleImpressao = @IDControleImpressao";
            _connection.Execute(sql, new { IDControleImpressao = id });
        }
    }
}
