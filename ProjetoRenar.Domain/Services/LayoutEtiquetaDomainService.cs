using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Contracts.Services;
using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Services
{
    public class LayoutEtiquetaDomainService : ILayoutEtiquetaDomainService
    {
        private readonly IUnitOfWork unitOfWork;

        public LayoutEtiquetaDomainService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public LayoutEtiqueta Obter(int idLayout)
        {
            return unitOfWork.LayoutEtiquetaRepository.GetById(idLayout);
        }

        public List<LayoutEtiqueta> GetAll(string nomeLayoutEtiqueta, int flagAtivo)
        {
            return unitOfWork.LayoutEtiquetaRepository.GetAll(nomeLayoutEtiqueta, flagAtivo);
        }

        public void Cadastrar(LayoutEtiqueta layoutEtiqueta)
        {
            unitOfWork.LayoutEtiquetaRepository.Insert(layoutEtiqueta);
        }

        public void Atualizar(LayoutEtiqueta layoutEtiqueta)
        {
            unitOfWork.LayoutEtiquetaRepository.Update(layoutEtiqueta);
        }
    }
}
