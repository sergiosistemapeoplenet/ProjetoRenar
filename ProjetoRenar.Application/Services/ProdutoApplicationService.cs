using AutoMapper;
using ProjetoRenar.Application.Contracts;
using ProjetoRenar.Application.ViewModels.Produtos;
using ProjetoRenar.Application.ViewModels.TiposProduto;
using ProjetoRenar.Domain.Contracts.Services;
using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.Services
{
    public class ProdutoApplicationService : IProdutoApplicationService
    {
        private readonly IProdutoDomainService _produtoDomainService;

        public ProdutoApplicationService(IProdutoDomainService produtoDomainService)
        {
            _produtoDomainService = produtoDomainService;
        }

        public void Cadastrar(CadastroProdutoViewModel model)
        {
            var produto = Mapper.Map<Produto>(model);
            _produtoDomainService.Cadastrar(produto);
        }

        public void Atualizar(EdicaoProdutoViewModel model)
        {
            var produto = Mapper.Map<Produto>(model);
            _produtoDomainService.Atualizar(produto);
        }

        public void Excluir(int id)
        {
            _produtoDomainService.Excluir(id);
        }

        public void Reativar(int id)
        {
            _produtoDomainService.Reativar(id);
        }

        public List<ConsultaProdutoViewModel> Consultar(string nomeProduto, int flagAtivo)
        {
            return Mapper.Map<List<ConsultaProdutoViewModel>>
                (_produtoDomainService.Consultar(nomeProduto, flagAtivo));
        }

        public ConsultaProdutoViewModel Obter(int id)
        {
            return Mapper.Map<ConsultaProdutoViewModel>
                (_produtoDomainService.ObterPorId(id));
        }

        public void IncluirControleImpressao(int idUnidade, int idProduto, int quantidadeEtiqueta, int idUsuario)
        {
            _produtoDomainService.IncluirControleImpressao(idUnidade, idProduto, quantidadeEtiqueta, idUsuario);
        }
    }
}
