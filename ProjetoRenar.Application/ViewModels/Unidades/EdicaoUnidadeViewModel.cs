using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProjetoRenar.Application.ViewModels.Unidades
{
    public class EdicaoUnidadeViewModel
    {
        public int IdUnidade { get; set; }

        [MaxLength(70, ErrorMessage = "Informe no máximo {1} caracteres.")]
        [Required(ErrorMessage = "Por favor, informe o nome da unidade.")]
        public string NomeUnidade { get; set; }

        [MaxLength(70, ErrorMessage = "Informe no máximo {1} caracteres.")]
        [Required(ErrorMessage = "Por favor, informe a razão social.")]
        public string RazaoSocial { get; set; }

        [MaxLength(18, ErrorMessage = "Informe no máximo {1} caracteres.")]
        [Required(ErrorMessage = "Por favor, informe o cnpj da unidade.")]
        public string CNPJ { get; set; }

        [Required(ErrorMessage = "Por favor, selecione a região.")]
        public short? IDRegiao { get; set; }

        [MaxLength(120, ErrorMessage = "Informe no máximo {1} caracteres.")]
        [Required(ErrorMessage = "Por favor, informe o endereço da unidade.")]
        public string Endereco { get; set; }

        public bool FlagAtivo { get; set; }

        [MaxLength(10, ErrorMessage = "Informe no máximo {1} caracteres.")]
        [Required(ErrorMessage = "Por favor, informe o cep da unidade.")]
        public string Cep { get; set; }

        [MaxLength(100, ErrorMessage = "Informe no máximo {1} caracteres.")]
        [Required(ErrorMessage = "Por favor, informe o horário de funcionamento da unidade.")]
        public string HorarioFuncionamento { get; set; }

        [MaxLength(20, ErrorMessage = "Informe no máximo {1} caracteres.")]
        [Required(ErrorMessage = "Por favor, informe o código da impressora.")]
        public string SerialImpressora { get; set; }

        [MaxLength(30, ErrorMessage = "Informe no máximo {1} caracteres.")]
        [Required(ErrorMessage = "Por favor, informe o nome do contato da unidade.")]
        public string NomeContato { get; set; }

        [MaxLength(35, ErrorMessage = "Informe no máximo {1} caracteres.")]
        [Required(ErrorMessage = "Por favor, informe o número do contato da unidade.")]
        public string NumeroContato { get; set; }

        [EmailAddress(ErrorMessage = "Informe um endereço de email válido")]
        [MaxLength(70, ErrorMessage = "Informe no máximo {1} caracteres.")]
        [Required(ErrorMessage = "Por favor, informe o email do contato da unidade.")]
        public string EmailContato { get; set; }

        [Required(ErrorMessage = "Por favor, selecione uma opção.")]
        public bool FlagImprimeCodigoBarra { get; set; }

        public List<SelectListItem> Regioes { get; set; }
    }
}
