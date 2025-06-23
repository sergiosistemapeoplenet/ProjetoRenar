using ProjetoRenar.Application.ViewModels.TiposProduto;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.ViewModels.Produtos
{
    public class ConsultaProdutoViewModel
    {
        public int IDProduto { get; set; }
        public string NomeProduto { get; set; }
        public string CodigoBarra { get; set; }
        public string Peso { get; set; }
        public bool? FlagFatiacopo { get; set; }
        public decimal? ValorEnergetico { get; set; }
        public decimal? PorcaoValorEnergetico { get; set; }
        public decimal? ValorEnergeticoValorDiario { get; set; }
        public decimal? Carboidratos { get; set; }
        public decimal? PorcaoCarboidratos { get; set; }
        public decimal? CarboidratosValorDiario { get; set; }
        public decimal? AcucarTotal { get; set; }
        public decimal? PorcaoAcucarTotal { get; set; }
        public decimal? AcucarTotalValorDiario { get; set; }
        public decimal? AcucarAdicionado { get; set; }
        public decimal? PorcaoAcucarAdicionado { get; set; }
        public decimal? AcucarAdicionadoValorDiario { get; set; }
        public decimal? Proteina { get; set; }
        public decimal? PorcaoProteina { get; set; }
        public decimal? ProteinaValorDiario { get; set; }
        public decimal? GorduraTotal { get; set; }
        public decimal? PorcaoGorduraTotal { get; set; }
        public decimal? Gtotvd { get; set; }
        public decimal? GorduraSaturada { get; set; }
        public decimal? PorcaoGorduraSaturada { get; set; }
        public decimal? GorduraSaturadaValorDiario { get; set; }
        public decimal? GorduraTrans { get; set; }
        public decimal? PorcaoGorduraTrans { get; set; }
        public decimal? GorduraTransValorDiario { get; set; }
        public decimal? FibraAlimentar { get; set; }
        public decimal? PorcaoFibraAlimentar { get; set; }
        public decimal? FibraAlimentarValorDiario { get; set; }
        public decimal? Sodio { get; set; }
        public decimal? PorcaoSodio { get; set; }
        public decimal? SodioValorDiario { get; set; }
        public string Receita { get; set; }
        public string Info1 { get; set; }
        public string Info2 { get; set; }
        public string Info3 { get; set; }
        public string PorcaoEmbalagem { get; set; }
        public string PorcaoFatia { get; set; }
        public byte? IDTipoProduto { get; set; }
        public bool? FlagAtivo { get; set; }
        public short? DiasValidade { get; set; }
        public DateTime DataAtual { get => new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0); }
        public DateTime DataValidade { get => DiasValidade != null ? DataAtual.AddDays(DiasValidade.Value) : new DateTime(); }
        public virtual ConsultaTipoProdutoViewModel TipoProduto { get; set; }
        public int QtdImpressoes { get; set; }
        public string Link { get; set; }
    }
}
