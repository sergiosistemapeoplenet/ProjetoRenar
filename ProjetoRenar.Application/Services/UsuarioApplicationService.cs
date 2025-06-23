using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using ProjetoRenar.Application.Contracts;
using ProjetoRenar.Application.Exceptions.Usuarios;
using ProjetoRenar.Application.Generators;
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
using System.Security.Cryptography;
using System.Text;

namespace ProjetoRenar.Application.Services
{
    public class UsuarioApplicationService : IUsuarioApplicationService
    {
        private readonly IUsuarioDomainService domainService;
        private readonly IEmailMessage emailMessage;
        private readonly IMD5Cryptography md5Cryptography;
        private readonly IPerfilDomainService perfilDomainService;

        public UsuarioApplicationService(IUsuarioDomainService domainService, IEmailMessage emailMessage, IMD5Cryptography md5Cryptography, IPerfilDomainService perfilDomainService)
        {
            this.domainService = domainService;
            this.emailMessage = emailMessage;
            this.md5Cryptography = md5Cryptography;
            this.perfilDomainService = perfilDomainService;
        }

        public Usuario ObterPorEmail(string email)
        {
            return domainService.Obter(email);
        }

        public MinhaContaViewModel Obter(LoginViewModel model)
        {
            var usuarioPorEmail = domainService.Obter(model.EmailUsuario);

            if (usuarioPorEmail != null && model.SenhaUsuario != usuarioPorEmail.SenhaUsuario)
            {
                throw new SenhaInvalidaException();
            }

            var usuario = domainService.Obter(model.EmailUsuario, model.SenhaUsuario);

            if (usuarioPorEmail != null && domainService.Obter(model.EmailUsuario).FlagBloqueado)
            {
                throw new UsuarioBloqueadoException();
            }
            else if (usuario != null)
            {
                var result = Mapper.Map<MinhaContaViewModel>(usuario);                
                return result;
            }           
            else
            {
                throw new AcessoNegadoException();
            }
        }

        public void Cadastrar(CadastroUsuarioViewModel model)
        {
            var usuario = Mapper.Map<Usuario>(model);
            domainService.Cadastrar(usuario, model.IDUnidade.Value);
        }

        public void Atualizar(EdicaoUsuarioViewModel model)
        {
            var usuario = Mapper.Map<Usuario>(model);
            domainService.Atualizar(usuario);
        }

        public void Excluir(int id)
        {
            domainService.Excluir(id);
        }

        public void Reativar(int id)
        {
            domainService.Reativar(id);
        }

        public List<ConsultaUsuarioViewModel> Consultar()
        {
            return Mapper.Map<List<ConsultaUsuarioViewModel>>
                (domainService.Consultar());
        }

        public ConsultaUsuarioViewModel Obter(int id)
        {
            return Mapper.Map<ConsultaUsuarioViewModel>
                (domainService.ObterPorId(id));
        }

        public List<ConsultaPerfilViewModel> ConsultarPerfis()
        {
            return Mapper.Map<List<ConsultaPerfilViewModel>>
                (domainService.ConsultarPerfis());
        }

        public ConsultaPerfilViewModel ObterPerfil(int idPerfil)
        {
            return Mapper.Map<ConsultaPerfilViewModel>
                (domainService.ObterPerfil(idPerfil));
        }

        public void RedefinirSenha(RedefinirSenhaViewModel model)
        {
            domainService.RedefinirSenha(model.IDUsuarioInclusao, model.SenhaAtualUsuario, model.NovaSenhaUsuario);
        }

        public string GravarTokenRecuperacaoDeSenha(int? idUsuario)
        {
            return domainService.GravarTokenRecuperacaoDeSenha(idUsuario);
        }

        public void ApagarTokenRecuperacaoDeSenha(int? idUsuario)
        {
            domainService.ApagarTokenRecuperacaoDeSenha(idUsuario);
        }

        public Usuario ObterTokenRecuperacaoDeSenha(string token)
        {
            return domainService.ObterTokenRecuperacaoDeSenha(token);
        }

        public void AtualizarSenhaUsuario(string senha, int idUsuario)
        {
            domainService.AtualizarSenhaUsuario(senha, idUsuario);
        }
    }
}




