using System.ComponentModel.DataAnnotations;

#nullable disable

namespace AutoresAPI.Validaciones; 
public class PrimeraLetraMayusculaAttribute: ValidationAttribute {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
        if (value is null || string.IsNullOrEmpty(value.ToString()))
            return ValidationResult.Success;

        if (value != null) {
            var primeraLetra = value.ToString()[0].ToString();
            if (primeraLetra != primeraLetra.ToUpper())
                return new ValidationResult("La primera letra debe ser mayúscula");

            return ValidationResult.Success;
        }
        return new ValidationResult("No debe ser nulo");
    }
}