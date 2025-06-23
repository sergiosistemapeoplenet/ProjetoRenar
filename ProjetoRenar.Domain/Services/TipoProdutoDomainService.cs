using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Contracts.Services;
using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Services
{
    public class TipoProdutoDomainService : ITipoProdutoDomainService
    {
        private readonly IUnitOfWork unitOfWork;

        public TipoProdutoDomainService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public List<TipoProduto> Consultar()
        {
            return unitOfWork.TipoProdutoRepository.GetAll();
        }

        public TipoProduto ObterPorId(int id)
        {
            return unitOfWork.TipoProdutoRepository.GetById((byte)id);
        }

        public void Cadastrar(TipoProduto tipoProduto)
        {
            unitOfWork.TipoProdutoRepository.Insert(tipoProduto);
        }

        public void Atualizar(TipoProduto tipoProduto)
        {
            unitOfWork.TipoProdutoRepository.Update(tipoProduto);
        }

        public void Excluir(int id)
        {
            unitOfWork.TipoProdutoRepository.Delete(id);
        }

        public List<TipoProduto> Consultar(string nomeTipoProduto, int flagAtivo)
        {
            return unitOfWork.TipoProdutoRepository.GetAll(nomeTipoProduto, flagAtivo);
        }

        public void Reativar(int id)
        {
            unitOfWork.TipoProdutoRepository.UnDelete(id);
        }
    }
}
