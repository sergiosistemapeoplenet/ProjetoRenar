using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Services
{
    public interface ILayoutEtiquetaDomainService
    {
        void Cadastrar(LayoutEtiqueta layoutEtiqueta);
        void Atualizar(LayoutEtiqueta layoutEtiqueta);
        LayoutEtiqueta Obter(int idLayout);
        List<LayoutEtiqueta> GetAll(string nomeLayoutEtiqueta, int flagAtivo);
    }
}
