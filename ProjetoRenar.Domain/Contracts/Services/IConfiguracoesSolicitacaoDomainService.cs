using PeopleNetRH.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeopleNetRH.Domain.Contracts.Services
{
    public interface IConfiguracoesSolicitacaoDomainService : IBaseDomainService<ConfiguracoesSolicitacao, int>
    {
        void InserirConfiguracoesSolicitacao(ConfiguracoesSolicitacao configuracoes);
    }
}
