using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Contracts.Services;
using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Services
{
    public class RegiaoDomainService : IRegiaoDomainService
    {
        private readonly IUnitOfWork unitOfWork;

        public RegiaoDomainService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Regiao Obter(int idRegiao)
        {
            return unitOfWork.RegiaoRepository.GetById(idRegiao);
        }

        public List<Regiao> Consultar()
        {
            return unitOfWork.RegiaoRepository.GetAll();
        }

        public void Cadastrar(Regiao regiao)
        {
            unitOfWork.RegiaoRepository.Insert(regiao);
        }

        public void Atualizar(Regiao regiao)
        {
            unitOfWork.RegiaoRepository.Update(regiao);
        }

        public void Excluir(int id)
        {
            unitOfWork.RegiaoRepository.Delete((short)id);
        }

        public void Reativar(int id)
        {
            unitOfWork.RegiaoRepository.UnDelete((short)id);
        }
    }
}
