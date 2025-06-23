using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Contracts.Services;
using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Services
{
    public class UnidadeDomainService : IUnidadeDomainService
    {
        private readonly IUnitOfWork unitOfWork;

        public UnidadeDomainService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Unidade Obter(int idUnidade)
        {
            return unitOfWork.UnidadeRepository.GetById(idUnidade);
        }

        public List<Unidade> Consultar()
        {
            return unitOfWork.UnidadeRepository.GetAll();
        }

        public void Cadastrar(Unidade unidade)
        {
            unitOfWork.UnidadeRepository.Insert(unidade);
        }

        public void Atualizar(Unidade unidade)
        {
            unitOfWork.UnidadeRepository.Update(unidade);
        }

        public void Excluir(int id)
        {
            unitOfWork.UnidadeRepository.Delete((short) id);
        }

        public void Reativar(int id)
        {
            unitOfWork.UnidadeRepository.UnDelete((short)id);
        }
    }
}
