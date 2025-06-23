using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetoRenar.Application.ViewModels.Etiquetas;
using ProjetoRenar.Application.ViewModels.TiposProduto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProjetoRenar.Application.ViewModels.Produtos
{
    public class CadastroProdutoViewModel
    {
        public int IDProduto { get; set; }

        [Required(ErrorMessage = "Por favor, informe o nome do produto.")]
        public string NomeProduto { get; set; }
        public string CodigoBarra { get; set; }

        [Required(ErrorMessage = "Informe o peso.")]
        public string Peso { get; set; }

        [Required(ErrorMessage = "Informe uma opção.")]
        public bool? FlagFatiacopo { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string ValorEnergetico { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string PorcaoValorEnergetico { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string ValorEnergeticoValorDiario { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string Carboidratos { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string PorcaoCarboidratos { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string CarboidratosValorDiario { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string AcucarTotal { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string PorcaoAcucarTotal { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string AcucarTotalValorDiario { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string AcucarAdicionado { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string PorcaoAcucarAdicionado { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string AcucarAdicionadoValorDiario { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string Proteina { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string PorcaoProteina { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string ProteinaValorDiario { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string GorduraTotal { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string PorcaoGorduraTotal { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string Gtotvd { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string GorduraSaturada { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string PorcaoGorduraSaturada { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string GorduraSaturadaValorDiario { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string GorduraTrans { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string PorcaoGorduraTrans { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string GorduraTransValorDiario { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string FibraAlimentar { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string PorcaoFibraAlimentar { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string FibraAlimentarValorDiario { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string Sodio { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string PorcaoSodio { get; set; }

       [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido.")]
        public string SodioValorDiario { get; set; }

        [MaxLength(600, ErrorMessage = "Informe no máximo {1} caracteres.")]
        public string Receita { get; set; }

        [MaxLength(250, ErrorMessage = "Informe no máximo {1} caracteres.")]
        public string Info1 { get; set; }

        [MaxLength(120, ErrorMessage = "Informe no máximo {1} caracteres.")]
        public string Info2 { get; set; }

        [MaxLength(250, ErrorMessage = "Informe no máximo {1} caracteres.")]
        public string Info3 { get; set; }

        public string PorcaoEmbalagem { get; set; }

        public string PorcaoFatia { get; set; }

        [Required(ErrorMessage = "Por favor, selecione o tipo do produto.")]
        public byte? IDTipoProduto { get; set; }

        public bool? FlagAtivo { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "Valor inválido.")]
        public string DiasValidade { get; set; }

        public DateTime DataAtual { get => new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0); }
        public DateTime DataValidade { get => DiasValidade != null ? DataAtual.AddDays(int.Parse(DiasValidade)) : new DateTime(); }
        public virtual ConsultaTipoProdutoViewModel TipoProduto { get; set; }
        public int QtdImpressoes { get; set; }

        public string Link { get; set; }

        public List<SelectListItem> TiposProduto { get; set; }
        public List<SelectListItem> TiposFatiaCopo { get; set; }
    }
}
