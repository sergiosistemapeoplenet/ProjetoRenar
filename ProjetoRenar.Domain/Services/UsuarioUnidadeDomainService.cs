using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Contracts.Services;
using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Services
{
    public class UsuarioUnidadeDomainService : IUsuarioUnidadeDomainService
    {
        private readonly IUnitOfWork unitOfWork;

        public UsuarioUnidadeDomainService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public List<UsuarioUnidade> ObterUnidades(int idUsuario)
        {
            return unitOfWork.UsuarioUnidadeRepository.GetByKey(idUsuario);
        }
    }
}
