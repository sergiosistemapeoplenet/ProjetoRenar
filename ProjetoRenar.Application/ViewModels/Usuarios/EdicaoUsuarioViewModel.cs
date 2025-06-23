using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProjetoRenar.Application.ViewModels.Usuarios
{
    public class EdicaoUsuarioViewModel
    {
        public int? IdUsuario { get; set; }

        [MaxLength(100, ErrorMessage = "Informe no máximo {1} caracteres.")]
        [EmailAddress(ErrorMessage = "Por favor, informe um endereço de email válido.")]
        [Required(ErrorMessage = "Por favor, informe o email.")]
        public string EmailUsuario { get; set; }

        public DateTime? DataUltimoAcesso { get; set; }

        public bool FlagAtivo { get; set; }

        public bool FlagBloqueado { get; set; }

        [Required(ErrorMessage = "Por favor, selecione o perfil.")]
        public int? IDPerfil { get; set; }

        public short? IDUsuarioInclusao { get; set; }

        public DateTime? DataUsuarioInclusao { get; set; }

        public short? IDUsuarioAlteracao { get; set; }

        public DateTime? DataUsuarioAlteracao { get; set; }

        public bool? FlagPrimeiroAcesso { get; set; }

        public string NovaSenha { get; set; }

        [Required(ErrorMessage = "Por favor, selecione a unidade.")]
        public int? IDUnidade { get; set; }

        public List<SelectListItem> Perfis { get; set; }

        public List<SelectListItem> Unidades { get; set; }
    }
}
