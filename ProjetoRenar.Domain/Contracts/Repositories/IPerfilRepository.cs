using System;
using System.Collections.Generic;
using System.Text;
using ProjetoRenar.Domain.Entities;

namespace ProjetoRenar.Domain.Contracts.Repositories
{
    public interface IPerfilRepository
    {
        List<Perfil> GetAll();
        Perfil GetById(short id);
        void Insert(Perfil perfil);
        void Update(Perfil perfil);
        void Delete(byte id);
    }
}
