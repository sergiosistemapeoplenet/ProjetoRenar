using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using ProjetoRenar.Application.ViewModels.Produtos;

namespace ProjetoRenar.Presentation.Mvc.Reports
{
    public class EtiquetasReportOld
    {
        public static byte[] ImprimirEtiqueta(TipoLayoutEtiqueta tipo, List<ConsultaProdutoViewModel> produtos)
        {
            switch(tipo)
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
            float width = MillimetersToPoints(100); // Largura em milímetros convertida para pontos
            float height = MillimetersToPoints(50); // Altura em milímetros convertida para pontos

            // Criação de um novo documento PDF com margens zero
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outputFile));
            Document document = new Document(pdfDoc, new PageSize(width, height));
            document.SetMargins(0, 0, 0, 2); // Definindo margens como zero

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
            float width = MillimetersToPoints(100); // Largura em milímetros convertida para pontos
            float height = MillimetersToPoints(50); // Altura em milímetros convertida para pontos

            // Criação de um novo documento PDF com margens zero
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outputFile));
            Document document = new Document(pdfDoc, new PageSize(width, height));
            document.SetMargins(0, 0, 0, 2); // Definindo margens como zero

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
                .SetMarginTop(0)
                .SetFontSize(8).SetFont(fonteArial);

            document.Add(title);

            // Adiciona o texto "Peso Líquido: 1700g" alinhado à direita
            Paragraph pesoParagrafo = new Paragraph("Peso Líquido: " + produto.Peso)
                .SetTextAlignment(TextAlignment.RIGHT).SetBold()
                .SetFontSize(6.5f).SetFont(fonteArial).SetCharacterSpacing(0.8f)
                .SetMarginTop(-10); // Definindo uma margem superior menor para o parágrafo

            document.Add(pesoParagrafo);

            // Adiciona a tabela
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 47, 6, 47 }))
                .UseAllAvailableWidth()
                .SetBorder(Border.NO_BORDER)
                .SetMargin(-4);

            var col1pg1 = new Paragraph("INFORMAÇÃO NUTRICIONAL").SetCharacterSpacing(0.8f).SetFontSize(5f).SetBold().SetMarginTop(-1).SetFont(fonteArial).SetMultipliedLeading(1f); ;
            var col1pg2 = new Paragraph(produto.PorcaoEmbalagem).SetCharacterSpacing(0.8f).SetFontSize(6f).SetBold().SetMarginTop(0).SetFont(fonteArial).SetMultipliedLeading(1f); ;
            var col1pg3 = new Paragraph(produto.PorcaoFatia).SetCharacterSpacing(0.8f).SetFontSize(6f).SetBold().SetMarginTop(0).SetFont(fonteArial).SetMultipliedLeading(1f); ;

            var col3pg1 = new Paragraph(produto.Receita + "\n\n\n\n").SetCharacterSpacing(0.8f).SetFontSize(5.4f).SetFont(fonteArial).SetMultipliedLeading(1f).SetBold();
            var col3pg2 = new Paragraph(produto.Info1 + "\n\n").SetCharacterSpacing(0.8f).SetFontSize(5.4f).SetFont(fonteArial).SetMultipliedLeading(1f).SetBold();
            var col3pg3 = new Paragraph("Fabricado e Distribuído por:").SetCharacterSpacing(0.8f).SetBold().SetFontSize(5.4f).SetFont(fonteArial).SetMultipliedLeading(1f);
            var col3pg4 = new Paragraph("MODESTO ALMEIDA DE BARROS COM.DE BOLOS LTDA - R GERALDO SIQUEIRA, 2711, LOJA 3 - CALADINHO, PORTO VELHO RO CNPJ: 41.128.555/0002-81").SetFontSize(4.5f).SetCharacterSpacing(0.8f).SetFont(fonteArial).SetMultipliedLeading(1f).SetBold();
            var col3pg5 = new Paragraph("Tel: (69) 99292-1726").SetCharacterSpacing(0.8f).SetFontSize(6f).SetTextAlignment(TextAlignment.RIGHT).SetFont(fonteArial).SetMultipliedLeading(1f).SetBold();

            Table tableNutricional = new Table(UnitValue.CreatePercentArray(new float[] { 50, 16, 16, 18 }))
               .UseAllAvailableWidth()
               .SetBorder(Border.NO_BORDER)
               .SetMarginTop(1)
               .SetMarginLeft(-2)
               .SetMarginRight(-2);

            if (produto.AcucarTotalValorDiario == null) produto.AcucarTotalValorDiario = 0m;
            if (produto.Gtotvd == null) produto.Gtotvd = 0m;
            if (produto.Info1 == null) produto.Info1 = string.Empty;
            if (produto.Info2 == null) produto.Info2 = string.Empty;

            tableNutricional.AddCell(new Cell().Add(new Paragraph("")));
            tableNutricional.AddCell(new Cell().Add(new Paragraph("100g").SetFontSize(4.8f).SetCharacterSpacing(0.6f).SetBold().SetMargin(-2).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f)));
            tableNutricional.AddCell(new Cell().Add(new Paragraph("Porção").SetFontSize(4.8f).SetCharacterSpacing(0.6f).SetBold().SetMargin(-2).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f)));
            tableNutricional.AddCell(new Cell().Add(new Paragraph("%VD(*)").SetFontSize(4.8f).SetCharacterSpacing(0.6f).SetBold().SetMargin(-2).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f)));

            tableNutricional.AddCell(new Cell().Add(new Paragraph("Valor Energ. (Kcal)").SetCharacterSpacing(0.8f).SetFontSize(5.2f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.ValorEnergetico?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.PorcaoValorEnergetico?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.ValorEnergeticoValorDiario?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

            tableNutricional.AddCell(new Cell().Add(new Paragraph("Carboidratos (g)").SetCharacterSpacing(0.8f).SetFontSize(5.2f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.Carboidratos?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.PorcaoCarboidratos?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.CarboidratosValorDiario?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

            tableNutricional.AddCell(new Cell().Add(new Paragraph("Açúcares Totais (g)").SetCharacterSpacing(0.8f).SetFontSize(5.2f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(3f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.AcucarTotal?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.PorcaoAcucarTotal?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.AcucarTotalValorDiario?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

            tableNutricional.AddCell(new Cell().Add(new Paragraph("Açúcares Adic. (g)").SetCharacterSpacing(0.8f).SetFontSize(5.2f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(5f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.AcucarAdicionado?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.PorcaoAcucarAdicionado?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.AcucarAdicionadoValorDiario?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

            tableNutricional.AddCell(new Cell().Add(new Paragraph("Proteínas (g)").SetCharacterSpacing(0.8f).SetFontSize(5.2f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.Proteina?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.PorcaoProteina?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.ProteinaValorDiario?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                       
            tableNutricional.AddCell(new Cell().Add(new Paragraph("Gorduras Totais (g)").SetCharacterSpacing(0.8f).SetFontSize(5.2f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.GorduraTotal?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.PorcaoGorduraTotal?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.Gtotvd?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

            tableNutricional.AddCell(new Cell().Add(new Paragraph("Gorduras Satur. (g)").SetCharacterSpacing(0.8f).SetFontSize(5.2f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(3f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.GorduraSaturada?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.PorcaoGorduraSaturada?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.GorduraSaturadaValorDiario?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

            tableNutricional.AddCell(new Cell().Add(new Paragraph("Gorduras Trans (g)").SetCharacterSpacing(0.8f).SetFontSize(5.2f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(5f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.GorduraTrans?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.PorcaoGorduraTrans?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.GorduraTransValorDiario?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                       
            tableNutricional.AddCell(new Cell().Add(new Paragraph("Fibra (g)").SetCharacterSpacing(0.8f).SetFontSize(5.2f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.FibraAlimentar?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.PorcaoFibraAlimentar?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.FibraAlimentarValorDiario?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

            tableNutricional.AddCell(new Cell().Add(new Paragraph("Sódio (mg)").SetCharacterSpacing(0.8f).SetFontSize(5.2f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.Sodio?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.PorcaoSodio?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
            tableNutricional.AddCell(new Cell().Add(new Paragraph(produto.SodioValorDiario?.ToString()).SetCharacterSpacing(0.8f).SetFontSize(5.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

            var percentual = new Paragraph(produto.Info2).SetMarginTop(2).SetFontSize(5f).SetBold().SetCharacterSpacing(0.2f);

            Paragraph verticalParagraph = new Paragraph($" FAB.: {produto.DataAtual.ToString("dd/MM/yy")} VAL.: {produto.DataValidade.ToString("dd/MM/yy")}")
                .SetRotationAngle(Math.PI / 2) // Rotação de 90 graus (em radianos)
                .SetBold().SetCharacterSpacing(0.2f)
                .SetFontSize(8f);

            // Define as coordenadas x e y do parágrafo
            float xPosition = MillimetersToPoints(54f); // Altere conforme necessário
            float yPosition = MillimetersToPoints(4.2f); // Altere conforme necessário
            verticalParagraph.SetFixedPosition(xPosition, yPosition, width - xPosition);

            document.Add(verticalParagraph);

            // Adiciona as células à tabela
            table.AddCell(new Cell().Add(col1pg1).Add(col1pg2).Add(col1pg3).Add(tableNutricional).Add(percentual));
            table.AddCell(new Cell().Add(new Paragraph("")));
            table.AddCell(new Cell().Add(col3pg1).Add(col3pg2).Add(col3pg3).Add(col3pg4).Add(col3pg5));

            document.Add(table);

            /*
            string imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "logo_alto_acucar.png");
            Image image = new Image(ImageDataFactory.Create(imagePath));
            image.SetWidth(MillimetersToPoints(13));
            image.SetHeight(MillimetersToPoints(10));
            float imageX = 236;
            float imageY = 40;
            image.SetFixedPosition(imageX, imageY);

            document.Add(image);
            */
        }

        static void AddContentBoloPote2Colunas(Document document, float width, ConsultaProdutoViewModel produto1, ConsultaProdutoViewModel produto2)
        {
            string caminhoParaArial = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts", "Arial.ttf");
            var fonteArial = iText.Kernel.Font.PdfFontFactory.CreateFont(caminhoParaArial, "Identity-H");

            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 }))
                .UseAllAvailableWidth()
                .SetBorder(new SolidBorder(ColorConstants.WHITE, 1))
                .SetMargin(-2);

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

                Paragraph pesoParagrafo1 = new Paragraph("Peso Líquido: [    ] " + produto1.Peso)
                    .SetTextAlignment(TextAlignment.LEFT).SetBold().SetCharacterSpacing(0.8f)
                    .SetFontSize(4.5f).SetFont(fonteArial)
                    .SetMarginTop(1);

                var ingredientes1 = new Paragraph(produto1.Receita).SetFontSize(4.6f).SetFont(fonteArial).SetMultipliedLeading(0.8f);
                var alergicos1 = new Paragraph(produto1.Info1).SetFontSize(4f).SetBold().SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(1);
                var fabricacao1 = new Paragraph($"FAB.: {DateTime.Now.ToString("dd/MM/yyyy")}  VAL.: {DateTime.Now.AddDays((int)produto1.DiasValidade).ToString("dd/MM/yyyy")}").SetFontSize(5.5f).SetBold().SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(1).SetTextAlignment(TextAlignment.LEFT);
                var infoNutricional1 = new Paragraph("INFORMAÇÃO NUTRICIONAL").SetFontSize(4f).SetBold().SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(1).SetTextAlignment(TextAlignment.LEFT);
                var porcoesEmbalagem1 = new Paragraph(produto1.PorcaoEmbalagem).SetFontSize(4.2f).SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(0).SetTextAlignment(TextAlignment.LEFT);
                var porcoes1 = new Paragraph(produto1.PorcaoFatia).SetFontSize(4.2f).SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(0).SetTextAlignment(TextAlignment.LEFT);

                Table tableNutricional1 = new Table(UnitValue.CreatePercentArray(new float[] { 50, 16, 16, 18 }))
                   .UseAllAvailableWidth()
                   .SetBorder(Border.NO_BORDER)
                   .SetMarginLeft(-2)
                   .SetMarginRight(-2);

                tableNutricional1.AddCell(new Cell().Add(new Paragraph("")));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph("100g").SetFontSize(4.5f).SetBold().SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph("60g").SetFontSize(4.5f).SetBold().SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph("%VD (*)").SetFontSize(4.5f).SetBold().SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));

                tableNutricional1.AddCell(new Cell().Add(new Paragraph("Valor Energ. (Kcal)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.ValorEnergetico?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.PorcaoValorEnergetico?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.ValorEnergeticoValorDiario?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

                tableNutricional1.AddCell(new Cell().Add(new Paragraph("Carboidratos (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.Carboidratos?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.PorcaoCarboidratos?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.CarboidratosValorDiario?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

                tableNutricional1.AddCell(new Cell().Add(new Paragraph("Açúcares Totais (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(3f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.AcucarTotal?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.PorcaoAcucarTotal?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.AcucarTotalValorDiario?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

                tableNutricional1.AddCell(new Cell().Add(new Paragraph("Açúcares Adicionados (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(5f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.AcucarAdicionado?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.PorcaoAcucarAdicionado?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.AcucarAdicionadoValorDiario?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

                tableNutricional1.AddCell(new Cell().Add(new Paragraph("Proteínas (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.Proteina?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.PorcaoProteina?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.ProteinaValorDiario?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

                tableNutricional1.AddCell(new Cell().Add(new Paragraph("Gorduras Totais (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.GorduraTotal?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.PorcaoGorduraTotal?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.Gtotvd?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

                tableNutricional1.AddCell(new Cell().Add(new Paragraph("Gorduras Saturadas (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(3f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.GorduraSaturada?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.PorcaoGorduraSaturada?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.GorduraSaturadaValorDiario?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

                tableNutricional1.AddCell(new Cell().Add(new Paragraph("Gorduras Trans (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(5f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.GorduraTrans?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.PorcaoGorduraTrans?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.GorduraTransValorDiario?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

                tableNutricional1.AddCell(new Cell().Add(new Paragraph("Fibra (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.FibraAlimentar?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.PorcaoFibraAlimentar?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.FibraAlimentarValorDiario?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

                tableNutricional1.AddCell(new Cell().Add(new Paragraph("Sódio (mg)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.Sodio?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.PorcaoSodio?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional1.AddCell(new Cell().Add(new Paragraph(produto1.SodioValorDiario?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

                var percentual1 = new Paragraph(produto1.Info2).SetMarginTop(1).SetFontSize(4.9f);

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

                /*
                    string imagePath1 = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "logo_alto_acucar.png");
                    Image image1 = new Image(ImageDataFactory.Create(imagePath1));
                    image1.SetWidth(MillimetersToPoints(13));
                    image1.SetHeight(MillimetersToPoints(10));
                    float imageX1 = 100;
                    float imageY1 = 80;
                    image1.SetFixedPosition(imageX1, imageY1);
                    document.Add(image1);
                */

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

                Paragraph pesoParagrafo2 = new Paragraph("Peso Líquido: [    ] " + produto2.Peso)
                       .SetTextAlignment(TextAlignment.LEFT).SetBold().SetCharacterSpacing(0.8f)
                       .SetFontSize(4.5f).SetFont(fonteArial)
                       .SetMarginTop(1);

                var ingredientes2 = new Paragraph(produto2.Receita).SetFontSize(4.6f).SetFont(fonteArial).SetMultipliedLeading(0.8f);
                var alergicos2 = new Paragraph(produto2.Info1).SetFontSize(4f).SetBold().SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(1);
                var fabricacao2 = new Paragraph($"FAB.: {DateTime.Now.ToString("dd/MM/yyyy")}  VAL.: {DateTime.Now.AddDays((int)produto2.DiasValidade).ToString("dd/MM/yyyy")}").SetFontSize(5.5f).SetBold().SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(1).SetTextAlignment(TextAlignment.LEFT);
                var infoNutricional2 = new Paragraph("INFORMAÇÃO NUTRICIONAL").SetFontSize(4f).SetBold().SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(1).SetTextAlignment(TextAlignment.LEFT);
                var porcoesEmbalagem2 = new Paragraph(produto2.PorcaoEmbalagem).SetFontSize(4.2f).SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(0).SetTextAlignment(TextAlignment.LEFT);
                var porcoes2 = new Paragraph(produto2.PorcaoFatia).SetFontSize(4.2f).SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(0).SetTextAlignment(TextAlignment.LEFT);

                Table tableNutricional2 = new Table(UnitValue.CreatePercentArray(new float[] { 50, 16, 16, 18 }))
                   .UseAllAvailableWidth()
                   .SetBorder(Border.NO_BORDER)
                   .SetMarginLeft(-2)
                   .SetMarginRight(-2);

                tableNutricional2.AddCell(new Cell().Add(new Paragraph("")));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph("100g").SetFontSize(4.5f).SetBold().SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph("60g").SetFontSize(4.5f).SetBold().SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph("%VD (*)").SetFontSize(4.5f).SetBold().SetMargin(-2.8f).SetTextAlignment(TextAlignment.CENTER)));

                tableNutricional2.AddCell(new Cell().Add(new Paragraph("Valor Energ. (Kcal)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.ValorEnergetico?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.PorcaoValorEnergetico?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.ValorEnergeticoValorDiario?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

                tableNutricional2.AddCell(new Cell().Add(new Paragraph("Carboidratos (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.Carboidratos?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.PorcaoCarboidratos?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.CarboidratosValorDiario?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

                tableNutricional2.AddCell(new Cell().Add(new Paragraph("Açúcares Totais (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(3f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.AcucarTotal?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.PorcaoAcucarTotal?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.AcucarTotalValorDiario?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

                tableNutricional2.AddCell(new Cell().Add(new Paragraph("Açúcares Adicionados (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(5f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.AcucarAdicionado?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.PorcaoAcucarAdicionado?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.AcucarAdicionadoValorDiario?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

                tableNutricional2.AddCell(new Cell().Add(new Paragraph("Proteínas (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.Proteina?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.PorcaoProteina?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.ProteinaValorDiario?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

                tableNutricional2.AddCell(new Cell().Add(new Paragraph("Gorduras Totais (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.GorduraTotal?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.PorcaoGorduraTotal?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.Gtotvd?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

                tableNutricional2.AddCell(new Cell().Add(new Paragraph("Gorduras Saturadas (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(3f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.GorduraSaturada?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.PorcaoGorduraSaturada?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.GorduraSaturadaValorDiario?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

                tableNutricional2.AddCell(new Cell().Add(new Paragraph("Gorduras Trans (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(5f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.GorduraTrans?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.PorcaoGorduraTrans?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.GorduraTransValorDiario?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

                tableNutricional2.AddCell(new Cell().Add(new Paragraph("Fibra (g)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.FibraAlimentar?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.PorcaoFibraAlimentar?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.FibraAlimentarValorDiario?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

                tableNutricional2.AddCell(new Cell().Add(new Paragraph("Sódio (mg)").SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.Sodio?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.PorcaoSodio?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));
                tableNutricional2.AddCell(new Cell().Add(new Paragraph(produto2.SodioValorDiario?.ToString()).SetFontSize(4.8f).SetMargin(-2.5f).SetTextAlignment(TextAlignment.LEFT).SetMarginLeft(1.4f).SetBold()));

                var percentual2 = new Paragraph(produto2.Info2).SetMarginTop(1).SetFontSize(4.9f);

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

                /*
                    string imagePath2 = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "logo_alto_acucar.png");
                    Image image2 = new Image(ImageDataFactory.Create(imagePath1));
                    image2.SetWidth(MillimetersToPoints(13));
                    image2.SetHeight(MillimetersToPoints(10));
                    float imageX2 = 240;
                    float imageY2 = 80;
                    image2.SetFixedPosition(imageX2, imageY2);
                    document.Add(image2);
                */

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
                float xPosition1 = MillimetersToPoints(51f); // Altere conforme necessário
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
                float xPosition2 = MillimetersToPoints(101f); // Altere conforme necessário
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
                    .SetFontSize(11f).SetFont(fonteArial)
                    .SetMarginTop(4)
                    .SetTextAlignment(TextAlignment.CENTER);

                var ingredientes = new Paragraph(produto1.Receita).SetMarginTop(2).SetFontSize(4.6f).SetFont(fonteArial).SetMultipliedLeading(1f);
                var infoNutricional = new Paragraph("INFORMAÇÃO NUTRICIONAL: " + produto1.PorcaoFatia).SetFontSize(5f).SetBold().SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(2).SetTextAlignment(TextAlignment.CENTER);

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

                var fabricacao = new Paragraph($"FAB.: {DateTime.Now.ToString("dd/MM/yy")}  VAL.: {DateTime.Now.AddDays((int) produto1.DiasValidade).ToString("dd/MM/yy")}").SetFontSize(9f).SetBold().SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(4).SetTextAlignment(TextAlignment.CENTER);
                var alergicos = new Paragraph(produto1.Info1).SetFontSize(4.5f).SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(3).SetTextAlignment(TextAlignment.CENTER);

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
                    .SetFontSize(11f).SetFont(fonteArial)
                    .SetMarginTop(4)
                    .SetTextAlignment(TextAlignment.CENTER);

                var ingredientes = new Paragraph(produto2.Receita).SetMarginTop(2).SetFontSize(4.6f).SetFont(fonteArial).SetMultipliedLeading(1f);
                var infoNutricional = new Paragraph("INFORMAÇÃO NUTRICIONAL: " + produto2.PorcaoFatia).SetFontSize(5f).SetBold().SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(2).SetTextAlignment(TextAlignment.CENTER);

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

                var fabricacao = new Paragraph($"FAB.: {DateTime.Now.ToString("dd/MM/yy")}  VAL.: {DateTime.Now.AddDays((int)produto2.DiasValidade).ToString("dd/MM/yy")}").SetFontSize(9f).SetBold().SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(4).SetTextAlignment(TextAlignment.CENTER);
                var alergicos = new Paragraph(produto2.Info1).SetFontSize(4.5f).SetFont(fonteArial).SetMultipliedLeading(1f).SetMarginTop(3).SetTextAlignment(TextAlignment.CENTER);

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
    }

    public enum TipoLayoutEtiquetaOld
    {
        Bolo_1Coluna = 1,
        BoloPote_2Colunas = 2,
        Fatia_2Colunas
    }
}
