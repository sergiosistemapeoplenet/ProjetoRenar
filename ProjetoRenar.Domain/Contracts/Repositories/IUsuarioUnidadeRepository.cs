using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Repositories
{
    public interface IUsuarioUnidadeRepository
    {
        List<UsuarioUnidade> GetAll();
        List<UsuarioUnidade> GetByKey(int idUsuario);
        void Insert(UsuarioUnidade usuarioUnidade);
        void Delete(short idUsuario, short idUnidade);
        void Delete(short idUsuario);
    }
}
