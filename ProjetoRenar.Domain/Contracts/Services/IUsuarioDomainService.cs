using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Services
{
    public interface IUsuarioDomainService
    {
        void Cadastrar(Usuario usuario, int idUnidade);
        void Atualizar(Usuario usuario);
        void Excluir(int id);
        void Reativar(int id);

        List<Usuario> Consultar();
        Usuario ObterPorId(int id);

        Usuario Obter(string email);
        Usuario Obter(string email, string senha);

        List<Perfil> ConsultarPerfis();
        Perfil ObterPerfil(int idPerfil);

        void RedefinirSenha(int? idUsuario, string senhaAtual, string novaSenha);

        string GravarTokenRecuperacaoDeSenha(int? idUsuario);
        void ApagarTokenRecuperacaoDeSenha(int? idUsuario);
        Usuario ObterTokenRecuperacaoDeSenha(string token);
        void AtualizarSenhaUsuario(string senha, int idUsuario);
    }
}
