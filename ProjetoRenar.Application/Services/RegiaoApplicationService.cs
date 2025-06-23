using AutoMapper;
using ProjetoRenar.Application.Contracts;
using ProjetoRenar.Application.ViewModels.Regioes;
using ProjetoRenar.Domain.Contracts.Services;
using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.Services
{
    public class RegiaoApplicationService : IRegiaoApplicationService
    {
        private readonly IRegiaoDomainService _regiaoProdutoDomainService;

        public RegiaoApplicationService(IRegiaoDomainService regiaoProdutoDomainService)
        {
            _regiaoProdutoDomainService = regiaoProdutoDomainService;
        }

        public void Cadastrar(CadastroRegiaoViewModel model)
        {
            var regiao = Mapper.Map<Regiao>(model);
            _regiaoProdutoDomainService.Cadastrar(regiao);
        }

        public void Atualizar(EdicaoRegiaoViewModel model)
        {
            var regiao = Mapper.Map<Regiao>(model);
            _regiaoProdutoDomainService.Atualizar(regiao);
        }

        public void Excluir(int id)
        {
            _regiaoProdutoDomainService.Excluir(id);
        }

        public void Reativar(int id)
        {
            _regiaoProdutoDomainService.Reativar(id);
        }

        public List<ConsultaRegiaoViewModel> Consultar()
        {
            return Mapper.Map<List<ConsultaRegiaoViewModel>>
                (_regiaoProdutoDomainService.Consultar());
        }

        public ConsultaRegiaoViewModel Obter(int id)
        {
            return Mapper.Map<ConsultaRegiaoViewModel>
                (_regiaoProdutoDomainService.Obter(id));
        }
    }
}
