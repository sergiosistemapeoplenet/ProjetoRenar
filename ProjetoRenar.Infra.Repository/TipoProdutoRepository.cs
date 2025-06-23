using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Entities;

namespace ProjetoRenar.Infra.Repository
{
    public class TipoProdutoRepository : ITipoProdutoRepository
    {
        private readonly SqlConnection _connection;

        public TipoProdutoRepository(SqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public List<TipoProduto> GetAll()
        {
            return _connection.Query<TipoProduto>("SELECT * FROM Renar.TipoProduto").ToList();
        }

        public List<TipoProduto> GetAll(string nomeTipoProduto, int flagAtivo)
        {
            return _connection.Query<TipoProduto>(@"
                SELECT 
                    IDTipoProduto,
                    NomeTipoProduto,
                    FlagAtivo,
                    IDLayoutEtiqueta,
                    CAST(FlagAltoAcucar AS INT) AS FlagAltoAcucar, 
                    CAST(FlagAltoGorduraSaturada AS INT) AS FlagAltoGorduraSaturada
                FROM Renar.TipoProduto
                WHERE NomeTipoProduto LIKE @NomeTipoProduto AND FlagAtivo = @FlagAtivo
                ORDER BY NomeTipoProduto
            ", new
                {
                    @NomeTipoProduto = $"%{nomeTipoProduto}%",
                    @FlagAtivo = flagAtivo
                })
                .ToList();
        }


        public TipoProduto GetById(int id)
        {
            string sql = @"
                SELECT 
                    IDTipoProduto,
                    NomeTipoProduto,
                    FlagAtivo,
                    IDLayoutEtiqueta,
                    CAST(FlagAltoAcucar AS INT) AS FlagAltoAcucar, 
                    CAST(FlagAltoGorduraSaturada AS INT) AS FlagAltoGorduraSaturada
                FROM Renar.TipoProduto
                WHERE IDTipoProduto = @IDTipoProduto";

            return _connection.QueryFirstOrDefault<TipoProduto>(sql, new { IDTipoProduto = id });
        }


        public void Insert(TipoProduto tipoProduto)
        {
            string sql = @"INSERT INTO Renar.TipoProduto (NomeTipoProduto, FlagAtivo, IDLayoutEtiqueta, FlagAltoAcucar, FlagAltoGorduraSaturada) 
                           VALUES (@NomeTipoProduto, @FlagAtivo, @IDLayoutEtiqueta, @FlagAltoAcucar, @FlagAltoGorduraSaturada)";
            _connection.Execute(sql, new
            {
                NomeTipoProduto = tipoProduto.NomeTipoProduto,
                FlagAtivo = tipoProduto.FlagAtivo,
                IDLayoutEtiqueta = tipoProduto.IDLayoutEtiqueta,
                FlagAltoAcucar = tipoProduto.FlagAltoAcucar != null ? tipoProduto.FlagAltoAcucar : 0,
                FlagAltoGorduraSaturada = tipoProduto.FlagAltoGorduraSaturada != null ? tipoProduto.FlagAltoGorduraSaturada : 0
            });
        }

        public void Update(TipoProduto tipoProduto)
        {
            string sql = @"UPDATE Renar.TipoProduto 
                           SET NomeTipoProduto = @NomeTipoProduto, FlagAtivo = @FlagAtivo, IDLayoutEtiqueta = @IDLayoutEtiqueta, FlagAltoAcucar = @FlagAltoAcucar, FlagAltoGorduraSaturada = @FlagAltoGorduraSaturada
                           WHERE IDTipoProduto = @IDTipoProduto";
            _connection.Execute(sql, new
            {
                IDTipoProduto = tipoProduto.IDTipoProduto,
                NomeTipoProduto = tipoProduto.NomeTipoProduto,
                FlagAtivo = tipoProduto.FlagAtivo,
                IDLayoutEtiqueta = tipoProduto.IDLayoutEtiqueta,
                FlagAltoAcucar = tipoProduto.FlagAltoAcucar != null ? tipoProduto.FlagAltoAcucar : 0,
                FlagAltoGorduraSaturada = tipoProduto.FlagAltoGorduraSaturada != null ? tipoProduto.FlagAltoGorduraSaturada : 0
            });
        }

        public void Delete(int id)
        {
            string sql = @"UPDATE Renar.TipoProduto SET FLAGATIVO = 0 WHERE IDTipoProduto = @IDTipoProduto";
            _connection.Execute(sql, new { IDTipoProduto = id });
        }

        public void UnDelete(int id)
        {
            string sql = @"UPDATE Renar.TipoProduto SET FLAGATIVO = 1 WHERE IDTipoProduto = @IDTipoProduto";
            _connection.Execute(sql, new { IDTipoProduto = id });
        }
    }
}
