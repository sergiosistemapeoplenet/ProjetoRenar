using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoRenar.Presentation.Mvc.Models
{
    public class DashboardDatasViewModel
    {
        [Required(ErrorMessage = "Por favor, informe a data de início.")]
        public DateTime? DataInicio { get; set; }

        [Required(ErrorMessage = "Por favor, informe a data de fim.")]
        public DateTime? DataFim { get; set; }
    }
}
