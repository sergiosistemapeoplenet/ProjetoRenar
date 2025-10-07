using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using iText.Barcodes;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Newtonsoft.Json;
using ProjetoRenar.Application.ViewModels.Produtos;
using ProjetoRenar.Application.ViewModels.Unidades;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Entities;
using ProjetoRenar.Presentation.Mvc.Areas.App.Controllers;
using QRCoder;

namespace ProjetoRenar.Presentation.Mvc.Reports
{
    public class ImpettusEtiquetasReport
    {
        public static byte[] ImprimirEtiqueta(List<ImpettusProdutoImpressaoModel> produtos, ConsultaUnidadeViewModel unidade, IUnitOfWork unitOfWork)
        {
            string outputFile = Guid.NewGuid() + ".pdf";
            float width = MillimetersToPoints(60);
            float height = MillimetersToPoints(60);

            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outputFile));
            Document document = new Document(pdfDoc, new PageSize(width, height));

            document.SetMargins(4, 2, 0, 6); 
            Imprimir(pdfDoc, document, width, produtos, unidade, unitOfWork);
            document.Close();

            byte[] pdfBytes = System.IO.File.ReadAllBytes(outputFile);
            System.IO.File.Delete(outputFile);

            return pdfBytes;
        }

        public static void Imprimir(PdfDocument pdf, Document document, float width, List<ImpettusProdutoImpressaoModel> produtos, ConsultaUnidadeViewModel unidade, IUnitOfWork unitOfWork)
        {
            for (int p = 0; p < produtos.Count; p++)
            {
                var item = produtos[p];
                for (int i = 1; i <= item.QtdImpressoes; i++)
                {
                    bool isNotLast = !(p == produtos.Count - 1 && i == item.QtdImpressoes);
                    try { AddContent(pdf, document, width, item, isNotLast, unidade, unitOfWork); } catch (Exception e) { }
                }
            }
        }

        static void AddContent(PdfDocument pdf, Document document, float width, ImpettusProdutoImpressaoModel produto, bool quebraDePagina, ConsultaUnidadeViewModel unidade, IUnitOfWork unitOfWork)
        {
            string caminhoParaArial = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts", "Arial.ttf");
            var fonteArial = iText.Kernel.Font.PdfFontFactory.CreateFont(caminhoParaArial, "Identity-H", true);

            // Título
            document.Add(new Paragraph(produto.Nome.ToUpper())
                .SetFont(fonteArial).SetFontSize(11).SetBold().SetTextAlignment(TextAlignment.LEFT).SetMultipliedLeading(0.9f).SetMarginBottom(0));

            // Modo
            var modos = new List<string>();
            if (produto.FlagResfriado) modos.Add("RESFRIADO");
            if (produto.FlagCongelado) modos.Add("CONGELADO");
            if (produto.FlagTemperaturaAmbiente) modos.Add("TEMPERATURA AMBIENTE");
            string modo = string.Join(" | ", modos);

            document.Add(new Paragraph(modo)
                .SetFont(fonteArial).SetFontSize(8).SetTextAlignment(TextAlignment.LEFT).SetMarginTop(0).SetMarginBottom(1));

            if(!string.IsNullOrEmpty(produto.Sif))
            {
                document.Add(new Paragraph("SIF:  " + produto.Sif)
                .SetFont(fonteArial).SetFontSize(6).SetTextAlignment(TextAlignment.LEFT).SetMarginTop(0).SetMarginBottom(-1));
            }

            if (!string.IsNullOrEmpty(produto.Lote))
            {
                document.Add(new Paragraph("Lote: " + produto.Lote)
                .SetFont(fonteArial).SetFontSize(6).SetTextAlignment(TextAlignment.LEFT).SetMarginTop(0).SetMarginBottom(-1));
            }

            if (!string.IsNullOrEmpty(produto.Quantidade))
            {
                document.Add(new Paragraph("Quantidade: " + produto.Quantidade)
                .SetFont(fonteArial).SetFontSize(6).SetTextAlignment(TextAlignment.LEFT).SetMarginTop(0).SetMarginBottom(-1));
            }

            var linha = new LineSeparator(new SolidLine(0.5f));
            document.Add(linha.SetMarginBottom(1).SetMarginTop(1));

            if(produto.FlagResfriado)
            {
                if(produto.TipoValidadeResfriado != null && produto.TipoValidadeResfriado.Equals("D"))
                {
                    document.Add(new Paragraph("MANIPULAÇÃO: \t\t" + DateTime.Now.ToString("dd/MM/yyyy"))
                        .SetFont(fonteArial).SetFontSize(8).SetBold().SetMarginBottom(1).SetMarginTop(1).SetPadding(0).SetTextAlignment(TextAlignment.LEFT));

                    document.Add(new Paragraph("VALIDADE: \t\t\t\t" + (DateTime.Now.AddDays(produto.ValidadeResfriado.Value).ToString("dd/MM/yyyy")))
                        .SetFont(fonteArial).SetFontSize(8).SetBold().SetMarginBottom(1).SetMarginTop(-2).SetPadding(0).SetTextAlignment(TextAlignment.LEFT));
                }
                else if(produto.TipoValidadeResfriado != null && produto.TipoValidadeResfriado.Equals("H"))
                {
                    document.Add(new Paragraph("MANIPULAÇÃO: \t\t" + DateTime.Now.ToString("dd/MM/yyyy - HH'H'mm"))
                        .SetFont(fonteArial).SetFontSize(8).SetBold().SetMarginBottom(1).SetMarginTop(1).SetPadding(0).SetTextAlignment(TextAlignment.LEFT));

                    document.Add(new Paragraph("VALIDADE: \t\t\t\t" + (DateTime.Now.AddHours(produto.ValidadeResfriado.Value).ToString("dd/MM/yyyy - HH'H'mm")))
                        .SetFont(fonteArial).SetFontSize(8).SetBold().SetMarginBottom(1).SetMarginTop(-2).SetPadding(0).SetTextAlignment(TextAlignment.LEFT));
                }
            }
            else if (produto.FlagCongelado)
            {
                document.Add(new Paragraph("MANIPULAÇÃO: \t\t" + DateTime.Now.ToString("dd/MM/yyyy"))
                       .SetFont(fonteArial).SetFontSize(8).SetBold().SetMarginBottom(1).SetMarginTop(1).SetPadding(0).SetTextAlignment(TextAlignment.LEFT));

                document.Add(new Paragraph("VALIDADE: \t\t\t\t" + (DateTime.Now.AddDays(produto.ValidadeCongelado.Value).ToString("dd/MM/yyyy")))
                    .SetFont(fonteArial).SetFontSize(8).SetBold().SetMarginBottom(1).SetMarginTop(-2).SetPadding(0).SetTextAlignment(TextAlignment.LEFT));
            }
            else if(produto.FlagTemperaturaAmbiente)
            {
                if (produto.TipoValidadeTemperaturaAmbiente != null && produto.TipoValidadeTemperaturaAmbiente.Equals("D"))
                {
                    document.Add(new Paragraph("MANIPULAÇÃO: \t\t" + DateTime.Now.ToString("dd/MM/yyyy"))
                        .SetFont(fonteArial).SetFontSize(8).SetBold().SetMarginBottom(1).SetMarginTop(1).SetPadding(0).SetTextAlignment(TextAlignment.LEFT));

                    document.Add(new Paragraph("VALIDADE: \t\t\t\t" + (DateTime.Now.AddDays(produto.ValidadeTemperaturaAmbiente.Value).ToString("dd/MM/yyyy")))
                        .SetFont(fonteArial).SetFontSize(8).SetBold().SetMarginBottom(1).SetMarginTop(-2).SetPadding(0).SetTextAlignment(TextAlignment.LEFT));
                }
                else if (produto.TipoValidadeTemperaturaAmbiente != null && produto.TipoValidadeTemperaturaAmbiente.Equals("H"))
                {
                    document.Add(new Paragraph("MANIPULAÇÃO: \t\t" + DateTime.Now.ToString("dd/MM/yyyy - HH'H'mm"))
                        .SetFont(fonteArial).SetFontSize(8).SetBold().SetMarginBottom(1).SetMarginTop(1).SetPadding(0).SetTextAlignment(TextAlignment.LEFT));

                    document.Add(new Paragraph("VALIDADE: \t\t\t\t" + (DateTime.Now.AddHours(produto.ValidadeTemperaturaAmbiente.Value).ToString("dd/MM/yyyy - HH'H'mm")))
                        .SetFont(fonteArial).SetFontSize(8).SetBold().SetMarginBottom(1).SetMarginTop(-2).SetPadding(0).SetTextAlignment(TextAlignment.LEFT));
                }
            }            
                            
            document.Add(linha.SetMarginBottom(2));

            if (string.IsNullOrEmpty(produto.Lote) && string.IsNullOrEmpty(produto.Sif))
            {
                document.Add(new Paragraph("\n")
                .SetFont(fonteArial).SetFontSize(6).SetTextAlignment(TextAlignment.LEFT).SetMarginTop(0).SetMarginBottom(1));
            }

            // Demais informações
            document.Add(new Paragraph($"RESP: {unidade.NomeContato.ToUpper()}").SetFont(fonteArial).SetFontSize(5f).SetMargin(0).SetBold().SetMarginTop(1).SetTextAlignment(TextAlignment.LEFT));
            document.Add(new Paragraph(unidade.NomeUnidade.ToUpper()).SetFont(fonteArial).SetFontSize(5f).SetMargin(0).SetTextAlignment(TextAlignment.LEFT));
            document.Add(new Paragraph($"CNPJ: " + unidade.CNPJ).SetFont(fonteArial).SetFontSize(5f).SetMargin(0).SetTextAlignment(TextAlignment.LEFT));
            document.Add(new Paragraph($"{unidade.Endereco}" + $" - CEP: {unidade.Cep}").SetFont(fonteArial).SetFontSize(5f).SetMargin(0).SetTextAlignment(TextAlignment.LEFT));

            // … (seu conteúdo anterior permanece igual, mas remova o document.Add(barcodeImage); antes da parte que trata o barcode)

            var code = produto.Id.ToString();
            var barcode = new iText.Barcodes.Barcode128(pdf);
            barcode.SetCode(code);
            barcode.SetCodeType(iText.Barcodes.Barcode128.CODE128);
            var barcodeXObject = barcode.CreateFormXObject(pdf);
            var barcodeImage = new iText.Layout.Element.Image(barcodeXObject)
                .SetWidth(60)
                .SetHeight(30);

            // Obtém o canvas da página atual
            PdfPage paginaAtual = pdf.GetLastPage();
            PdfCanvas pdfCanvas = new PdfCanvas(paginaAtual);
            var canvas = new Canvas(pdfCanvas, new iText.Kernel.Geom.Rectangle(0, 0, 60, 60));
            canvas.Add(barcodeImage.SetFixedPosition(55, 8));
            canvas.Close();

            // === Fim do barcode ===

            // Por fim quebra de página se necessário
            if (quebraDePagina)
                document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

            var etiqueta = new EtiquetaProdutoDTO
            {
                IdProduto = produto.Id,
                NomeProduto = produto.Nome.ToUpper(),
                FlagPreparacao = produto.FlagPreparacao,
                FlagProduto = produto.FlagProduto,
                SIF = produto.Sif != null ? produto.Sif.ToString() : string.Empty,
                Lote = produto.Lote != null ? produto.Lote.ToString() : string.Empty,
                Quantidade = produto.Quantidade != null ? produto.Quantidade.ToString() : string.Empty,
                ModosConservacao = new List<string>
                {
                    produto.FlagResfriado ? "RESFRIADO" : null,
                    produto.FlagCongelado ? "CONGELADO" : null,
                    produto.FlagTemperaturaAmbiente ? "TEMPERATURA AMBIENTE" : null
                }.Where(x => x != null).ToList(),
                            TipoValidade = produto.FlagResfriado ? produto.TipoValidadeResfriado :
                               produto.FlagCongelado ? "D" :
                               produto.TipoValidadeTemperaturaAmbiente,
                            DataManipulacao = produto.TipoValidadeResfriado == "H" || produto.TipoValidadeTemperaturaAmbiente == "H"
                    ? DateTime.Now.ToString("dd/MM/yyyy - HH'H'mm")
                    : DateTime.Now.ToString("dd/MM/yyyy"),
                            DataValidade = produto.FlagResfriado && produto.TipoValidadeResfriado == "H"
                    ? DateTime.Now.AddHours(produto.ValidadeResfriado.Value).ToString("dd/MM/yyyy - HH'H'mm")
                    : produto.FlagResfriado
                        ? DateTime.Now.AddDays(produto.ValidadeResfriado.Value).ToString("dd/MM/yyyy")
                    : produto.FlagCongelado
                        ? DateTime.Now.AddDays(produto.ValidadeCongelado.Value).ToString("dd/MM/yyyy")
                    : produto.TipoValidadeTemperaturaAmbiente == "H"
                        ? DateTime.Now.AddHours(produto.ValidadeTemperaturaAmbiente.Value).ToString("dd/MM/yyyy - HH'H'mm")
                        : DateTime.Now.AddDays(produto.ValidadeTemperaturaAmbiente.Value).ToString("dd/MM/yyyy"),
                            Responsavel = unidade.NomeContato.ToUpper(),
                            NomeUnidade = unidade.NomeUnidade.ToUpper(),
                            CNPJ = unidade.CNPJ,
                            EnderecoCompleto = $"CEP: {unidade.Cep} - {unidade.Endereco}",
                            CodigoBarras = produto.Id.ToString()
                        };

            var json = JsonConvert.SerializeObject(etiqueta);
            unitOfWork.ImpettusProdutoRepository.AdicionarControleEtiqueta(DateTime.Now, json);
        }

        public class EtiquetaProdutoDTO
        {
            public int IdProduto { get; set; }
            public string NomeProduto { get; set; }
            public List<string> ModosConservacao { get; set; } = new List<string>();
            public string TipoValidade { get; set; } // "D" ou "H"
            public string DataManipulacao { get; set; }
            public string DataValidade { get; set; }
            public string Responsavel { get; set; }
            public string NomeUnidade { get; set; }
            public string CNPJ { get; set; }
            public string EnderecoCompleto { get; set; }
            public string CodigoBarras { get; set; }
            public bool FlagProduto { get; set; }
            public bool FlagPreparacao { get; set; }
            public string SIF { get; set; }
            public string Lote { get; set; }
            public string Quantidade { get; set; }
        }


        static float MillimetersToPoints(float mm)
        {
            return mm * 2.83465f;
        }
    }
}
