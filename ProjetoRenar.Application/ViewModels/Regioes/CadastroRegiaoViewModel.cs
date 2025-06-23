using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProjetoRenar.Application.ViewModels.Regioes
{
    public class CadastroRegiaoViewModel
    {
        [MaxLength(30, ErrorMessage = "Informe no máximo {1} caracteres.")]
        [Required(ErrorMessage = "Por favor, informe o nome da região.")]
        public string NomeRegiao { get; set; }

        public bool? FlagAtivo { get; set; }
    }
}
