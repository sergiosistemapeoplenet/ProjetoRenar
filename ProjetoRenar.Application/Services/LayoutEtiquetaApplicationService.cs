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
    public class LayoutEtiquetaApplicationService : ILayoutEtiquetaApplicationService
    {
        private readonly ILayoutEtiquetaDomainService _layoutEtiquetaDomainService;

        public LayoutEtiquetaApplicationService(ILayoutEtiquetaDomainService layoutEtiquetaDomainService)
        {
            _layoutEtiquetaDomainService = layoutEtiquetaDomainService;
        }

        public void Cadastrar(CadastroLayoutEtiquetaViewModel model)
        {
            var layoutEtiqueta = Mapper.Map<LayoutEtiqueta>(model);
            _layoutEtiquetaDomainService.Cadastrar(layoutEtiqueta);
        }

        public void Atualizar(EdicaoLayoutEtiquetaViewModel model)
        {
            var layoutEtiqueta = Mapper.Map<LayoutEtiqueta>(model);
            _layoutEtiquetaDomainService.Atualizar(layoutEtiqueta);
        }

        public List<ConsultaLayoutEtiquetaViewModel> ConsultarLayoutEtiquetas(string nomeLayoutEtiqueta, int flagAtivo)
        {
            return Mapper.Map<List<ConsultaLayoutEtiquetaViewModel>>(_layoutEtiquetaDomainService.GetAll(nomeLayoutEtiqueta, flagAtivo));
        }

        public ConsultaLayoutEtiquetaViewModel ObterLayoutEtiquetas(int id)
        {
            return Mapper.Map<ConsultaLayoutEtiquetaViewModel>(_layoutEtiquetaDomainService.Obter(id));
        }
    }
}
