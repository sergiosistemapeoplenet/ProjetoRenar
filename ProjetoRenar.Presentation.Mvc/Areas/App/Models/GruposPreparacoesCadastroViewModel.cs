using System.ComponentModel.DataAnnotations;

namespace ProjetoRenar.Presentation.Mvc.Areas.App.Models
{
    public class GruposPreparacoesCadastroViewModel
    {
        [MaxLength(15, ErrorMessage = "Informe no máximo {1} caracteres.")]
        [Required(ErrorMessage = "Por favor, informe o nome do grupo.")]
        public string NomeGrupoPreparacao { get; set; }

        public bool? FlagAtivo { get; set; }
    }
}
