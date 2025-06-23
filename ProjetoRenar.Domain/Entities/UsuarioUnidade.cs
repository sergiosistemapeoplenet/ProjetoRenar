using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Entities
{
    public class UsuarioUnidade
    {
        public short IDUsuario { get; set; }
        public short IDUnidade { get; set; }

        public bool? FlagUnidadePadrao { get; set; }

        public virtual Usuario Usuario { get; set; }
        public virtual Unidade Unidade { get; set; }
    }
}
