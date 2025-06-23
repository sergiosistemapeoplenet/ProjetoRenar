using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoRenar.Presentation.Mvc.Utils
{
    public class RemoverAcentuacao
    {
        public static string Exec(string text)
        {
            return new string(text
                    .Normalize(NormalizationForm.FormD)
                    .Where(ch => char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                    .ToArray());
        }
    }
}
