using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Repositories
{
    public interface IClienteRepository
    {
        List<Cliente> GetAll();
        Cliente GetByName(string nomeCliente);
        void Insert(Cliente cliente);
        void Update(Cliente cliente);
        void Delete(string nomeCliente);
    }
}
