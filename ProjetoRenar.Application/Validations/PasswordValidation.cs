using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProjetoRenar.Application.Validations
{
    public class PasswordValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is string)
            {
                var password = value as string;

                bool hasUpperCaseLetter = false;
                bool hasLowerCaseLetter = false;
                bool hasDecimalDigit = false;

                foreach (char c in password)
                {
                    if (char.IsUpper(c)) hasUpperCaseLetter = true;
                    else if (char.IsLower(c)) hasLowerCaseLetter = true;
                    else if (char.IsDigit(c)) hasDecimalDigit = true;
                }

                return hasUpperCaseLetter && hasLowerCaseLetter && hasDecimalDigit;
            }

            return false;
        }
    }
}
