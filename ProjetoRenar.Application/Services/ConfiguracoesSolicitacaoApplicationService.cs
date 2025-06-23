using AutoMapper;
using PeopleNetRH.Application.Contracts;
using PeopleNetRH.Application.ViewModels.Funcionarios;
using PeopleNetRH.Domain.Contracts.Services;
using PeopleNetRH.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeopleNetRH.Application.Services
{
    public class ConfiguracoesSolicitacaoApplicationService : IConfiguracoesSolicitacaoApplicationService
    {
        private readonly IConfiguracoesSolicitacaoDomainService _domainService;

        public ConfiguracoesSolicitacaoApplicationService(IConfiguracoesSolicitacaoDomainService domainService)
        {
            _domainService = domainService;
        }

        public void Alterar(CadastroConfiguracoesSolicitacaoViewModel model)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void ExcluirById(int idTipoSolicitacao)
        {
            throw new NotImplementedException();
        }

        public void InserirConfiguracoesSolicitacao(ConfiguracoesSolicitacao configuracoes)
        {
            throw new NotImplementedException();
        }

        public ConfiguracaoSolicitacoesViewModel ObterById(int idTipoSolicitacao)
        {
            throw new NotImplementedException();
        }

        public List<ConfiguracaoSolicitacoesViewModel> ObterTodos()
        {
            throw new NotImplementedException();
        }
    }
}
