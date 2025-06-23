using ProjetoRenar.Application.ViewModels.Regioes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.Contracts
{
    public interface IRegiaoApplicationService
    {
        void Cadastrar(CadastroRegiaoViewModel model);
        void Atualizar(EdicaoRegiaoViewModel model);
        void Excluir(int id);
        void Reativar(int id);

        List<ConsultaRegiaoViewModel> Consultar();
        ConsultaRegiaoViewModel Obter(int id);
    }
}
