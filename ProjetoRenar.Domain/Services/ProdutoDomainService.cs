using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Contracts.Services;
using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Services
{
    public class ProdutoDomainService : IProdutoDomainService
    {
        private readonly IUnitOfWork unitOfWork;

        public ProdutoDomainService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public List<Produto> Consultar()
        {
            return unitOfWork.ProdutoRepository.GetAll();
        }

        public Produto ObterPorId(int id)
        {
            return unitOfWork.ProdutoRepository.GetById(id);
        }

        public void Cadastrar(Produto produto)
        {
            unitOfWork.ProdutoRepository.Insert(produto);
        }

        public void Atualizar(Produto produto)
        {
            unitOfWork.ProdutoRepository.Update(produto);
        }

        public void Excluir(int id)
        {
            unitOfWork.ProdutoRepository.Delete(id);
        }

        public List<Produto> Consultar(string nomeProduto, int flagAtivo)
        {
            return unitOfWork.ProdutoRepository.GetAll(nomeProduto, flagAtivo);
        }

        public void Reativar(int id)
        {
            unitOfWork.ProdutoRepository.UnDelete(id);
        }

        public List<Produto> ObterPorTipo(int idTipoPRoduto)
        {
            return unitOfWork.ProdutoRepository.GetByTipo(idTipoPRoduto);
        }

        public void IncluirControleImpressao(int idUnidade, int idProduto, int quantidadeEtiqueta, int idUsuario)
        {
            unitOfWork.ProdutoRepository.IncluirControleImpressao(idUnidade, idProduto, quantidadeEtiqueta, idUsuario);
        }
    }
}
