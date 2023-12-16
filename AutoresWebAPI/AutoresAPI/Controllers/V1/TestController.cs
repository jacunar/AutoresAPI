namespace AutoresAPI.Controllers.V1; 
[Route("api/test")]
[ApiController]
public class TestController : ControllerBase {
    [HttpGet(Name = "test")]
    public async Task<string> Get() {
        await Task.CompletedTask;
        return "El Web API está funcionando. Go ahead!";
    }
}