using System.ComponentModel.DataAnnotations;

namespace AutoresAPI.Test.PruebasUnitarias;

[TestClass]
public class PrimeraLetraMayusculaAttributeTest {
    [TestMethod]
    public void PrimeraLetraMinuscula_DevuelveError() {
        // Preparacion
        var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
        var valor = "josue";
        var valContext = new ValidationContext(new { Nombre = valor });

        // Ejecucion
        var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

        // Verificacion
        Assert.AreEqual("La primera letra debe ser mayúscula", resultado.ErrorMessage);
    }

    [TestMethod]
    public void ValorConPrimeraLetraMayuscula_NoDevuelveError() {
        // Preparacion
        var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
        string valor = "Josué";
        var valContext = new ValidationContext(new { Nombre = valor });

        // Ejecucion
        var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

        // Verificacion
        Assert.IsNull(resultado);
    }

    [TestMethod]
    public void ValorNulo_NoDevuelveError() {
        // Preparacion
        var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
        string valor = null;
        var valContext = new ValidationContext(new { Nombre = valor });

        // Ejecucion
        var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

        // Verificacion
        Assert.IsNull(resultado);
    }
}