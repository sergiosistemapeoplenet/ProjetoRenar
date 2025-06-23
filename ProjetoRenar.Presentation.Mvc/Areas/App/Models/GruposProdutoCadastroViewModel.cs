using System.ComponentModel.DataAnnotations;

namespace ProjetoRenar.Presentation.Mvc.Areas.App.Models
{
    public class GruposProdutoCadastroViewModel
    {
        [MaxLength(30, ErrorMessage = "Informe no máximo {1} caracteres.")]
        [Required(ErrorMessage = "Por favor, informe o nome do grupo.")]
        public string NomeGrupoProduto { get; set; }

        public bool? FlagAtivo { get; set; }
    }
}
