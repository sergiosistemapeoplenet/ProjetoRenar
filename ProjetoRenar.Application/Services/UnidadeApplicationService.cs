using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using ProjetoRenar.Application.Contracts;
using ProjetoRenar.Application.Exceptions.Usuarios;
using ProjetoRenar.Application.Generators;
using ProjetoRenar.Application.ViewModels.Regioes;
using ProjetoRenar.Application.ViewModels.Unidades;
using ProjetoRenar.Application.ViewModels.Usuarios;
using ProjetoRenar.Domain.Contracts.Cryptographies;
using ProjetoRenar.Domain.Contracts.Messages;
using ProjetoRenar.Domain.Contracts.Services;
using ProjetoRenar.Domain.Entities;
using ProjetoRenar.Domain.Models;
using ProjetoRenar.Infra.Cryptography;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ProjetoRenar.Application.Services
{
    public class UnidadeApplicationService : IUnidadeApplicationService
    {
        private readonly IUnidadeDomainService unidadeDomainService;
        private readonly IUsuarioUnidadeDomainService usuarioUnidadeDomainService;
        private readonly IRegiaoDomainService regiaoDomainService;

        public UnidadeApplicationService(IUnidadeDomainService unidadeDomainService, IUsuarioUnidadeDomainService usuarioUnidadeDomainService, IRegiaoDomainService regiaoDomainService)
        {
            this.unidadeDomainService = unidadeDomainService;
            this.usuarioUnidadeDomainService = usuarioUnidadeDomainService;
            this.regiaoDomainService = regiaoDomainService;
        }

        public List<ConsultaUnidadeViewModel> Obter(int idUsuario)
        {
            var lista = new List<ConsultaUnidadeViewModel>();

            var unidades = usuarioUnidadeDomainService.ObterUnidades(idUsuario);

            var registros = unidades.Where(u => u.FlagUnidadePadrao != null && u.FlagUnidadePadrao.Value).ToList();
            if (!registros.Any())
            {
                var dados = new List<UsuarioUnidade>();
                dados.Add(unidades.FirstOrDefault());
                unidades = dados;
            }

            foreach (var item in unidades)
            {
                try
                {
                    var unidade = Mapper.Map<ConsultaUnidadeViewModel>
                    (unidadeDomainService.Obter(item.IDUnidade));

                    if (item.FlagUnidadePadrao != null)
                    {
                        unidade.FlagUnidadePadrao = item.FlagUnidadePadrao.Value;
                    }

                    unidade.Regiao = Mapper.Map<ConsultaRegiaoViewModel>
                        (regiaoDomainService.Obter(unidade.IDRegiao));

                    lista.Add(unidade);
                }
                catch (Exception e) { }
            }

            return lista;
        }

        public List<ConsultaUnidadeViewModel> Consultar()
        {
            var unidades = Mapper.Map<List<ConsultaUnidadeViewModel>>
                    (unidadeDomainService.Consultar());

            foreach (var item in unidades)
            {
                item.Regiao = Mapper.Map<ConsultaRegiaoViewModel>
                    (regiaoDomainService.Obter(item.IDRegiao));
            }

            return unidades;
        }

        public void Cadastrar(CadastroUnidadeViewModel model)
        {
            var unidade = Mapper.Map<Unidade>(model);
            unidadeDomainService.Cadastrar(unidade);
        }

        public void Atualizar(EdicaoUnidadeViewModel model)
        {
            var unidade = Mapper.Map<Unidade>(model);
            unidadeDomainService.Atualizar(unidade);
        }

        public void Excluir(int id)
        {
            unidadeDomainService.Excluir(id);
        }

        public void Reativar(int id)
        {
            unidadeDomainService.Reativar(id);
        }

        public List<ConsultaRegiaoViewModel> ConsultarRegioes()
        {
            return Mapper.Map<List<ConsultaRegiaoViewModel>>(regiaoDomainService.Consultar());
        }

        public ConsultaUnidadeViewModel ObterPorId(int id)
        {
            return Mapper.Map<ConsultaUnidadeViewModel>(unidadeDomainService.Obter(id));
        }
    }
}




