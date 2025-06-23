using ProjetoRenar.Application.ViewModels.Usuarios;
using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.Contracts
{
    public interface IUsuarioApplicationService
    {
        void Cadastrar(CadastroUsuarioViewModel model);
        void Atualizar(EdicaoUsuarioViewModel model);
        void Excluir(int id);
        void Reativar(int id);
        ConsultaUsuarioViewModel Obter(int idUsuario);
        List<ConsultaUsuarioViewModel> Consultar();
        List<ConsultaPerfilViewModel> ConsultarPerfis();
        ConsultaPerfilViewModel ObterPerfil(int idPerfil);
        MinhaContaViewModel Obter(LoginViewModel model);
        void RedefinirSenha(RedefinirSenhaViewModel model);

        string GravarTokenRecuperacaoDeSenha(int? idUsuario);
        void ApagarTokenRecuperacaoDeSenha(int? idUsuario);
        Usuario ObterTokenRecuperacaoDeSenha(string token);

        Usuario ObterPorEmail(string email);
        void AtualizarSenhaUsuario(string senha, int idUsuario);        
    }
}
