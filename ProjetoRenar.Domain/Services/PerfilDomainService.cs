using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Contracts.Services;
using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Services
{
    public class PerfilDomainService : IPerfilDomainService
    {
        private readonly IUnitOfWork unitOfWork;

        public PerfilDomainService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
    }
}
