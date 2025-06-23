using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Drawing;
using System.IO;


namespace ProjetoRenar.Presentation.Mvc.Helpers
{
    public static class ITextSharpHelper
    {
        public static byte[] GeneratePdf(string html, byte[] graficoPDF)
        {
            using (MemoryStream outputStream = new MemoryStream())
            {
                using (Document document = new Document())
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, outputStream);
                    document.Open();

                    // Define a fonte a ser usada no PDF
                    BaseFont bfHelvetica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    iTextSharp.text.Font font = new iTextSharp.text.Font(bfHelvetica, 10);

                    // Converte o HTML para o PDF
                    StringReader stringReader = new StringReader(html);
                    HTMLWorker htmlWorker = new HTMLWorker(document);
                    htmlWorker.Parse(stringReader);

                    // Converter o gráfico em uma imagem iTextSharp
                    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(graficoPDF);
                    // Definir a posição da imagem no PDF
                    img.SetAbsolutePosition(50, 360);
                    // Adicionar a imagem ao documento PDF
                    document.Add(img);

                    // Fecha o documento
                    document.Close();

                    // Retorna o conteúdo do PDF como um array de bytes
                    return outputStream.ToArray();
                }
            }
        }

    }
}
