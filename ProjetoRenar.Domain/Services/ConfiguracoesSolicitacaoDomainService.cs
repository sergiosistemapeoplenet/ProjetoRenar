using PeopleNetRH.Domain.Contracts.Repositories;
using PeopleNetRH.Domain.Contracts.Services;
using PeopleNetRH.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeopleNetRH.Domain.Services
{
    public class ConfiguracoesSolicitacaoDomainService : BaseDomainService<ConfiguracoesSolicitacao, int>, IConfiguracoesSolicitacaoDomainService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ConfiguracoesSolicitacaoDomainService(IUnitOfWork unitOfWork)
            : base(unitOfWork.ConfiguracoesSolicitacaoRepository)
        {
            _unitOfWork = unitOfWork;
        }

        public void InserirConfiguracoesSolicitacao(ConfiguracoesSolicitacao configuracoes)
        {
            _unitOfWork.ConfiguracoesSolicitacaoRepository.Inserir(configuracoes);
        }
    }
}
