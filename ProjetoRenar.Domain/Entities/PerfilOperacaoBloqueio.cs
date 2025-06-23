using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Entities
{
    public class PerfilOperacaoBloqueio
    {
        public byte IDPerfil { get; set; }
        public int IDOperacaoBloqueio { get; set; }

        public virtual Perfil Perfil { get; set; }
        public virtual OperacaoBloqueio OperacaoBloqueio { get; set; }
    }
}
