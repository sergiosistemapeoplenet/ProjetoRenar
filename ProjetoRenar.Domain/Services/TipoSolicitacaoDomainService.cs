using PeopleNetRH.Domain.Contracts.Repositories;
using PeopleNetRH.Domain.Contracts.Services;
using PeopleNetRH.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeopleNetRH.Domain.Services
{
    public class TipoSolicitacaoDomainService : BaseDomainService<TipoSolicitacao, int>, ITipoSolicitacaoDomainService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TipoSolicitacaoDomainService(IUnitOfWork unitOfWork)
            : base(unitOfWork.TipoSolicitacaoRepository)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
