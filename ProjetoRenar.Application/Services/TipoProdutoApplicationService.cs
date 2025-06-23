using AutoMapper;
using ProjetoRenar.Application.Contracts;
using ProjetoRenar.Application.ViewModels.Etiquetas;
using ProjetoRenar.Application.ViewModels.LayoutEtiquetas;
using ProjetoRenar.Application.ViewModels.Produtos;
using ProjetoRenar.Application.ViewModels.TiposProduto;
using ProjetoRenar.Domain.Contracts.Services;
using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.Services
{
    public class TipoProdutoApplicationService : ITipoProdutoApplicationService
    {
        private readonly ITipoProdutoDomainService _tipoProdutoDomainService;

        public TipoProdutoApplicationService(ITipoProdutoDomainService tipoProdutoDomainService)
        {
            _tipoProdutoDomainService = tipoProdutoDomainService;
        }

        public void Cadastrar(CadastroTipoProdutoViewModel model)
        {
            var tipoProduto = Mapper.Map<TipoProduto>(model);
            _tipoProdutoDomainService.Cadastrar(tipoProduto);
        }

        public void Atualizar(EdicaoTipoProdutoViewModel model)
        {
            var tipoProduto = Mapper.Map<TipoProduto>(model);
            _tipoProdutoDomainService.Atualizar(tipoProduto);
        }

        public void Excluir(int id)
        {
            _tipoProdutoDomainService.Excluir(id);
        }

        public void Reativar(int id)
        {
            _tipoProdutoDomainService.Reativar(id);
        }

        public List<ConsultaTipoProdutoViewModel> Consultar(string nomeTipoProduto, int flagAtivo)
        {
            return Mapper.Map<List<ConsultaTipoProdutoViewModel>>
                (_tipoProdutoDomainService.Consultar(nomeTipoProduto, flagAtivo));
        }

        public ConsultaTipoProdutoViewModel Obter(int id)
        {
            return Mapper.Map<ConsultaTipoProdutoViewModel>
                (_tipoProdutoDomainService.ObterPorId(id));
        }
    }
}
