namespace AutoresAPI.Controllers.V1; 

[Route("api/v1/test")]
[ApiController]
public class TestController : CustomBaseController {
    [HttpGet(Name = "test")]
    public async Task<string> Get() {
        await Task.CompletedTask;
        return "El Web API está funcionando. Go ahead!";
    }
}