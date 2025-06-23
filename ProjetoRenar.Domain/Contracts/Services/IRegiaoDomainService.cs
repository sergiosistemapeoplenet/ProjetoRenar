using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Services
{
    public interface IRegiaoDomainService
    {
        void Cadastrar(Regiao regiao);
        void Atualizar(Regiao regiao);
        void Excluir(int id);
        void Reativar(int id);

        Regiao Obter(int idRegiao);
        List<Regiao> Consultar();
    }
}
