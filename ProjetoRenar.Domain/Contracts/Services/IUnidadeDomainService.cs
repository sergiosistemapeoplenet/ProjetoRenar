using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Services
{
    public interface IUnidadeDomainService
    {
        void Cadastrar(Unidade unidade);
        void Atualizar(Unidade unidade);
        void Excluir(int id);
        void Reativar(int id);

        Unidade Obter(int idUnidade);
        List<Unidade> Consultar();
    }
}
