using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Entities;

namespace ProjetoRenar.Infra.Repository
{
    public class UsuarioUnidadeRepository : IUsuarioUnidadeRepository
    {
        private readonly SqlConnection _connection;

        public UsuarioUnidadeRepository(SqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public List<UsuarioUnidade> GetAll()
        {
            return _connection.Query<UsuarioUnidade>("SELECT * FROM Acesso.UsuarioUnidade").ToList();
        }

        public List<UsuarioUnidade> GetByKey(int idUsuario)
        {
            string sql = "SELECT * FROM Acesso.UsuarioUnidade WHERE IDUsuario = @IDUsuario";
            return _connection.Query<UsuarioUnidade>(sql, new { IDUsuario = idUsuario }).ToList();
        }

        public void Insert(UsuarioUnidade usuarioUnidade)
        {
            string sql = @"INSERT INTO Acesso.UsuarioUnidade (IDUsuario, IDUnidade, FlagUnidadePadrao) VALUES (@IDUsuario, @IDUnidade, @FlagUnidadePadrao)";
            _connection.Execute(sql, usuarioUnidade);
        }

        public void Delete(short idUsuario, short idUnidade)
        {
            string sql = @"DELETE FROM Acesso.UsuarioUnidade WHERE IDUsuario = @IDUsuario AND IDUnidade = @IDUnidade";
            _connection.Execute(sql, new { IDUsuario = idUsuario, IDUnidade = idUnidade });
        }

        public void Delete(short idUsuario)
        {
            string sql = @"DELETE FROM Acesso.UsuarioUnidade WHERE IDUsuario = @IDUsuario";
            _connection.Execute(sql, new { IDUsuario = idUsuario });
        }
    }
}
