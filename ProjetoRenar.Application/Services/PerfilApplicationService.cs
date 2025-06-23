using AutoMapper;
using ProjetoRenar.Application.Contracts;
using ProjetoRenar.Domain.Contracts.Services;
using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.Services
{
    public class PerfilApplicationService : IPerfilApplicationService
    {
        private readonly IPerfilDomainService domainService;

        public PerfilApplicationService(IPerfilDomainService domainService)
        {
            this.domainService = domainService;
        }
    }
}
