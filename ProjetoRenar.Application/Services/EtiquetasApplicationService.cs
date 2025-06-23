using AutoMapper;
using ProjetoRenar.Application.Contracts;
using ProjetoRenar.Application.ViewModels.Etiquetas;
using ProjetoRenar.Application.ViewModels.Produtos;
using ProjetoRenar.Application.ViewModels.TiposProduto;
using ProjetoRenar.Domain.Contracts.Services;
using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.Services
{
    public class EtiquetasApplicationService : IEtiquetasApplicationService
    {
        private readonly ITipoProdutoDomainService _tipoProdutoDomainService;
        private readonly IProdutoDomainService _produtoDomainService;
        private readonly ILayoutEtiquetaDomainService _layoutEtiquetaDomainService;

        public EtiquetasApplicationService(ITipoProdutoDomainService tipoProdutoDomainService, IProdutoDomainService produtoDomainService, ILayoutEtiquetaDomainService layoutEtiquetaDomainService)
        {
            _tipoProdutoDomainService = tipoProdutoDomainService;
            _produtoDomainService = produtoDomainService;
            _layoutEtiquetaDomainService = layoutEtiquetaDomainService;
        }

        public List<ConsultaTipoProdutoViewModel> ObterTiposDeProduto()
        {
            return Mapper.Map<List<ConsultaTipoProdutoViewModel>>(_tipoProdutoDomainService.Consultar());
        }

        public List<ConsultaProdutoViewModel> ObterProdutos(int idTipoProduto)
        {
            return Mapper.Map<List<ConsultaProdutoViewModel>>(_produtoDomainService.ObterPorTipo(idTipoProduto));
        }

        public ConsultaLayoutEtiquetaViewModel ObterLayoutEtiqueta(int idLayoutEtiqueta)
        {
            return Mapper.Map<ConsultaLayoutEtiquetaViewModel>(_layoutEtiquetaDomainService.Obter(idLayoutEtiqueta));
        }

        public ConsultaProdutoViewModel ObterProduto(int idProduto)
        {
            return Mapper.Map<ConsultaProdutoViewModel>(_produtoDomainService.ObterPorId(idProduto));
        }
    }
}
