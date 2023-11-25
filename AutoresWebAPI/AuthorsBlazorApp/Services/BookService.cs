using AuthorsBlazorApp.DTOs;
using Newtonsoft.Json;
using System.Net.Http;

namespace AuthorsBlazorApp.Services;

public interface IBookService {
    Task<List<LibroDTO>> GetBooks();
}

public class BookService : IBookService {
    private readonly HttpClient _client;

    public BookService(HttpClient client) => _client = client;

    public async Task<List<LibroDTO>> GetBooks() {
        List<LibroDTO> books = new List<LibroDTO>();
        var apiName = "api/libros";
        var httpResponse = await _client.GetAsync(apiName);

        if (httpResponse.IsSuccessStatusCode) {
            books = JsonConvert.DeserializeObject<List<LibroDTO>>(await httpResponse.Content.ReadAsStringAsync()) ?? new();
        }
        return books;
    }
}