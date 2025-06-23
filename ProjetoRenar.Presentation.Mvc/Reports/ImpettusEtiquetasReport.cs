using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using ProjetoRenar.Application.ViewModels.Produtos;
using ProjetoRenar.Application.ViewModels.Unidades;
using ProjetoRenar.Domain.Entities;
using ProjetoRenar.Presentation.Mvc.Areas.App.Controllers;
using QRCoder;

namespace ProjetoRenar.Presentation.Mvc.Reports
{
    public class ImpettusEtiquetasReport
    {
        public static byte[] ImprimirEtiqueta(List<ImpettusProdutoImpressaoModel> produtos)
        {
            string outputFile = Guid.NewGuid() + ".pdf";
            float width = MillimetersToPoints(65);
            float height = MillimetersToPoints(65);

            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outputFile));
            Document document = new Document(pdfDoc, new PageSize(width, height));

            document.SetMargins(5, 5, 5, 5); 
            Imprimir(pdfDoc, document, width, produtos);
            document.Close();

            byte[] pdfBytes = System.IO.File.ReadAllBytes(outputFile);
            System.IO.File.Delete(outputFile);

            return pdfBytes;
        }

        public static void Imprimir(PdfDocument pdf, Document document, float width, List<ImpettusProdutoImpressaoModel> produtos)
        {
            for (int p = 0; p < produtos.Count; p++)
            {
                var item = produtos[p];
                for (int i = 1; i <= item.QtdImpressoes; i++)
                {
                    bool isNotLast = !(p == produtos.Count - 1 && i == item.QtdImpressoes);
                    try { AddContent(pdf, document, width, item, isNotLast); } catch (Exception e) { }
                }
            }
        }

        static void AddContent(PdfDocument pdf, Document document, float width, ImpettusProdutoImpressaoModel produto, bool quebraDePagina)
        {
            string caminhoParaArial = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts", "Arial.ttf");
            var fonteArial = iText.Kernel.Font.PdfFontFactory.CreateFont(caminhoParaArial, "Identity-H", true);

            // Título
            document.Add(new Paragraph(produto.Nome.ToUpper())
                .SetFont(fonteArial).SetFontSize(12).SetBold().SetTextAlignment(TextAlignment.LEFT).SetMultipliedLeading(0.9f).SetMarginBottom(0));

            // Modo
            var modos = new List<string>();
            if (produto.FlagResfriado) modos.Add("RESFRIADO");
            if (produto.FlagCongelado) modos.Add("CONGELADO");
            if (produto.FlagTemperaturaAmbiente) modos.Add("TEMPERATURA AMBIENTE");
            string modo = string.Join(" | ", modos);

            document.Add(new Paragraph(modo)
                .SetFont(fonteArial).SetFontSize(7).SetTextAlignment(TextAlignment.LEFT).SetMarginTop(0).SetMarginBottom(1));

            var linha = new LineSeparator(new SolidLine(0.5f));
            document.Add(linha.SetMarginBottom(4).SetMarginTop(2));

            if(produto.FlagResfriado)
            {
                if(produto.TipoValidadeResfriado.Equals("D"))
                {
                    document.Add(new Paragraph("MANIPULAÇÃO: \t\t" + DateTime.Now.ToString("dd/MM/yy"))
                        .SetFont(fonteArial).SetFontSize(9).SetBold().SetMarginBottom(1).SetMarginTop(1).SetPadding(0).SetTextAlignment(TextAlignment.LEFT));

                    document.Add(new Paragraph("VALIDADE: \t\t\t\t" + (DateTime.Now.AddDays(produto.ValidadeResfriado.Value).ToString("dd/MM/yy")))
                        .SetFont(fonteArial).SetFontSize(9).SetBold().SetMarginBottom(1).SetMarginTop(-2).SetPadding(0).SetTextAlignment(TextAlignment.LEFT));
                }
                else if(produto.TipoValidadeResfriado.Equals("H"))
                {
                    document.Add(new Paragraph("MANIPULAÇÃO: \t\t" + DateTime.Now.ToString("dd/MM/yy - HH'H'mm"))
                        .SetFont(fonteArial).SetFontSize(9).SetBold().SetMarginBottom(1).SetMarginTop(1).SetPadding(0).SetTextAlignment(TextAlignment.LEFT));

                    document.Add(new Paragraph("VALIDADE: \t\t\t\t" + (DateTime.Now.AddHours(produto.ValidadeResfriado.Value).ToString("dd/MM/yy - HH'H'mm")))
                        .SetFont(fonteArial).SetFontSize(9).SetBold().SetMarginBottom(1).SetMarginTop(-2).SetPadding(0).SetTextAlignment(TextAlignment.LEFT));
                }
            }
            else if (produto.FlagCongelado)
            {
                document.Add(new Paragraph("MANIPULAÇÃO: \t\t" + DateTime.Now.ToString("dd/MM/yy"))
                       .SetFont(fonteArial).SetFontSize(9).SetBold().SetMarginBottom(1).SetMarginTop(1).SetPadding(0).SetTextAlignment(TextAlignment.LEFT));

                document.Add(new Paragraph("VALIDADE: \t\t\t\t" + (DateTime.Now.AddDays(produto.ValidadeCongelado.Value).ToString("dd/MM/yy")))
                    .SetFont(fonteArial).SetFontSize(9).SetBold().SetMarginBottom(1).SetMarginTop(-2).SetPadding(0).SetTextAlignment(TextAlignment.LEFT));
            }
            else if(produto.FlagTemperaturaAmbiente)
            {
                if (produto.TipoValidadeResfriado.Equals("D"))
                {
                    document.Add(new Paragraph("MANIPULAÇÃO: \t\t" + DateTime.Now.ToString("dd/MM/yy"))
                        .SetFont(fonteArial).SetFontSize(9).SetBold().SetMarginBottom(1).SetMarginTop(1).SetPadding(0).SetTextAlignment(TextAlignment.LEFT));

                    document.Add(new Paragraph("VALIDADE: \t\t\t\t" + (DateTime.Now.AddDays(produto.ValidadeTemperaturaAmbiente.Value).ToString("dd/MM/yy")))
                        .SetFont(fonteArial).SetFontSize(9).SetBold().SetMarginBottom(1).SetMarginTop(-2).SetPadding(0).SetTextAlignment(TextAlignment.LEFT));
                }
                else if (produto.TipoValidadeResfriado.Equals("H"))
                {
                    document.Add(new Paragraph("MANIPULAÇÃO: \t\t" + DateTime.Now.ToString("dd/MM/yy - HH'H'mm"))
                        .SetFont(fonteArial).SetFontSize(9).SetBold().SetMarginBottom(1).SetMarginTop(1).SetPadding(0).SetTextAlignment(TextAlignment.LEFT));

                    document.Add(new Paragraph("VALIDADE: \t\t\t\t" + (DateTime.Now.AddHours(produto.ValidadeTemperaturaAmbiente.Value).ToString("dd/MM/yy - HH'H'mm")))
                        .SetFont(fonteArial).SetFontSize(9).SetBold().SetMarginBottom(1).SetMarginTop(-2).SetPadding(0).SetTextAlignment(TextAlignment.LEFT));
                }
            }            
                            
            document.Add(linha.SetMarginBottom(4));

            // Demais informações
            document.Add(new Paragraph("RESP: DANIEL").SetFont(fonteArial).SetFontSize(8).SetBold().SetMarginTop(1).SetTextAlignment(TextAlignment.LEFT));
            document.Add(new Paragraph("MANE NITERÓI").SetFont(fonteArial).SetFontSize(6.5f).SetMargin(0).SetTextAlignment(TextAlignment.LEFT));
            document.Add(new Paragraph("CNPJ: 44.021.725/0001-31").SetFont(fonteArial).SetFontSize(6.5f).SetMargin(0).SetTextAlignment(TextAlignment.LEFT));
            document.Add(new Paragraph("CEP: 24360-022 AVENIDA QUINTINO 185, NITERÓI RJ").SetFont(fonteArial).SetFontSize(6.5f).SetMargin(0).SetTextAlignment(TextAlignment.LEFT));

            // … (seu conteúdo anterior permanece igual, mas remova o document.Add(barcodeImage); antes da parte que trata o barcode)

            var code = Guid.NewGuid().ToString();
            var barcode = new iText.Barcodes.Barcode128(pdf);
            barcode.SetCode(code);
            barcode.SetCodeType(iText.Barcodes.Barcode128.CODE128);
            var barcodeXObject = barcode.CreateFormXObject(pdf);
            var barcodeImage = new iText.Layout.Element.Image(barcodeXObject)
                .SetWidth(170)
                .SetHeight(25);

            // === Posicionar o barcode manualmente no fim da etiqueta ===
            // Converta 6.5cm para pontos (72 pts por polegada; 1cm ≈ 28.35pt)
            float etiquetaAltura = 6.5f * 28.35f;
            float margemInferior = 10; // 10 pontos (~0.35 cm) da borda inferior
            float margemEsquerda = (etiquetaAltura - 170) / 2; // centralizar horizontalmente
            float yPosicao = margemInferior; // fixa a 10pt da borda inferior da página

            // Obtém o canvas da página atual
            PdfPage paginaAtual = pdf.GetLastPage();
            PdfCanvas pdfCanvas = new PdfCanvas(paginaAtual);
            var canvas = new Canvas(pdfCanvas, new iText.Kernel.Geom.Rectangle(0, 0, etiquetaAltura, etiquetaAltura));
            canvas.Add(barcodeImage.SetFixedPosition(margemEsquerda, yPosicao));
            canvas.Close();

            // === Fim do barcode ===

            // Por fim quebra de página se necessário
            if (quebraDePagina)
                document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

        }

        static float MillimetersToPoints(float mm)
        {
            return mm * 2.83465f;
        }
    }
}
