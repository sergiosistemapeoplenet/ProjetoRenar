using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Entities
{
    public class ImpettusPreparacao
    {
        public int IdPreparacao { get; set; }
        public string NomePreparacao { get; set; } = string.Empty;
        public bool FlagResfriado { get; set; }
        public string TipoValidadeResfriado { get; set; }
        public int? ValidadeResfriado { get; set; }
        public bool FlagCongelado { get; set; }
        public int? ValidadeCongelado { get; set; }
        public bool FlagTemperaturaAmbiente { get; set; }
        public string TipoValidadeTemperaturaAmbiente { get; set; }
        public int? ValidadeTemperaturaAmbiente { get; set; }
        public int? IdGrupoPreparacao { get; set; }
        public string NomeGrupoPreparacao { get; set; }
        public bool FlagAtivo { get; set; }
        public string Sif { get; set; }
        public int FlagFavorito { get; set; }
    }
}
