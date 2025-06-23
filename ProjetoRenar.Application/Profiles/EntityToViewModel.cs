using AutoMapper;
using ProjetoRenar.Application.ViewModels;
using ProjetoRenar.Application.ViewModels.Etiquetas;
using ProjetoRenar.Application.ViewModels.Produtos;
using ProjetoRenar.Application.ViewModels.Regioes;
using ProjetoRenar.Application.ViewModels.TiposProduto;
using ProjetoRenar.Application.ViewModels.Unidades;
using ProjetoRenar.Application.ViewModels.Usuarios;
using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.Profiles
{
    public class EntityToViewModel : Profile
    {
        public EntityToViewModel()
        {
            CreateMap<Usuario, MinhaContaViewModel>();
            CreateMap<TipoProduto, ConsultaTipoProdutoViewModel>();
            CreateMap<Produto, ConsultaProdutoViewModel>();
            CreateMap<LayoutEtiqueta, ConsultaLayoutEtiquetaViewModel>();
            CreateMap<Unidade, ConsultaUnidadeViewModel>();
            CreateMap<LayoutEtiqueta, ConsultaLayoutEtiquetaViewModel>();
            CreateMap<Regiao, ConsultaRegiaoViewModel>();
            CreateMap<Usuario, ConsultaUsuarioViewModel>();
            CreateMap<Perfil, ConsultaPerfilViewModel>();
        }
    }
}
