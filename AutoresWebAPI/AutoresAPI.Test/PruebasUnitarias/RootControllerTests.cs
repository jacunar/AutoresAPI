using AutoresAPI.Controllers.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace AutoresAPI.Test.PruebasUnitarias;

[TestClass]
public class RootControllerTests {
    [TestMethod]
    public async Task SiUsuarioEsAdmin_Obtenemos4Links() {
        // Preparacion
        var authorizationService = new AuthorizationServiceMock();
        authorizationService.Resultado = AuthorizationResult.Success();

        var rootController = new RootController(authorizationService);
        rootController.Url = new UrlHelperMock();

        // Ejecucion
        var resultado = await rootController.Get();

        // Verificacion
        Assert.AreEqual(4, resultado.Value.Count());
    }

    [TestMethod]
    public async Task SiUsuarioNoEsAdmin_Obtenemos2Links() {
        // Preparacion
        var authorizationService = new AuthorizationServiceMock();
        authorizationService.Resultado = AuthorizationResult.Failed();

        var rootController = new RootController(authorizationService);
        rootController.Url = new UrlHelperMock();

        // Ejecucion
        var resultado = await rootController.Get();

        // Verificacion
        Assert.AreEqual(2, resultado.Value.Count());
    }

    [TestMethod]
    public async Task SiUsuarioNoEsAdmin_Obtenemos2Links_UsandoMoq() {
        // Preparacion
        var mockAuthorizationService = new Mock<IAuthorizationService>();
        mockAuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), 
                    It.IsAny<object>(),
                    It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
            .Returns(Task.FromResult(AuthorizationResult.Failed()));

        mockAuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<object>(),
                    It.IsAny<string>()))
            .Returns(Task.FromResult(AuthorizationResult.Failed()));

        var mockURLHelper = new Mock<IUrlHelper>();
        mockURLHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(string.Empty);

        var rootController = new RootController(mockAuthorizationService.Object);
        rootController.Url = mockURLHelper.Object;

        // Ejecucion
        var resultado = await rootController.Get();

        // Verificacion
        Assert.AreEqual(2, resultado.Value.Count());
    }
}