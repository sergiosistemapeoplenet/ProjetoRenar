using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using ProjetoRenar.Application.ViewModels.Produtos;
using ProjetoRenar.Application.ViewModels.Unidades;

namespace ProjetoRenar.Presentation.Mvc.Reports
{
    public class EtiquetasReportOld2
    {
        private static ConsultaUnidadeViewModel _unidade;

        public static byte[] ImprimirEtiqueta(TipoLayoutEtiqueta tipo, List<ConsultaProdutoViewModel> produtos, ConsultaUnidadeViewModel unidade)
        {
            _unidade = unidade;

            switch (tipo)
            {
                case TipoLayoutEtiqueta.Bolo_1Coluna: return ImprimirBolo1Coluna(produtos);
                case TipoLayoutEtiqueta.BoloPote_2Colunas: return ImprimirBoloPote2Colunas(produtos);
                case TipoLayoutEtiqueta.Fatia_2Colunas: return ImprimirFatia2Colunas(produtos);
            }

            return null;
        }

        #region Bolo de pote 1 coluna


        public static byte[] ImprimirBolo1Coluna(List<ConsultaProdutoViewModel> produtos)
        {
            string outputFile = Guid.NewGuid() + ".pdf";
            float width = MillimetersToPoints(101); // Largura em milímetros convertida para pontos
            float height = MillimetersToPoints(50); // Altura em milímetros convertida para pontos

            // Criação de um novo documento PDF com margens zero
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outputFile));
            Document document = new Document(pdfDoc, new PageSize(width, height));
            document.SetMargins(-2.5f, 0, 0, 16); // Definindo margens como zero

            foreach (var item in produtos)
            {
                for (int i = 1; i <= item.QtdImpressoes; i++)
                {
                    AddContentBolo1Coluna(document, width, item);
                }
            }

            document.Close();

            // Converte o arquivo PDF em um array de bytes
            byte[] pdfBytes = System.IO.File.ReadAllBytes(outputFile);
            // Deleta o arquivo PDF temporário
            System.IO.File.Delete(outputFile);

            return pdfBytes;
        }

        public static byte[] ImprimirBoloPote2Colunas(List<ConsultaProdutoViewModel> produtos)
        {
            string outputFile = Guid.NewGuid() + ".pdf";
            float width = MillimetersToPoints(102); // Largura em milímetros convertida para pontos
            float height = MillimetersToPoints(50); // Altura em milímetros convertida para pontos

            // Criação de um novo documento PDF com margens zero
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outputFile));
            Document document = new Document(pdfDoc, new PageSize(width, height));
            document.SetMargins(0, 0, 0, 10); // Definindo margens como zero

            foreach (var item in produtos)
            {
                int limite = item.QtdImpressoes % 2 == 0 ? item.QtdImpressoes : item.QtdImpressoes - 1;
                for (int i = 1; i <= limite; i += 2)
                {
                    AddContentBoloPote2Colunas(document, width, item, item);
                }

                if (item.QtdImpressoes % 2 != 0)
                {
                    AddContentBoloPote2Colunas(document, width, item, null);
                }
            }

            document.Close();

            // Converte o arquivo PDF em um array de bytes
            byte[] pdfBytes = System.IO.File.ReadAllBytes(outputFile);
            // Deleta o arquivo PDF temporário
            System.IO.File.Delete(outputFile);

            return pdfBytes;
        }

        public static byte[] ImprimirFatia2Colunas(List<ConsultaProdutoViewModel> produtos)
        {
            string outputFile = Guid.NewGuid() + ".pdf";
            float width = MillimetersToPoints(100); // Largura em milímetros convertida para pontos
            float height = MillimetersToPoints(50); // Altura em milímetros convertida para pontos

            // Criação de um novo documento PDF com margens zero
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outputFile));
            Document document = new Document(pdfDoc, new PageSize(width, height));
            document.SetMargins(0, 3, 0, 3); // Definindo margens como zero

            foreach (var item in produtos)
            {
                int limite = item.QtdImpressoes % 2 == 0 ? item.QtdImpressoes : item.QtdImpressoes - 1;
                for (int i = 1; i <= limite; i += 2)
                {
                    AddContentFatia2Colunas(document, width, item, item);
                }

                if (item.QtdImpressoes % 2 != 0)
                {
                    AddContentFatia2Colunas(document, width, item, null);
                }
            }

            document.Close();

            // Converte o arquivo PDF em um array de bytes
            byte[] pdfBytes = System.IO.File.ReadAllBytes(outputFile);
            // Deleta o arquivo PDF temporário
            System.IO.File.Delete(outputFile);

            return pdfBytes;
        }

        static void AddContentBolo1Coluna(Document document, float width, ConsultaProdutoViewModel produto)
        {
            string caminhoParaArial = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts", "Arial.ttf");
            var fonteArial = iText.Kernel.Font.PdfFontFactory.CreateFont(caminhoParaArial, "Identity-H", true);

            // Adiciona um título alinhado à esquerda
            Paragraph title = new Paragraph()
                .Add(new Text(produto.NomeProduto.ToUpper()).SetBold())
                .SetTextAlignment(TextAlignment.LEFT)
                .SetMarginTop(2)
                .SetFontSize(6).SetFont(fonteArial);

            document.Add(title);

            Paragraph pesoParagrafo = new Paragraph($"Peso Líquido: {produto.Peso}")
                .SetTextAlignment(TextAlignment.RIGHT).SetBold()
                .SetFontSize(4.5f).SetFont(fonteArial).SetCharacterSpacing(0.8f)
                .SetMarginTop(-5); // Definindo uma margem superior menor para o parágrafo

            document.Add(pesoParagrafo);

            // Adiciona a tabela
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 50, 6, 42 }))
                .UseAllAvailableWidth()
                .SetBorder(Border.NO_BORDER)
                .SetMargin(-4);

            var col1pg1 = new Paragraph("INFORMAÇÃO NUTRICIONAL").SetCharacterSpacing(0.5f).SetFontSize(4f).SetBold().SetMarginTop(-1).SetFont(fonteArial).SetMultipliedLeading(1f).SetTextAlignment(TextAlignment.CENTER);
            var col1pg2 = new Paragraph(produto.PorcaoEmbalagem).SetCharacterSpacing(0.5f).SetFontSize(4f).SetBold().SetMarginTop(0).SetFont(fonteArial).SetMultipliedLeading(1f).SetTextAlignment(TextAlignment.CENTER);
            var col1pg3 = new Paragraph(produto.PorcaoFatia).SetCharacterSpacing(0.5f).SetFontSize(4f).SetBold().SetMarginTop(0).SetFont(fonteArial).SetMultipliedLeading(1f).SetTextAlignment(TextAlignment.CENTER);

            var maisQuebrasDeLinha = "\n";
            if (produto.Receita.Length <= 100)
                maisQuebrasDeLinha += "\n";

            var col3pg1 = new Paragraph((!string.IsNullOrEmpty(produto.Info3) ? produto.Info3 + "\n\n" : "") + produto.Receita + "\n\n").SetCharacterSpacing(0.5f).SetFontSize(4f).SetFont(fonteArial).SetMultipliedLeading(1f).SetBold();
            var col3pg2 = new Paragraph(produto.Info1 + maisQuebrasDeLinha).SetCharacterSpacing(0.5f).SetFontSize(4f).SetFont(fonteArial).SetMultipliedLeading(1f).SetBold();
            var col3pg3 = new Paragraph("Fabricado e Distribuído por: ").SetCharacterSpacing(0.5f).SetBold().SetFontSize(3.5f).SetFont(fonteArial).SetMultipliedLeading(1f);
            var col3pg4 = new Paragraph(_unidade.RazaoSocial + " - " + _unidade.Endereco + " - CNPJ: " + _unidade.CNPJ).SetFontSize(3.5f).SetCharacterSpacing(0.5f).SetFont(fonteArial).SetMultipliedLeading(1f).SetBold();
            var telefoneUnidade = _unidade.NumeroContato.Contains("/") ? _unidade.NumeroContato.Split("/")[0] : _unidade.NumeroContato;
            var col3pg5 = new Paragraph("Tel: " + telefoneUnidade).SetMarginRight(2).SetCharacterSpacing(0.5f).SetFontSize(5f).SetTextAlignment(TextAlignment.RIGHT).SetFont(fonteArial).SetMultipliedLeading(1f).SetBold();

            if (produto.TipoProduto.FlagAltoAcucar == 0 && produto.TipoProduto.FlagAltoGorduraSaturada == 0)
            {
                col3pg1 = new Paragraph((!string.IsNullOrEmpty(produto.Info3) ? produto.Info3 + "\n\n" : "") + produto.Receita + "\n\n").SetCharacterSpacing(0.5f).SetFontSize(4f).SetFont(fonteArial).SetMultipliedLeading(1f).SetBold();
                col3pg2 = new Paragraph(produto.Info1).SetCharacterSpacing(0.5f).SetFontSize(4f).SetFont(fonteArial).SetMultipliedLeading(1f).SetBold();
                col3pg3 = new Paragraph("\n\nFabricado e Distribuído por: ").SetCharacterSpacing(0.5f).SetBold().SetFontSize(3.5f).SetFont(fonteArial).SetMultipliedLeading(1f);
                col3pg4 = new Paragraph(_unidade.RazaoSocial + " - " + _unidade.Endereco + " - CNPJ: " + _unidade.CNPJ).SetFontSize(3.5f).SetCharacterSpacing(0.5f).SetFont(fonteArial).SetMultipliedLeading(1f).SetBold();
                telefoneUnidade = _unidade.NumeroContato.Contains("/") ? _unidade.NumeroContato.Split("/")[0] : _unidade.NumeroContato;
                col3pg5 = new Paragraph("Tel: " + telefoneUnidade).SetMarginRight(2).SetCharacterSpacing(0.5f).SetFontSize(5f).SetTextAlignment(TextAlignment.RIGHT).SetFont(fonteArial).SetMultipliedLeading(1f).SetBold();

            }

            if(produto.TipoProduto.FlagAltoAcucar == 1 && produto.TipoProduto.FlagAltoGorduraSaturada == 1)
            {
                string imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "alto_acucar_gordura_horizontal.png");
                Image image = new Image(ImageDataFactory.Create(imagePath));
                float scaleFactor = 0.70f; // Reduz em 25%

                image.SetWidth(MillimetersToPoints(41.0f * scaleFactor));
                image.SetHeight(MillimetersToPoints(7.5f * scaleFactor));

                col3pg2.Add(image);

                if (produto.Receita.Length <= 280)
                    col3pg2.Add(new Paragraph("\n"));
            }
            else if (produto.TipoProduto.FlagAltoAcucar == 1)
            {
                string imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "alto_acucar_horizontal.png");
                Image image = new Image(ImageDataFactory.Create(imagePath));
                float scaleFactor = 0.80f; // Reduz em 25%

                image.SetWidth(MillimetersToPoints(28.5f * scaleFactor));
                image.SetHeight(MillimetersToPoints(6f * scaleFactor));
                //float imageX = 195;
                //float imageY = 47;
                //image.SetFixedPosition(imageX, imageY);

                col3pg2.Add(image);

                if (produto.Receita.Length <= 280)
                    col3pg2.Add(new Paragraph("\n"));
            }
            else if (produto.TipoProduto.FlagAltoGorduraSaturada == 1)
            {
                string imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "alto_gordura_horizontal.png");
                Image image = new Image(ImageDataFactory.Create(imagePath));
                float scaleFactor = 0.80f; // Reduz em 25%

                image.SetWidth(MillimetersToPoints(28.5f * scaleFactor));
                image.SetHeight(MillimetersToPoints(6f * scaleFactor));

                col3pg2.Add(image);

                if (produto.Receita.Length <= 280)
                    col3pg2.Add(new Paragraph("\n"));
            }

            Table tableNutricional = new Table(UnitValue.CreatePercentArray(new float[] { 57, 14, 15, 14 }))
                .UseAllAvailableWidth()
                .SetBorder(Border.NO_BORDER) // Remove a borda da tabela
                .SetMarginTop(1)
                .SetMarginLeft(-2)
                .SetMarginRight(0);


            if (produto.AcucarTotalValorDiario == null) produto.AcucarTotalValorDiario = 0m;
            if (produto.Gtotvd == null) produto.Gtotvd = 0m;
            if (produto.Info1 == null) produto.Info1 = string.Empty;
            if (produto.Info2 == null) produto.Info2 = string.Empty;

            tableNutricional.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).SetBorderTop(new SolidBorder(1.5f)).Add(new Paragraph("")));
            tableNutricional.AddCell(new Cell().SetBorderTop(new SolidBorder(1.5f)).Add(new Paragraph("100g").SetFontSize(4.8f).SetCharacterSpacing(0.6f).SetBold().SetMargin(-2).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.0f)));
            tableNutricional.AddCell(new Cell().SetBorderTop(new SolidBorder(1.5f)).Add(new Paragraph("60g").SetFontSize(4.8f).SetCharacterSpacing(0.6f).SetBold().SetMargin(-2).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f)));
            tableNutricional.AddCell(new Cell().SetBorderTop(new SolidBorder(1.5f)).SetBorderRight(Border.NO_BORDER).Add(new Paragraph("%VD").SetFontSize(4.8f).SetCharacterSpacing(0.6f).SetBold().SetMargin(-2).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f)));

            tableNutricional.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Valor Energ. (Kcal)").SetCharacterSpacing(0.8f).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(-1f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto.ValorEnergetico)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto.PorcaoValorEnergetico)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto.ValorEnergeticoValorDiario)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));

            tableNutricional.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Carboidratos (g)").SetCharacterSpacing(0.8f).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(-1f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto.Carboidratos)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto.PorcaoCarboidratos)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto.CarboidratosValorDiario)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));

            tableNutricional.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Açúcares Totais (g)").SetCharacterSpacing(0.8f).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto.AcucarTotal)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto.PorcaoAcucarTotal)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
            if(produto.AcucarTotalValorDiario != null && produto.AcucarTotalValorDiario != 0)
                tableNutricional.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto.AcucarTotalValorDiario)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
            else
                tableNutricional.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph("").SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

            tableNutricional.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Açúcares Adicionados (g)").SetCharacterSpacing(0.8f).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(3f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto.AcucarAdicionado)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto.PorcaoAcucarAdicionado)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto.AcucarAdicionadoValorDiario)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));

            tableNutricional.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Proteínas (g)").SetCharacterSpacing(0.8f).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(-1f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto.Proteina)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto.PorcaoProteina)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto.ProteinaValorDiario)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                       
            tableNutricional.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Gorduras Totais (g)").SetCharacterSpacing(0.8f).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(-1f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto.GorduraTotal)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto.PorcaoGorduraTotal)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto.Gtotvd)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));

            tableNutricional.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Gord Saturadas (g)").SetCharacterSpacing(0.8f).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(3f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto.GorduraSaturada)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto.PorcaoGorduraSaturada)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto.GorduraSaturadaValorDiario)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));

            tableNutricional.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Gorduras Trans(g)").SetCharacterSpacing(0.8f).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(3f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto.GorduraTrans)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto.PorcaoGorduraTrans)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto.GorduraTransValorDiario)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                       
            tableNutricional.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Fibra (g)").SetCharacterSpacing(0.8f).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(-1f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto.FibraAlimentar)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto.PorcaoFibraAlimentar)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto.FibraAlimentarValorDiario)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));

            tableNutricional.AddCell(new Cell().SetBorderBottom(new SolidBorder(1.5f)).SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Sódio (mg)").SetCharacterSpacing(0.8f).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(-1f).SetBold()));
            tableNutricional.AddCell(new Cell().SetBorderBottom(new SolidBorder(1.5f)).Add(new Paragraph(FormatarValor(produto.Sodio)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().SetBorderBottom(new SolidBorder(1.5f)).Add(new Paragraph(FormatarValor(produto.PorcaoSodio)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().SetBorderBottom(new SolidBorder(1.5f)).SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto.SodioValorDiario)).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));

            var percentual = new Paragraph(produto.Info2).SetMarginTop(2).SetFontSize(4.8f).SetBold().SetCharacterSpacing(0.2f);

            Paragraph verticalParagraph = new Paragraph($" FAB.: {produto.DataAtual.ToString("dd/MM/yy")} VAL.: {produto.DataValidade.ToString("dd/MM/yy")}")
                .SetRotationAngle(Math.PI / 2) // Rotação de 90 graus (em radianos)
                .SetBold().SetCharacterSpacing(0.2f)
                .SetFontSize(7.8f);

            // Define as coordenadas x e y do parágrafo
            float xPosition = MillimetersToPoints(61f); // Altere conforme necessário
            float yPosition = MillimetersToPoints(5f); // Altere conforme necessário
            verticalParagraph.SetFixedPosition(xPosition, yPosition, width - xPosition);

            document.Add(verticalParagraph);

            // Adiciona as células à tabela
            table.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(col1pg1).Add(col1pg2).Add(col1pg3).Add(tableNutricional).Add(percentual));
            table.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("")));
            table.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(col3pg1).Add(col3pg2).Add(col3pg3).Add(col3pg4).Add(col3pg5));
            
            PdfDocument pdfDocument = document.GetPdfDocument();
            PdfPage page = pdfDocument.GetLastPage();
            if (page == null)
            {
                page = pdfDocument.AddNewPage();
            }

            PdfCanvas canvasLinhaSuperior = new PdfCanvas(page);
            canvasLinhaSuperior.SetLineWidth(0.5f);

            float startX = document.GetLeftMargin() - 5;
            float endX = width - document.GetRightMargin() + 8;
            float yPos = page.GetPageSize().GetTop() - 13.5f;

            canvasLinhaSuperior.MoveTo(startX, yPos)
                  .LineTo(endX, yPos)
                  .Stroke();

            PdfCanvas canvasLinhaInferior = new PdfCanvas(page);
            canvasLinhaInferior.SetLineWidth(0.5f);

            startX = document.GetLeftMargin() - 5;
            endX = width - document.GetRightMargin() + 8;
            yPos = page.GetPageSize().GetTop() -137f;

            canvasLinhaInferior.MoveTo(startX, yPos)
                  .LineTo(endX, yPos)
                  .Stroke();

            PdfCanvas canvasLinhaVertical0 = new PdfCanvas(page);
            canvasLinhaVertical0.SetLineWidth(0.5f);

            // Definindo a posição x para a linha vertical (por exemplo, no centro da página)
            float xPos = (page.GetPageSize().GetLeft() + 10f);

            // Definindo a posição y para o início e o final da linha vertical
            float startY = page.GetPageSize().GetTop() - document.GetTopMargin() - 18;
            float endY = page.GetPageSize().GetBottom() + document.GetBottomMargin() + 7;

            // Desenhando a linha vertical
            canvasLinhaVertical0.MoveTo(xPos, startY)
                               .LineTo(xPos, endY)
                               .Stroke();

            PdfCanvas canvasLinhaVertical1 = new PdfCanvas(page);
            canvasLinhaVertical1.SetLineWidth(0.5f);

            // Definindo a posição x para a linha vertical (por exemplo, no centro da página)
            xPos = (page.GetPageSize().GetLeft() + (page.GetPageSize().GetRight()) / 2) + 12;

            // Definindo a posição y para o início e o final da linha vertical
            startY = page.GetPageSize().GetTop() - document.GetTopMargin() -20;
            endY = page.GetPageSize().GetBottom() + document.GetBottomMargin() + 9;

            // Desenhando a linha vertical
            canvasLinhaVertical1.MoveTo(xPos, startY)
                               .LineTo(xPos, endY)
                               .Stroke();

            PdfCanvas canvasLinhaVertical2 = new PdfCanvas(page);
            canvasLinhaVertical2.SetLineWidth(0.5f);

            // Definindo a posição x para a linha vertical (por exemplo, no centro da página)
            xPos = (page.GetPageSize().GetLeft() + (page.GetPageSize().GetRight()) / 2) + 26.5f;

            // Definindo a posição y para o início e o final da linha vertical
            startY = page.GetPageSize().GetTop() - document.GetTopMargin() - 20;
            endY = page.GetPageSize().GetBottom() + document.GetBottomMargin() + 9;

            // Desenhando a linha vertical
            canvasLinhaVertical2.MoveTo(xPos, startY)
                               .LineTo(xPos, endY)
                               .Stroke();

            document.Add(table);
            
            if(false)
            {
                /*
                canvasLinhaSuperior = new PdfCanvas(page);
                canvasLinhaSuperior.SetLineWidth(0.5f);

                startX = document.GetLeftMargin() + 158f;
                endX = width - document.GetRightMargin() + 7;
                yPos = page.GetPageSize().GetTop() -80f;

                canvasLinhaSuperior.MoveTo(startX, yPos)
                      .LineTo(endX, yPos)
                      .Stroke();
                */
            }
            
        }

        static void AddContentBoloPote2Colunas(Document document, float width, ConsultaProdutoViewModel produto1, ConsultaProdutoViewModel produto2)
        {
            string caminhoParaArial = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts", "Arial.ttf");
            var fonteArial = iText.Kernel.Font.PdfFontFactory.CreateFont(caminhoParaArial, "Identity-H");

            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 }))
                .UseAllAvailableWidth()
                .SetBorder(new SolidBorder(ColorConstants.WHITE, 1))
                .SetMargin(0).SetMarginTop(-2);

            if(produto1 != null)
            {

                #region COLUNA 1

                if (produto1.AcucarTotalValorDiario == null) produto1.AcucarTotalValorDiario = 0m;
                if (produto1.Gtotvd == null) produto1.Gtotvd = 0m;
                if (produto1.Info1 == null) produto1.Info1 = string.Empty;
                if (produto1.Info2 == null) produto1.Info2 = string.Empty;

                if (produto1.NomeProduto.Length > 34)
                    produto1.NomeProduto = produto1.NomeProduto.Substring(0, 34);

                Paragraph title1 = new Paragraph()
                    .Add(new Text(produto1.NomeProduto).SetBold())
                    .SetTextAlignment(TextAlignment.LEFT).SetMultipliedLeading(0.7f).SetMarginTop(1)
                    .SetFontSize(6f).SetFont(fonteArial);

                Paragraph pesoParagrafo1 = new Paragraph($"Peso Líquido: [    ] {produto1.Peso}")
                    .SetTextAlignment(TextAlignment.LEFT).SetBold().SetCharacterSpacing(0.8f)
                    .SetFontSize(4.5f).SetFont(fonteArial)
                    .SetMarginTop(1);

                var ingredientes1 = new Paragraph(produto1.Receita).SetFontSize(4.2f).SetFont(fonteArial).SetMultipliedLeading(0.8f);
                var alergicos1 = new Paragraph(produto1.Info1).SetFontSize(4f).SetBold().SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(1);
                var fabricacao1 = new Paragraph($"FAB.: {DateTime.Now.ToString("dd/MM/yyyy")}  VAL.: {DateTime.Now.AddDays((int)produto1.DiasValidade).ToString("dd/MM/yyyy")}").SetFontSize(5.5f).SetBold().SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(1).SetTextAlignment(TextAlignment.LEFT);
                var infoNutricional1 = new Paragraph("INFORMAÇÃO NUTRICIONAL").SetFontSize(4f).SetBold().SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(1).SetTextAlignment(TextAlignment.LEFT);
                var porcoesEmbalagem1 = new Paragraph(produto1.PorcaoEmbalagem).SetFontSize(4.2f).SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(0).SetTextAlignment(TextAlignment.LEFT).SetTextAlignment(TextAlignment.LEFT);
                var porcoes1 = new Paragraph(produto1.PorcaoFatia).SetFontSize(4.2f).SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(0).SetTextAlignment(TextAlignment.LEFT).SetTextAlignment(TextAlignment.LEFT).SetTextAlignment(TextAlignment.LEFT);

                Table tableNutricional1 = new Table(UnitValue.CreatePercentArray(new float[] { 52, 16, 16, 16 }))
                   .UseAllAvailableWidth()
                   .SetBorder(Border.NO_BORDER)
                   .SetMarginLeft(-2)
                   .SetMarginRight(-2);

                tableNutricional1.AddCell(new Cell().SetBorderTop(new SolidBorder(1.5f)).SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("")));
                tableNutricional1.AddCell(new Cell().SetBorderTop(new SolidBorder(1.5f)).Add(new Paragraph("100g").SetFontSize(4.5f).SetBold().SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));
                tableNutricional1.AddCell(new Cell().SetBorderTop(new SolidBorder(1.5f)).Add(new Paragraph("60g").SetFontSize(4.5f).SetBold().SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));
                tableNutricional1.AddCell(new Cell().SetBorderTop(new SolidBorder(1.5f)).SetBorderRight(Border.NO_BORDER).Add(new Paragraph("%VD").SetFontSize(4.5f).SetBold().SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));

                tableNutricional1.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Valor Energ. (Kcal)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto1.ValorEnergetico)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto1.PorcaoValorEnergetico)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto1.ValorEnergeticoValorDiario)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));

                tableNutricional1.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Carboidratos (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto1.Carboidratos)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto1.PorcaoCarboidratos)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto1.CarboidratosValorDiario)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));

                tableNutricional1.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Açúcares Totais (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(3f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto1.AcucarTotal)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto1.PorcaoAcucarTotal)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                if(produto1.AcucarTotalValorDiario != null && produto1.AcucarTotalValorDiario != 0)
                    tableNutricional1.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto1.AcucarTotalValorDiario)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                else
                    tableNutricional1.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph("").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

                tableNutricional1.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Açúcares Adicionados(g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(7f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto1.AcucarAdicionado)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto1.PorcaoAcucarAdicionado)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto1.AcucarAdicionadoValorDiario)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));

                tableNutricional1.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Proteínas (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto1.Proteina)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto1.PorcaoProteina)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto1.ProteinaValorDiario)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));

                tableNutricional1.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Gorduras Totais (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto1.GorduraTotal)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto1.PorcaoGorduraTotal)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto1.Gtotvd)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));

                tableNutricional1.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Gorduras Saturadas (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(3f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto1.GorduraSaturada)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto1.PorcaoGorduraSaturada)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto1.GorduraSaturadaValorDiario)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));

                tableNutricional1.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Gorduras Trans (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(3f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto1.GorduraTrans)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto1.PorcaoGorduraTrans)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto1.GorduraTransValorDiario)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));

                tableNutricional1.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Fibra (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto1.FibraAlimentar)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto1.PorcaoFibraAlimentar)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto1.FibraAlimentarValorDiario)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));

                tableNutricional1.AddCell(new Cell().SetBorderBottom(new SolidBorder(1.5f)).SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Sódio (mg)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
                tableNutricional1.AddCell(new Cell().SetBorderBottom(new SolidBorder(1.5f)).Add(new Paragraph(FormatarValor(produto1.Sodio)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().SetBorderBottom(new SolidBorder(1.5f)).Add(new Paragraph(FormatarValor(produto1.PorcaoSodio)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().SetBorderBottom(new SolidBorder(1.5f)).SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto1.SodioValorDiario)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));

                var percentual1 = new Paragraph(produto1.Info2).SetMarginTop(-1).SetFontSize(4.5f);

                tableNutricional1.SetMarginRight(10);

                //COLUNA 1
                table.AddCell(new Cell()
                    .Add(title1)
                    .Add(pesoParagrafo1)
                    .Add(ingredientes1)
                    .Add(alergicos1)
                    .Add(fabricacao1)
                    .Add(infoNutricional1)
                    .Add(porcoesEmbalagem1)
                    .Add(porcoes1)
                    .Add(tableNutricional1)
                    .Add(percentual1));

                if (produto1.TipoProduto.FlagAltoAcucar == 1)
                {
                    string imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "alto_acucar_horizontal.png");
                    Image image = new Image(ImageDataFactory.Create(imagePath));
                    image.SetWidth(MillimetersToPoints(15.0f));
                    image.SetHeight(MillimetersToPoints(5.0f));
                    float imageX = 95;
                    float imageY = 88;
                    image.SetFixedPosition(imageX, imageY);

                    document.Add(image);
                }
                else if (produto1.TipoProduto.FlagAltoGorduraSaturada == 1)
                {
                    string imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "alto_gordura_horizontal.png");
                    Image image = new Image(ImageDataFactory.Create(imagePath));
                    image.SetWidth(MillimetersToPoints(15.0f));
                    image.SetHeight(MillimetersToPoints(5.0f));
                    float imageX = 95;
                    float imageY = 88;
                    image.SetFixedPosition(imageX, imageY);

                    document.Add(image);
                }
                else if (produto1.TipoProduto.FlagAltoAcucar == 1 && produto1.TipoProduto.FlagAltoGorduraSaturada == 1)
                {
                    string imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "alto_acucar_gordura_horizontal.png");
                    Image image = new Image(ImageDataFactory.Create(imagePath));
                    image.SetWidth(MillimetersToPoints(20.0f));
                    image.SetHeight(MillimetersToPoints(5.0f));
                    float imageX = 85;
                    float imageY = 88;
                    image.SetFixedPosition(imageX, imageY);

                    document.Add(image);
                }

                #endregion

            }

            if (produto2 != null)
            {

                #region COLUNA 2

                if (produto2.AcucarTotalValorDiario == null) produto2.AcucarTotalValorDiario = 0m;
                if (produto2.Gtotvd == null) produto2.Gtotvd = 0m;
                if (produto2.Info1 == null) produto2.Info1 = string.Empty;
                if (produto2.Info2 == null) produto2.Info2 = string.Empty;

                if (produto1.NomeProduto.Length > 34)
                    produto1.NomeProduto = produto1.NomeProduto.Substring(0, 34);

                Paragraph title2 = new Paragraph()
                    .Add(new Text(produto2.NomeProduto).SetBold())
                    .SetTextAlignment(TextAlignment.LEFT).SetMultipliedLeading(0.7f).SetMarginTop(1)
                    .SetFontSize(6f).SetFont(fonteArial);

                Paragraph pesoParagrafo2 = new Paragraph($"Peso Líquido: [    ] {produto1.Peso}")
                       .SetTextAlignment(TextAlignment.LEFT).SetBold().SetCharacterSpacing(0.8f)
                       .SetFontSize(4.5f).SetFont(fonteArial)
                       .SetMarginTop(1);

                var ingredientes2 = new Paragraph(produto2.Receita).SetFontSize(4.2f).SetFont(fonteArial).SetMultipliedLeading(0.8f);
                var alergicos2 = new Paragraph(produto2.Info1).SetFontSize(4f).SetBold().SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(1);
                var fabricacao2 = new Paragraph($"FAB.: {DateTime.Now.ToString("dd/MM/yyyy")}  VAL.: {DateTime.Now.AddDays((int)produto2.DiasValidade).ToString("dd/MM/yyyy")}").SetFontSize(5.5f).SetBold().SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(1).SetTextAlignment(TextAlignment.LEFT);
                var infoNutricional2 = new Paragraph("INFORMAÇÃO NUTRICIONAL").SetFontSize(4f).SetBold().SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(1).SetTextAlignment(TextAlignment.LEFT);
                var porcoesEmbalagem2 = new Paragraph(produto2.PorcaoEmbalagem).SetFontSize(4.2f).SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(0).SetTextAlignment(TextAlignment.LEFT);
                var porcoes2 = new Paragraph(produto2.PorcaoFatia).SetFontSize(4.2f).SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(0).SetTextAlignment(TextAlignment.LEFT);

                Table tableNutricional2 = new Table(UnitValue.CreatePercentArray(new float[] { 52, 16, 16, 16 }))
                   .UseAllAvailableWidth()
                   .SetBorder(Border.NO_BORDER)
                   .SetMarginLeft(1)
                   .SetMarginRight(-2);

                tableNutricional2.AddCell(new Cell().SetBorderTop(new SolidBorder(1.5f)).SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("")));
                tableNutricional2.AddCell(new Cell().SetBorderTop(new SolidBorder(1.5f)).Add(new Paragraph("100g").SetFontSize(4.5f).SetBold().SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));
                tableNutricional2.AddCell(new Cell().SetBorderTop(new SolidBorder(1.5f)).Add(new Paragraph("60g").SetFontSize(4.5f).SetBold().SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));
                tableNutricional2.AddCell(new Cell().SetBorderTop(new SolidBorder(1.5f)).Add(new Paragraph("%VD").SetFontSize(4.5f).SetBold().SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));

                tableNutricional2.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Valor Energ. (Kcal)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto2.ValorEnergetico)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto2.PorcaoValorEnergetico)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto2.ValorEnergeticoValorDiario)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));

                tableNutricional2.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Carboidratos (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto2.Carboidratos)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto2.PorcaoCarboidratos)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto2.CarboidratosValorDiario)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));

                tableNutricional2.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Açúcares Totais (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(3f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto2.AcucarTotal)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto2.PorcaoAcucarTotal)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                if(produto2.AcucarTotalValorDiario != null && produto2.AcucarTotalValorDiario != 0)
                    tableNutricional2.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto2.AcucarTotalValorDiario)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                else
                    tableNutricional2.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph("").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

                tableNutricional2.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Açúcares Adicionados(g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(7f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto2.AcucarAdicionado)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto2.PorcaoAcucarAdicionado)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto2.AcucarAdicionadoValorDiario)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));

                tableNutricional2.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Proteínas (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto2.Proteina)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto2.PorcaoProteina)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto2.ProteinaValorDiario)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));

                tableNutricional2.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Gorduras Totais (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto2.GorduraTotal)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto2.PorcaoGorduraTotal)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto2.Gtotvd)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));

                tableNutricional2.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Gorduras Saturadas (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(3f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto2.GorduraSaturada)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto2.PorcaoGorduraSaturada)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto2.GorduraSaturadaValorDiario)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));

                tableNutricional2.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Gorduras Trans (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(3f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto2.GorduraTrans)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto2.PorcaoGorduraTrans)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto2.GorduraTransValorDiario)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));

                tableNutricional2.AddCell(new Cell().SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Fibra (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto2.FibraAlimentar)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(FormatarValor(produto2.PorcaoFibraAlimentar)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto2.FibraAlimentarValorDiario)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));

                tableNutricional2.AddCell(new Cell().SetBorderBottom(new SolidBorder(1.5f)).SetBorderLeft(Border.NO_BORDER).Add(new Paragraph("Sódio (mg)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
                tableNutricional2.AddCell(new Cell().SetBorderBottom(new SolidBorder(1.5f)).Add(new Paragraph(FormatarValor(produto2.Sodio)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().SetBorderBottom(new SolidBorder(1.5f)).Add(new Paragraph(FormatarValor(produto2.PorcaoSodio)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().SetBorderBottom(new SolidBorder(1.5f)).SetBorderRight(Border.NO_BORDER).Add(new Paragraph(FormatarValorSemArredondar(produto2.SodioValorDiario)).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.CENTER).SetMarginLeft(1.4f).SetBold()));

                var percentual2 = new Paragraph(produto2.Info2).SetMarginTop(-1).SetFontSize(4.5f);

                tableNutricional2.SetMarginRight(10);

                table.AddCell(new Cell()
                    .Add(title2)
                    .Add(pesoParagrafo2)
                    .Add(ingredientes2)
                    .Add(alergicos2)
                    .Add(fabricacao2)
                    .Add(infoNutricional2)
                    .Add(porcoesEmbalagem2)
                    .Add(porcoes2)
                    .Add(tableNutricional2)
                    .Add(percentual2));

                if (produto2.TipoProduto.FlagAltoAcucar == 1)
                {
                    string imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "alto_acucar_horizontal.png");
                    Image image = new Image(ImageDataFactory.Create(imagePath));
                    image.SetWidth(MillimetersToPoints(15.0f));
                    image.SetHeight(MillimetersToPoints(5.0f));
                    float imageX = 232;
                    float imageY = 88;
                    image.SetFixedPosition(imageX, imageY);

                    document.Add(image);
                }
                else if (produto2.TipoProduto.FlagAltoGorduraSaturada == 1)
                {
                    string imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "alto_acucar_gordura_horizontal.png");
                    Image image = new Image(ImageDataFactory.Create(imagePath));
                    image.SetWidth(MillimetersToPoints(20.0f));
                    image.SetHeight(MillimetersToPoints(5.0f));
                    float imageX = 220;
                    float imageY = 88;
                    image.SetFixedPosition(imageX, imageY);

                    document.Add(image);
                }

                #endregion

            }

            document.Add(table);

            if (produto1 != null)
            {
                Paragraph verticalParagraph1 = new Paragraph($"Tel.: (69) 99292-1726")
                 .SetRotationAngle(Math.PI / 2) // Rotação de 90 graus (em radianos)
                 .SetBold()
                 .SetFontSize(7f);

                // Define as coordenadas x e y do parágrafo
                float xPosition1 = MillimetersToPoints(54f); // Altere conforme necessário
                float yPosition1 = MillimetersToPoints(4); // Altere conforme necessário
                verticalParagraph1.SetFixedPosition(xPosition1, yPosition1, width - xPosition1);

                document.Add(verticalParagraph1);
            }

            if(produto2 != null)
            {
                Paragraph verticalParagraph2 = new Paragraph($"Tel.: (69) 99292-1726")
               .SetRotationAngle(Math.PI / 2) // Rotação de 90 graus (em radianos)
               .SetBold()
               .SetFontSize(7f);

                // Define as coordenadas x e y do parágrafo
                float xPosition2 = MillimetersToPoints(104f); // Altere conforme necessário
                float yPosition2 = MillimetersToPoints(4); // Altere conforme necessário
                verticalParagraph2.SetFixedPosition(xPosition2, yPosition2, 150);

                document.Add(verticalParagraph2);
            }            
        }

        static void AddContentFatia2Colunas(Document document, float width, ConsultaProdutoViewModel produto1, ConsultaProdutoViewModel produto2)
        {
            string caminhoParaArial = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts", "Arial.ttf");
            var fonteArial = iText.Kernel.Font.PdfFontFactory.CreateFont(caminhoParaArial, "Identity-H");

            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 }))
                .UseAllAvailableWidth()
                .SetBorder(new SolidBorder(ColorConstants.WHITE, 1))
                .SetMargin(-4);

            if (produto1 != null)
            {
                #region COLUNA 1

                if (produto1.AcucarTotalValorDiario == null) produto1.AcucarTotalValorDiario = 0m;
                if (produto1.Gtotvd == null) produto1.Gtotvd = 0m;
                if (produto1.Info1 == null) produto1.Info1 = string.Empty;
                if (produto1.Info2 == null) produto1.Info2 = string.Empty;

                Paragraph title = new Paragraph()
                    .Add(new Text(produto1.NomeProduto).SetBold())
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(6).SetFont(fonteArial)
                    .SetMarginTop(4)
                    .SetTextAlignment(TextAlignment.CENTER);

                var ingredientes = new Paragraph(produto1.Receita).SetMarginTop(2).SetFontSize(4f).SetFont(fonteArial).SetMultipliedLeading(1f);
                var infoNutricional = new Paragraph("INFORMAÇÃO NUTRICIONAL: " + produto1.PorcaoFatia).SetFontSize(4f).SetBold().SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(2).SetTextAlignment(TextAlignment.CENTER);

                Table tableNutricional = new Table(UnitValue.CreatePercentArray(new float[] { 40, 35, 25 }))
               .UseAllAvailableWidth()
               .SetBorder(Border.NO_BORDER)
               .SetMarginLeft(-2)
               .SetMarginRight(-2)
               .SetMarginTop(1);

                tableNutricional.AddCell(new Cell().Add(new Paragraph("Valor Energ.").SetFontSize(4.5f).SetMargin(-2.8f).SetMarginLeft(1).SetTextAlignment(TextAlignment.LEFT)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph("183Kcal = 179Kj").SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph("6%").SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));

                tableNutricional.AddCell(new Cell().Add(new Paragraph("Carboidratos").SetFontSize(4.5f).SetMargin(-2.8f).SetMarginLeft(1).SetTextAlignment(TextAlignment.LEFT)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto1.Carboidratos?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto1.PorcaoCarboidratos?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));

                tableNutricional.AddCell(new Cell().Add(new Paragraph("Proteínas").SetFontSize(4.5f).SetMargin(-2.8f).SetMarginLeft(1).SetTextAlignment(TextAlignment.LEFT)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto1.Proteina?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto1.PorcaoProteina?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));

                tableNutricional.AddCell(new Cell().Add(new Paragraph("Gord. Totais").SetFontSize(4.5f).SetMargin(-2.8f).SetMarginLeft(1).SetTextAlignment(TextAlignment.LEFT)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto1.GorduraTotal?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto1.PorcaoGorduraTotal?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));

                tableNutricional.AddCell(new Cell().Add(new Paragraph("Gord. Trans").SetFontSize(4.5f).SetMargin(-2.8f).SetMarginLeft(1).SetTextAlignment(TextAlignment.LEFT)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto1.GorduraTrans?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto1.PorcaoGorduraTrans?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));

                tableNutricional.AddCell(new Cell().Add(new Paragraph("Gord. Saturada").SetFontSize(4.5f).SetMargin(-2.8f).SetMarginLeft(1).SetTextAlignment(TextAlignment.LEFT)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto1.GorduraSaturada?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto1.PorcaoGorduraSaturada?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));

                tableNutricional.AddCell(new Cell().Add(new Paragraph("Fibra Alim").SetFontSize(4.5f).SetMargin(-2.8f).SetMarginLeft(1).SetTextAlignment(TextAlignment.LEFT)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto1.FibraAlimentar?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto1.PorcaoFibraAlimentar?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));

                tableNutricional.AddCell(new Cell().Add(new Paragraph("Sódio").SetFontSize(4.5f).SetMargin(-2.8f).SetMarginLeft(1).SetTextAlignment(TextAlignment.LEFT)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto1.Sodio?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto1.PorcaoSodio?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));

                tableNutricional.SetMarginLeft(10);

                var fabricacao = new Paragraph($"  FAB.: {DateTime.Now.ToString("dd/MM/yy")}  VAL.: {DateTime.Now.AddDays((int) produto1.DiasValidade).ToString("dd/MM/yy")}").SetFontSize(8f).SetBold().SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(4).SetTextAlignment(TextAlignment.CENTER);
                var alergicos = new Paragraph(produto1.Info1).SetFontSize(4f).SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(3).SetTextAlignment(TextAlignment.CENTER);

                //COLUNA 1
                table.AddCell(new Cell()
                    .Add(title)
                    .Add(ingredientes)
                    .Add(infoNutricional)
                    .Add(tableNutricional)
                    .Add(fabricacao)
                    .Add(alergicos));

                #endregion

            }

            if (produto2 != null)
            {
                #region COLUNA 2

                if (produto2.AcucarTotalValorDiario == null) produto2.AcucarTotalValorDiario = 0m;
                if (produto2.Gtotvd == null) produto2.Gtotvd = 0m;
                if (produto2.Info1 == null) produto2.Info1 = string.Empty;
                if (produto2.Info2 == null) produto2.Info2 = string.Empty;

                Paragraph title = new Paragraph()
                    .Add(new Text(produto2.NomeProduto).SetBold())
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(6).SetFont(fonteArial)
                    .SetMarginTop(4)
                    .SetTextAlignment(TextAlignment.CENTER);

                var ingredientes = new Paragraph(produto2.Receita).SetMarginTop(2).SetFontSize(4f).SetFont(fonteArial).SetMultipliedLeading(1f);
                var infoNutricional = new Paragraph("INFORMAÇÃO NUTRICIONAL: " + produto2.PorcaoFatia).SetFontSize(4f).SetBold().SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(2).SetTextAlignment(TextAlignment.CENTER);

                Table tableNutricional = new Table(UnitValue.CreatePercentArray(new float[] { 40, 35, 25 }))
               .UseAllAvailableWidth()
               .SetBorder(Border.NO_BORDER)
               .SetMarginLeft(-2)
               .SetMarginRight(-2)
               .SetMarginTop(1);

                tableNutricional.AddCell(new Cell().Add(new Paragraph("Valor Energ.").SetFontSize(4.5f).SetMargin(-2.8f).SetMarginLeft(1).SetTextAlignment(TextAlignment.LEFT)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph("183Kcal = 179Kj").SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph("6%").SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));

                tableNutricional.AddCell(new Cell().Add(new Paragraph("Carboidratos").SetFontSize(4.5f).SetMargin(-2.8f).SetMarginLeft(1).SetTextAlignment(TextAlignment.LEFT)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto2.Carboidratos?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto2.PorcaoCarboidratos?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));

                tableNutricional.AddCell(new Cell().Add(new Paragraph("Proteínas").SetFontSize(4.5f).SetMargin(-2.8f).SetMarginLeft(1).SetTextAlignment(TextAlignment.LEFT)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto2.Proteina?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto2.PorcaoProteina?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));

                tableNutricional.AddCell(new Cell().Add(new Paragraph("Gord. Totais").SetFontSize(4.5f).SetMargin(-2.8f).SetMarginLeft(1).SetTextAlignment(TextAlignment.LEFT)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto2.GorduraTotal?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto2.PorcaoGorduraTotal?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));

                tableNutricional.AddCell(new Cell().Add(new Paragraph("Gord. Trans").SetFontSize(4.5f).SetMargin(-2.8f).SetMarginLeft(1).SetTextAlignment(TextAlignment.LEFT)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto2.GorduraTrans?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto2.PorcaoGorduraTrans?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));

                tableNutricional.AddCell(new Cell().Add(new Paragraph("Gord. Saturada").SetFontSize(4.5f).SetMargin(-2.8f).SetMarginLeft(1).SetTextAlignment(TextAlignment.LEFT)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto2.GorduraSaturada?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto2.PorcaoGorduraSaturada?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));

                tableNutricional.AddCell(new Cell().Add(new Paragraph("Fibra Alim").SetFontSize(4.5f).SetMargin(-2.8f).SetMarginLeft(1).SetTextAlignment(TextAlignment.LEFT)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto2.FibraAlimentar?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto2.PorcaoFibraAlimentar?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));

                tableNutricional.AddCell(new Cell().Add(new Paragraph("Sódio").SetFontSize(4.5f).SetMargin(-2.8f).SetMarginLeft(1).SetTextAlignment(TextAlignment.LEFT)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto2.Sodio?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));
                tableNutricional.AddCell(new Cell().Add(new Paragraph(produto2.PorcaoSodio?.ToString()).SetFontSize(5.0f).SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));

                tableNutricional.SetMarginLeft(10);

                var fabricacao = new Paragraph($"  FAB.: {DateTime.Now.ToString("dd/MM/yy")}  VAL.: {DateTime.Now.AddDays((int)produto2.DiasValidade).ToString("dd/MM/yy")}").SetFontSize(8f).SetBold().SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(4).SetTextAlignment(TextAlignment.CENTER);
                var alergicos = new Paragraph(produto2.Info1).SetFontSize(4f).SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(3).SetTextAlignment(TextAlignment.CENTER);

                //COLUNA 1
                table.AddCell(new Cell()
                    .Add(title)
                    .Add(ingredientes)
                    .Add(infoNutricional)
                    .Add(tableNutricional)
                    .Add(fabricacao)
                    .Add(alergicos));

                #endregion

            }

            document.Add(table);

            if (produto1 != null)
            {
                Paragraph verticalParagraph1 = new Paragraph($"Tel.: (69) 99292-1726")
                .SetRotationAngle(Math.PI / 2) // Rotação de 90 graus (em radianos)
                .SetBold()
                .SetFontSize(5.5f);

                // Define as coordenadas x e y do parágrafo
                float xPosition1 = MillimetersToPoints(5.5f); // Altere conforme necessário
                float yPosition1 = MillimetersToPoints(14); // Altere conforme necessário
                verticalParagraph1.SetFixedPosition(xPosition1, yPosition1, width - xPosition1);

                document.Add(verticalParagraph1);
            }

            if (produto2 != null)
            {
                Paragraph verticalParagraph2 = new Paragraph($"Tel.: (69) 99292-1726")
                .SetRotationAngle(Math.PI / 2) // Rotação de 90 graus (em radianos)
                .SetBold()
                .SetFontSize(5.5f);

                // Define as coordenadas x e y do parágrafo
                float xPosition2 = MillimetersToPoints(55f); // Altere conforme necessário
                float yPosition2 = MillimetersToPoints(14); // Altere conforme necessário
                verticalParagraph2.SetFixedPosition(xPosition2, yPosition2, 250 - xPosition2);

                document.Add(verticalParagraph2);
            }
        }


        #endregion

        // Função para converter milímetros para pontos (1 mm = 2.83465 pontos)
        static float MillimetersToPoints(float mm)
        {
            return mm * 2.83465f;
        }

        public static string FormatarValor(decimal? valor)
        {
            if (valor == null)
            {
                return string.Empty;
            }

            decimal valorDecimal = valor.Value;

            // Verifica se o valor é zero
            if (valorDecimal == 0)
            {
                return "0";
            }

            // Valores acima de 10: arredonda se tiver .5 ou mais
            if (valorDecimal >= 10)
            {
                // Arredondamento tradicional (0.5 para cima)
                decimal valorArredondado = Math.Round(valorDecimal);

                // Retorna sem vírgula, apenas números inteiros
                return valorArredondado.ToString("0");
            }
            else
            {
                // Para valores abaixo de 10, não arredonda, mantém com vírgula
                return valorDecimal.ToString("0.0");
            }
        }


        public static string FormatarValorSemArredondar(decimal? valor)
        {
            if (valor == null)
            {
                return string.Empty;
            }

            decimal valorDecimal = valor.Value;

            // Verifica se o valor é zero
            if (valorDecimal == 0)
            {
                return "0";
            }

            // Remove a parte decimal, mantendo apenas a parte inteira
            decimal valorSemDecimal = Math.Floor(valorDecimal);

            // Retorna o valor sem casas decimais
            return valorSemDecimal.ToString("0");
        }


    }

    public enum TipoLayoutEtiquetaOld2
    {
        Bolo_1Coluna = 1,
        BoloPote_2Colunas = 2,
        Fatia_2Colunas
    }
}
