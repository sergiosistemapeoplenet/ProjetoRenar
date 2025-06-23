using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Services
{
    public interface IUsuarioUnidadeDomainService
    {
        List<UsuarioUnidade> ObterUnidades(int idUsuario);       
    }
}
