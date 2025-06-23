using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Entities;

namespace ProjetoRenar.Infra.Repository
{
    public class PerfilRepository : IPerfilRepository
    {
        private readonly SqlConnection _connection;

        public PerfilRepository(SqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public List<Perfil> GetAll()
        {
            return _connection.Query<Perfil>("SELECT * FROM Acesso.Perfil").ToList();
        }

        public Perfil GetById(short id)
        {
            return _connection.QueryFirstOrDefault<Perfil>("SELECT * FROM Acesso.Perfil WHERE IDPerfil = @IDPerfil", new { IDPerfil = id });
        }

        public void Insert(Perfil perfil)
        {
            string sql = @"INSERT INTO Acesso.Perfil (NomePerfil, DiasExpiracaoSenha, ErrosSenha, FlagSituacao, FlagUsuarioMaster) 
                           VALUES (@NomePerfil, @DiasExpiracaoSenha, @ErrosSenha, @FlagSituacao, @FlagUsuarioMaster)";
            _connection.Execute(sql, perfil);
        }

        public void Update(Perfil perfil)
        {
            string sql = @"UPDATE Acesso.Perfil 
                           SET NomePerfil = @NomePerfil, DiasExpiracaoSenha = @DiasExpiracaoSenha, 
                               ErrosSenha = @ErrosSenha, FlagSituacao = @FlagSituacao, FlagUsuarioMaster = @FlagUsuarioMaster 
                           WHERE IDPerfil = @IDPerfil";
            _connection.Execute(sql, perfil);
        }

        public void Delete(byte id)
        {
            string sql = @"DELETE FROM Acesso.Perfil WHERE IDPerfil = @IDPerfil";
            _connection.Execute(sql, new { IDPerfil = id });
        }
    }
}