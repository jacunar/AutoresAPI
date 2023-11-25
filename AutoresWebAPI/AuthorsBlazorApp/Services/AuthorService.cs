using AuthorsBlazorApp.DTOs;
using Newtonsoft.Json;

namespace AuthorsBlazorApp.Services;

public interface IAuthorService {
    Task<List<AutorDTO>> GetAuthors();
}

public class AuthorService : IAuthorService {
    private readonly HttpClient _httpClient;

    public AuthorService(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    public async Task<List<AutorDTO>> GetAuthors() {
        List<AutorDTO> autors = new List<AutorDTO>();
        var apiName = "api/autores";
        var httpResponse = await _httpClient.GetAsync(apiName);

        if (httpResponse.IsSuccessStatusCode) {
            autors = JsonConvert.DeserializeObject<List<AutorDTO>>(await httpResponse.Content.ReadAsStringAsync()) ?? new();
        }
        return autors;
    }
}