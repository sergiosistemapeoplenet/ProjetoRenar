using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjetoRenar.Presentation.Mvc.Areas.App.Models
{
    public class ProdutoCadastroViewModel
    {
        [Required(ErrorMessage = "O nome do produto é obrigatório")]
        [StringLength(50, ErrorMessage = "O nome não pode ultrapassar 50 caracteres")]
        public string NomeProduto { get; set; } = string.Empty;

        public bool FlagResfriado { get; set; }

        public string TipoValidadeResfriado { get; set; }

        public int? ValidadeResfriado { get; set; }

        public bool FlagCongelado { get; set; }

        public int? ValidadeCongelado { get; set; }

        public bool FlagTemperaturaAmbiente { get; set; }

        public string TipoValidadeTemperaturaAmbiente { get; set; }

        public int? ValidadeTemperaturaAmbiente { get; set; }

        [Required(ErrorMessage = "O Grupo do Produto é obrigatório")]
        public int? IdGrupoProduto { get; set; }
        public bool FlagAtivo { get; set; }

        [StringLength(5, ErrorMessage = "SIF não pode ultrapassar 5 caracteres")]
        public string Sif { get; set; }

        public List<SelectListItem> Grupos { get; set; }
        public List<SelectListItem> Tipos { get; set; }
    }
}
