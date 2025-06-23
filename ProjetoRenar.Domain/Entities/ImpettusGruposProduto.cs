using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Entities
{
    public class ImpettusGruposProduto
    {
        public int IDGrupoProduto { get; set; }
        public string NomeGrupoProduto { get; set; }
        public bool FlagSituacao { get; set; }
    }
}
