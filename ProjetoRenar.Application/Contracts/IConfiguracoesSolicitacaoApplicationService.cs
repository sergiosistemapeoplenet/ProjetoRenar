using PeopleNetRH.Application.ViewModels.Funcionarios;
using PeopleNetRH.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeopleNetRH.Application.Contracts
{
    public interface IConfiguracoesSolicitacaoApplicationService : IDisposable
    {
        void InserirConfiguracoesSolicitacao(ConfiguracoesSolicitacao configuracoes);
        void Alterar(CadastroConfiguracoesSolicitacaoViewModel model);
        void ExcluirById(int idTipoSolicitacao);
        List<ConfiguracaoSolicitacoesViewModel> ObterTodos();
        ConfiguracaoSolicitacoesViewModel ObterById(int idTipoSolicitacao);
    }
}
