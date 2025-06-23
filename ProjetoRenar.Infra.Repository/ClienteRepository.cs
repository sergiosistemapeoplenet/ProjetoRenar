using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Entities;

namespace ProjetoRenar.Infra.Repository
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly SqlConnection _connection;

        public ClienteRepository(SqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public List<Cliente> GetAll()
        {
            return _connection.Query<Cliente>("SELECT * FROM Configuracao.Cliente").ToList();
        }

        public Cliente GetByName(string nomeCliente)
        {
            string sql = "SELECT * FROM Configuracao.Cliente WHERE NomeCliente = @NomeCliente";
            return _connection.QueryFirstOrDefault<Cliente>(sql, new { NomeCliente = nomeCliente });
        }

        public void Insert(Cliente cliente)
        {
            string sql = @"INSERT INTO Configuracao.Cliente (NomeCliente, Cnpj, Endereco, Bairro, Cidade, UF, Cep, Telefone, NumeroUsuarios) 
                           VALUES (@NomeCliente, @Cnpj, @Endereco, @Bairro, @Cidade, @UF, @Cep, @Telefone, @NumeroUsuarios)";
            _connection.Execute(sql, cliente);
        }

        public void Update(Cliente cliente)
        {
            string sql = @"UPDATE Configuracao.Cliente 
                           SET Cnpj = @Cnpj, Endereco = @Endereco, Bairro = @Bairro, Cidade = @Cidade, 
                               UF = @UF, Cep = @Cep, Telefone = @Telefone, NumeroUsuarios = @NumeroUsuarios 
                           WHERE NomeCliente = @NomeCliente";
            _connection.Execute(sql, cliente);
        }

        public void Delete(string nomeCliente)
        {
            string sql = @"DELETE FROM Configuracao.Cliente WHERE NomeCliente = @NomeCliente";
            _connection.Execute(sql, new { NomeCliente = nomeCliente });
        }
    }
}
