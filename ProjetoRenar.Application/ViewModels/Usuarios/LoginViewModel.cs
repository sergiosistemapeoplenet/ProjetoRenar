using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace ProjetoRenar.Application.ViewModels.Usuarios
{
    public class LoginViewModel
    {
        [EmailAddress(ErrorMessage = "Por favor, informe um endereço de email válido.")]
        [Required(ErrorMessage = "Por favor, informe o email.")]
        public string EmailUsuario { get; set; }

        [Required(ErrorMessage = "Por favor, informe a senha.")]
        public string SenhaUsuario { get; set; }

        
    }

    public class AtualizarSenhaViewModel
    {
        [Required(ErrorMessage = "Por favor, informe a nova senha.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Por favor, informe a senha com letras maiúsculas, letras minúsculas, números, símbolos e pelo menos 8 caracteres.")]
        public string NovaSenha { get; set; }

        [Compare("NovaSenha", ErrorMessage = "Senhas não conferem, por favor verifique.")]
        [Required(ErrorMessage = "Por favor, confirme a nova senha.")]
        public string NovaSenhaConfirmacao { get; set; }

        public string Token { get; set; }
        public int IdUsuario { get; set; }
    }

    public class RecuperarSenhaViewModel
    {
        [EmailAddress(ErrorMessage = "Por favor, informe um endereço de email válido.")]
        [Required(ErrorMessage = "Por favor, informe o email.")]
        public string EmailUsuario { get; set; }
    }
}
