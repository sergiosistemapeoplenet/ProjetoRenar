using Dapper;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace ProjetoRenar.Infra.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly SqlConnection _connection;

        public UsuarioRepository(SqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public List<Usuario> GetAll()
        {
            return _connection.Query<Usuario>("SELECT * FROM Acesso.Usuario").ToList();
        }

        public Usuario GetById(int id)
        {
            return _connection.QueryFirstOrDefault<Usuario>("SELECT * FROM Acesso.Usuario WHERE IDUsuario = @IDUsuario", new { IDUsuario = id });
        }

        public void Insert(Usuario usuario, int idUnidade)
        {
            usuario.FlagPrimeiroAcesso = true;

            string sqlUsuario = @"
                INSERT INTO Acesso.Usuario (SenhaUsuario, EmailUsuario, DataUltimoAcesso, FlagAtivo, FlagBloqueado, IDPerfil, IDUsuarioInclusao, DataUsuarioInclusao, IDUsuarioAlteracao, DataUsuarioAlteracao, FlagPrimeiroAcesso) 
                VALUES (@SenhaUsuario, @EmailUsuario, @DataUltimoAcesso, @FlagAtivo, @FlagBloqueado, @IDPerfil, @IDUsuarioInclusao, @DataUsuarioInclusao, @IDUsuarioAlteracao, @DataUsuarioAlteracao, @FlagPrimeiroAcesso) 
                SELECT CAST(SCOPE_IDENTITY() AS INT)";

            var id = _connection.QuerySingle<int>(sqlUsuario, usuario);

            string sqlUnidade = @"INSERT INTO Acesso.UsuarioUnidade (IDUsuario, IDUnidade, FlagUnidadePadrao) VALUES (@IDUsuario, @IDUnidade, @FlagUnidadePadrao)";

            _connection.Execute(sqlUnidade, new { @IDUsuario = id, @IDUnidade = idUnidade, @FlagUnidadePadrao = 1 });
        }

        public void Update(Usuario usuario)
        {
            string sql = @"UPDATE Acesso.Usuario 
                           SET EmailUsuario = @EmailUsuario, DataUltimoAcesso = @DataUltimoAcesso, 
                               FlagAtivo = @FlagAtivo, FlagBloqueado = @FlagBloqueado, IDPerfil = @IDPerfil, IDUsuarioInclusao = @IDUsuarioInclusao, 
                               DataUsuarioInclusao = @DataUsuarioInclusao, IDUsuarioAlteracao = @IDUsuarioAlteracao, DataUsuarioAlteracao = @DataUsuarioAlteracao, 
                               FlagPrimeiroAcesso = @FlagPrimeiroAcesso 
                           WHERE IDUsuario = @IDUsuario";
            _connection.Execute(sql, usuario);
        }


        public void Delete(short id)
        {
            string sql = @"UPDATE Acesso.Usuario SET FLAGATIVO = 0 WHERE IDUsuario = @IDUsuario";
            _connection.Execute(sql, new { IDUsuario = id });
        }

        public void UnDelete(short id)
        {
            string sql = @"UPDATE Acesso.Usuario SET FLAGATIVO = 1 WHERE IDUsuario = @IDUsuario";
            _connection.Execute(sql, new { IDUsuario = id });
        }

        public Usuario Get(string email)
        {
            var query = "select * from Acesso.Usuario where EmailUsuario = @Email and FlagAtivo = 1";
            return _connection.QueryFirstOrDefault<Usuario>(query, new { email });
        }

        public Usuario Get(string email, string senha)
        {
            var query = "select * from Acesso.Usuario where EmailUsuario = @Email and SenhaUsuario = @Senha and FlagAtivo = 1";
            return _connection.QueryFirstOrDefault<Usuario>(query, new { email, senha });
        }

        public void RedefinirSenha(int? idUsuario, string senhaAtual, string novaSenha)
        {
            var consulta = "select * from Acesso.Usuario where IDUsuario = @idUsuario and SenhaUsuario = @senhaAtual";
            var dados = _connection.QueryFirstOrDefault<Usuario>(consulta, new { idUsuario, senhaAtual });
            if (dados == null)
                throw new Exception("Senha atual inválida. Verifique e tente novamente.");

            var query = "update Acesso.Usuario set FlagPrimeiroAcesso = 0, SenhaUsuario = @novaSenha where IDUsuario = @idUsuario and SenhaUsuario = @senhaAtual";
            _connection.Execute(query, new { idUsuario, senhaAtual, novaSenha });
        }

        public string GravarTokenRecuperacaoDeSenha(int? idUsuario)
        {
            var token = Guid.NewGuid().ToString();
            var dataAtual = DateTime.Now.AddMinutes(30);

            string sql = @"UPDATE Acesso.Usuario SET TokenRecuperacaoSenha = @token, DataHoraRecuperacaoSenha = @dataAtual WHERE IDUsuario = @IDUsuario";
            _connection.Execute(sql, new { IDUsuario = idUsuario, @token = token, @dataAtual = dataAtual });

            return token;
        }

        public void ApagarTokenRecuperacaoDeSenha(int? idUsuario)
        {
            string sql = @"UPDATE Acesso.Usuario SET TokenRecuperacaoSenha = NULL, DataHoraRecuperacaoSenha = NULL WHERE IDUsuario = @IDUsuario";
            _connection.Execute(sql, new { IDUsuario = idUsuario });
        }

        public Usuario ObterTokenRecuperacaoDeSenha(string token)
        {
            var query = "select * from Acesso.Usuario where TokenRecuperacaoSenha = @token and DataHoraRecuperacaoSenha >= GETDATE()";
            return _connection.QueryFirstOrDefault<Usuario>(query, new { @token = token });
        }

        public void AtualizarSenhaUsuario(string senha, int idUsuario)
        {
            string sql = @"UPDATE Acesso.Usuario SET SenhaUsuario = @senha WHERE IDUsuario = @IDUsuario";
            _connection.Execute(sql, new { @senha = senha, IDUsuario = idUsuario });
        }

        public void DefinirPrimeiroAcesso(int idUsuario)
        {
            string sql = @"UPDATE Acesso.Usuario SET FlagPrimeiroAcesso = 1 WHERE IDUsuario = @IDUsuario";
            _connection.Execute(sql, new { IDUsuario = idUsuario });
        }
    }
}
