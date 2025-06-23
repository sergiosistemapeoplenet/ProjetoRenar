using AutoMapper;
using ProjetoRenar.Application.ViewModels.LayoutEtiquetas;
using ProjetoRenar.Application.ViewModels.Produtos;
using ProjetoRenar.Application.ViewModels.Regioes;
using ProjetoRenar.Application.ViewModels.TiposProduto;
using ProjetoRenar.Application.ViewModels.Unidades;
using ProjetoRenar.Application.ViewModels.Usuarios;
using ProjetoRenar.Domain.Entities;
using ProjetoRenar.Infra.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.Profiles
{
    public class ViewModelToEntity : Profile
    {
        public ViewModelToEntity()
        {
            CreateMap<CadastroLayoutEtiquetaViewModel, LayoutEtiqueta>();
            CreateMap<EdicaoLayoutEtiquetaViewModel, LayoutEtiqueta>();

            CreateMap<CadastroTipoProdutoViewModel, TipoProduto>();
            CreateMap<EdicaoTipoProdutoViewModel, TipoProduto>();

            CreateMap<CadastroProdutoViewModel, Produto>();
            CreateMap<EdicaoProdutoViewModel, Produto>();

            CreateMap<CadastroUnidadeViewModel, Unidade>();
            CreateMap<EdicaoUnidadeViewModel, Unidade>();

            CreateMap<CadastroRegiaoViewModel, Regiao>();
            CreateMap<EdicaoRegiaoViewModel, Regiao>();

            CreateMap<CadastroUsuarioViewModel, Usuario>();
            CreateMap<EdicaoUsuarioViewModel, Usuario>();
        }
    }
}
