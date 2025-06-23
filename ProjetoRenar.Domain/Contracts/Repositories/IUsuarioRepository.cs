using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Repositories
{
    public interface IUsuarioRepository
    {
        List<Usuario> GetAll();
        Usuario GetById(int id);
        void Insert(Usuario usuario, int idUnidade);
        void Update(Usuario usuario);
        void Delete(short id);
        void UnDelete(short id);

        Usuario Get(string email);
        Usuario Get(string email, string senha);

        void RedefinirSenha(int? idUsuario, string senhaAtual, string novaSenha);

        string GravarTokenRecuperacaoDeSenha(int? idUsuario);
        void ApagarTokenRecuperacaoDeSenha(int? idUsuario);
        Usuario ObterTokenRecuperacaoDeSenha(string token);
        void AtualizarSenhaUsuario(string senha, int idUsuario);
        void DefinirPrimeiroAcesso(int idUsuario);
    }
}
