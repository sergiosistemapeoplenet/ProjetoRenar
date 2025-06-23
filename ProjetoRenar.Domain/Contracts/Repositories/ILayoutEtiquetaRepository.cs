using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Repositories
{
    public interface ILayoutEtiquetaRepository
    {
        List<LayoutEtiqueta> GetAll(string nomeLayoutEtiqueta, int flagAtivo);
        LayoutEtiqueta GetById(int id);
        LayoutEtiqueta GetByCodLayout(string codLayout);
        void Insert(LayoutEtiqueta layoutEtiqueta);
        void Update(LayoutEtiqueta layoutEtiqueta);
        void Delete(short id);
    }
}
