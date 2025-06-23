using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Entities;

namespace ProjetoRenar.Infra.Repository
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly SqlConnection _connection;

        public ProdutoRepository(SqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public List<Produto> GetAll()
        {
            return _connection.Query<Produto>("SELECT * FROM Renar.Produto").ToList();
        }

        public List<Produto> GetAll(string NomeProduto, int FlagAtivo)
        {
            return _connection.Query<Produto>(@"
                    SELECT * FROM Renar.Produto
                    WHERE NomeProduto LIKE @NomeProduto AND FlagAtivo = @FlagAtivo
                ", new 
                {
                    NomeProduto = $"%{NomeProduto}%",
                    FlagAtivo
                })
                .ToList();
        }

        public Produto GetById(int id)
        {
            string sql = "SELECT * FROM Renar.Produto WHERE IDProduto = @IDProduto";
            return _connection.QueryFirstOrDefault<Produto>(sql, new { IDProduto = id });
        }

        public Produto GetByCodigoBarra(string codigoBarra)
        {
            string sql = "SELECT * FROM Renar.Produto WHERE CodigoBarra = @CodigoBarra";
            return _connection.QueryFirstOrDefault<Produto>(sql, new { CodigoBarra = codigoBarra });
        }

        public void Insert(Produto produto)
        {
            string sql = @"INSERT INTO Renar.Produto (NomeProduto, CodigoBarra, Peso, FlagFatiacopo, ValorEnergetico,
                                                      PorcaoValorEnergetico, ValorEnergeticoValorDiario, Carboidratos,
                                                      PorcaoCarboidratos, CarboidratosValorDiario, AcucarTotal,
                                                      PorcaoAcucarTotal, AcucarTotalValorDiario, AcucarAdicionado,
                                                      PorcaoAcucarAdicionado, AcucarAdicionadoValorDiario, Proteina,
                                                      PorcaoProteina, ProteinaValorDiario, GorduraTotal, PorcaoGorduraTotal,
                                                      Gtotvd, GorduraSaturada, PorcaoGorduraSaturada, GorduraSaturadaValorDiario,
                                                      GorduraTrans, PorcaoGorduraTrans, GorduraTransValorDiario, FibraAlimentar,
                                                      PorcaoFibraAlimentar, FibraAlimentarValorDiario, Sodio, PorcaoSodio,
                                                      SodioValorDiario, Receita, Info1, Info2, Info3, PorcaoEmbalagem, PorcaoFatia,
                                                      IDTipoProduto, FlagAtivo, DiasValidade, Link) 
                           VALUES (@NomeProduto, @CodigoBarra, @Peso, @FlagFatiacopo, @ValorEnergetico, @PorcaoValorEnergetico,
                                   @ValorEnergeticoValorDiario, @Carboidratos, @PorcaoCarboidratos, @CarboidratosValorDiario,
                                   @AcucarTotal, @PorcaoAcucarTotal, @AcucarTotalValorDiario, @AcucarAdicionado, @PorcaoAcucarAdicionado,
                                   @AcucarAdicionadoValorDiario, @Proteina, @PorcaoProteina, @ProteinaValorDiario, @GorduraTotal,
                                   @PorcaoGorduraTotal, @Gtotvd, @GorduraSaturada, @PorcaoGorduraSaturada, @GorduraSaturadaValorDiario,
                                   @GorduraTrans, @PorcaoGorduraTrans, @GorduraTransValorDiario, @FibraAlimentar, @PorcaoFibraAlimentar,
                                   @FibraAlimentarValorDiario, @Sodio, @PorcaoSodio, @SodioValorDiario, @Receita, @Info1, @Info2, @Info3,
                                   @PorcaoEmbalagem, @PorcaoFatia, @IDTipoProduto, @FlagAtivo, @DiasValidade, @Link)";
            _connection.Execute(sql, produto);
        }

        public void Update(Produto produto)
        {
            string sql = @"UPDATE Renar.Produto 
                           SET NomeProduto = @NomeProduto, CodigoBarra = @CodigoBarra, Peso = @Peso, FlagFatiacopo = @FlagFatiacopo,
                               ValorEnergetico = @ValorEnergetico, PorcaoValorEnergetico = @PorcaoValorEnergetico,
                               ValorEnergeticoValorDiario = @ValorEnergeticoValorDiario, Carboidratos = @Carboidratos,
                               PorcaoCarboidratos = @PorcaoCarboidratos, CarboidratosValorDiario = @CarboidratosValorDiario,
                               AcucarTotal = @AcucarTotal, PorcaoAcucarTotal = @PorcaoAcucarTotal, AcucarTotalValorDiario = @AcucarTotalValorDiario,
                               AcucarAdicionado = @AcucarAdicionado, PorcaoAcucarAdicionado = @PorcaoAcucarAdicionado,
                               AcucarAdicionadoValorDiario = @AcucarAdicionadoValorDiario, Proteina = @Proteina,
                               PorcaoProteina = @PorcaoProteina, ProteinaValorDiario = @ProteinaValorDiario, GorduraTotal = @GorduraTotal,
                               PorcaoGorduraTotal = @PorcaoGorduraTotal, Gtotvd = @Gtotvd, GorduraSaturada = @GorduraSaturada,
                               PorcaoGorduraSaturada = @PorcaoGorduraSaturada, GorduraSaturadaValorDiario = @GorduraSaturadaValorDiario,
                               GorduraTrans = @GorduraTrans, PorcaoGorduraTrans = @PorcaoGorduraTrans, GorduraTransValorDiario = @GorduraTransValorDiario,
                               FibraAlimentar = @FibraAlimentar, PorcaoFibraAlimentar = @PorcaoFibraAlimentar,
                               FibraAlimentarValorDiario = @FibraAlimentarValorDiario, Sodio = @Sodio, PorcaoSodio = @PorcaoSodio,
                               SodioValorDiario = @SodioValorDiario, Receita = @Receita, Info1 = @Info1, Info2 = @Info2, Info3 = @Info3,
                               PorcaoEmbalagem = @PorcaoEmbalagem, PorcaoFatia = @PorcaoFatia, IDTipoProduto = @IDTipoProduto,
                               FlagAtivo = @FlagAtivo, DiasValidade = @DiasValidade, Link = @Link
                           WHERE IDProduto = @IDProduto";
            _connection.Execute(sql, produto);
        }

        public void Delete(int id)
        {
            string sql = @"UPDATE Renar.Produto SET FLAGATIVO = 0 WHERE IDProduto = @IDProduto";
            _connection.Execute(sql, new { IDProduto = id });
        }

        public void UnDelete(int id)
        {
            string sql = @"UPDATE Renar.Produto SET FLAGATIVO = 1 WHERE IDProduto = @IDProduto";
            _connection.Execute(sql, new { IDProduto = id });
        }

        public List<Produto> GetByTipo(int idTipo)
        {
            string sql = "SELECT * FROM Renar.Produto WHERE IDTipoProduto = @idTipo";
            return _connection.Query<Produto>(sql, new { @idTipo = idTipo }).ToList();
        }

        public void IncluirControleImpressao(int idUnidade, int idProduto, int quantidadeEtiqueta, int idUsuario)
        {
            _connection.Execute
               ("Renar.SP_IncluirControleImpressao", new
               {
                   IDUnidade = idUnidade,
                   IDProduto = idProduto,
                   QuantidadeEtiqueta = quantidadeEtiqueta,
                   IDUsuario = idUsuario
               }, commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
