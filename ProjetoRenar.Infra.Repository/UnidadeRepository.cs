using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Entities;

namespace ProjetoRenar.Infra.Repository
{
    public class UnidadeRepository : IUnidadeRepository
    {
        private readonly SqlConnection _connection;

        public UnidadeRepository(SqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public List<Unidade> GetAll()
        {
            return _connection.Query<Unidade>("SELECT * FROM Renar.Unidade").ToList();
        }

        public Unidade GetById(int id)
        {
            string sql = "SELECT * FROM Renar.Unidade WHERE IDUnidade = @IDUnidade";
            return _connection.QueryFirstOrDefault<Unidade>(sql, new { IDUnidade = id });
        }

        public void Insert(Unidade unidade)
        {
            string sql = @"INSERT INTO Renar.Unidade (NomeUnidade, CNPJ, IDRegiao, Endereco, FlagAtivo, Cep, HorarioFuncionamento, 
                           QuantidadeUso, SerialImpressora, NomeContato, NumeroContato, EmailContato, FlagImprimeCodigoBarra, RazaoSocial) 
                           VALUES (@NomeUnidade, @CNPJ, @IDRegiao, @Endereco, @FlagAtivo, @Cep, @HorarioFuncionamento, @QuantidadeUso, 
                           @SerialImpressora, @NomeContato, @NumeroContato, @EmailContato, @FlagImprimeCodigoBarra, @RazaoSocial)";
            _connection.Execute(sql, unidade);
        }

        public void Update(Unidade unidade)
        {
            string sql = @"UPDATE Renar.Unidade 
                           SET NomeUnidade = @NomeUnidade, CNPJ = @CNPJ, IDRegiao = @IDRegiao, Endereco = @Endereco, FlagAtivo = @FlagAtivo, 
                           Cep = @Cep, HorarioFuncionamento = @HorarioFuncionamento, QuantidadeUso = @QuantidadeUso, SerialImpressora = @SerialImpressora, 
                           NomeContato = @NomeContato, NumeroContato = @NumeroContato, EmailContato = @EmailContato, FlagImprimeCodigoBarra = @FlagImprimeCodigoBarra, RazaoSocial = @RazaoSocial  
                           WHERE IDUnidade = @IDUnidade";
            _connection.Execute(sql, unidade);
        }

        public void Delete(short id)
        {
            string sql = @"UPDATE Renar.Unidade SET FLAGATIVO = 0 WHERE IDUnidade = @IDUnidade";
            _connection.Execute(sql, new { IDUnidade = id });
        }

        public void UnDelete(short id)
        {
            string sql = @"UPDATE Renar.Unidade SET FLAGATIVO = 1 WHERE IDUnidade = @IDUnidade";
            _connection.Execute(sql, new { IDUnidade = id });
        }
    }
}
