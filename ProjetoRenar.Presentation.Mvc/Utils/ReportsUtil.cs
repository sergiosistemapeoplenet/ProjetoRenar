using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoRenar.Presentation.Mvc.Utils
{
    public class ReportsUtil
    {
        public static byte[] GetPdf(string conteudo, bool landscape)
        {
            byte[] pdf = null;

            MemoryStream ms = new MemoryStream();
            TextReader reader = new StringReader(conteudo);

            iTextSharp.text.Document doc = new iTextSharp.text.Document(PageSize.A4, 40, 40, 20, 20);

            if(landscape)
            {
                doc.SetPageSize(PageSize.A4.Rotate());
            }

            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
            HTMLWorker html = new HTMLWorker(doc);

            doc.Open();
            html.StartDocument();
            html.Parse(reader);

            html.EndDocument();
            html.Close();
            doc.Close();

            pdf = ms.ToArray();
            return pdf;
        }
    }
}
