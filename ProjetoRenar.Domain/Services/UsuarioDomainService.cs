using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Contracts.Services;
using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Services
{
    public class UsuarioDomainService : IUsuarioDomainService
    {
        private readonly IUnitOfWork unitOfWork;

        public UsuarioDomainService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Usuario Obter(string email)
        {
            return unitOfWork.UsuarioRepository.Get(email);
        }

        public Usuario Obter(string email, string senha)
        {
            var usuario = unitOfWork.UsuarioRepository.Get(email, senha);
            return usuario;
        }

        public void Cadastrar(Usuario usuario, int idUnidade)
        {
            unitOfWork.UsuarioRepository.Insert(usuario, idUnidade);
        }

        public void Atualizar(Usuario usuario)
        {
            unitOfWork.UsuarioRepository.Update(usuario);
        }

        public void Excluir(int id)
        {
            unitOfWork.UsuarioRepository.Delete((short)id);
        }

        public void Reativar(int id)
        {
            unitOfWork.UsuarioRepository.UnDelete((short)id);
        }

        public List<Usuario> Consultar()
        {
            return unitOfWork.UsuarioRepository.GetAll();
        }

        public Usuario ObterPorId(int id)
        {
            return unitOfWork.UsuarioRepository.GetById(id);
        }

        public List<Perfil> ConsultarPerfis()
        {
            return unitOfWork.PerfilRepository.GetAll();
        }

        public Perfil ObterPerfil(int idPerfil)
        {
            return unitOfWork.PerfilRepository.GetById((short)idPerfil);
        }

        public void RedefinirSenha(int? idUsuario, string senhaAtual, string novaSenha)
        {
            unitOfWork.UsuarioRepository.RedefinirSenha(idUsuario, senhaAtual, novaSenha);
        }

        public string GravarTokenRecuperacaoDeSenha(int? idUsuario)
        {
            return unitOfWork.UsuarioRepository.GravarTokenRecuperacaoDeSenha(idUsuario);
        }

        public void ApagarTokenRecuperacaoDeSenha(int? idUsuario)
        {
            unitOfWork.UsuarioRepository.ApagarTokenRecuperacaoDeSenha(idUsuario);
        }

        public Usuario ObterTokenRecuperacaoDeSenha(string token)
        {
            return unitOfWork.UsuarioRepository.ObterTokenRecuperacaoDeSenha(token);
        }

        public void AtualizarSenhaUsuario(string senha, int idUsuario)
        {
            unitOfWork.UsuarioRepository.AtualizarSenhaUsuario(senha, idUsuario);
        }
    }
}
