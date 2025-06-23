using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProjetoRenar.Application.ViewModels.Usuarios
{
    public class RedefinirSenhaViewModel
    {        
        [Required(ErrorMessage = "Por favor, informe a senha atual.")]
        public string SenhaAtualUsuario { get; set; }

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
            ErrorMessage = "Por favor, informe a senha com letras maiúsculas, letras minúsculas, números, símbolos e pelo menos 8 caracteres.")]
        [Required(ErrorMessage = "Por favor, informe a nova senha.")]
        public string NovaSenhaUsuario { get; set; }

        [Compare("NovaSenhaUsuario", ErrorMessage = "Senhas não conferem, por favor verifique.")]
        [Required(ErrorMessage = "Por favor, confirme a nova senha.")]
        public string NovaSenhaConfirmacao { get; set; }
                        
        public int? IDUsuarioInclusao { get; set; }
        public string EmailInclusao { get; set; }
    }
}
