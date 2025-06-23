using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.ViewModels.Usuarios
{
    public class FiltroConsultaUsuarioViewModel
    {
        public string Nome { get; set; }
        public int Ativo { get; set; }

        public int? IdUnidade { get; set; }
        public List<SelectListItem> Unidades { get; set; }

        public int? IdPerfil { get; set; }
        public List<SelectListItem> Perfis{ get; set; }

        public List<ConsultaUsuarioViewModel> Consulta { get; set; } = new List<ConsultaUsuarioViewModel>();
    }
}
