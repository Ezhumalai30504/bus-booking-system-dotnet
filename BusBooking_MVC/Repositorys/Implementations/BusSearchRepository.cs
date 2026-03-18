using BusBooking_MVC.Models;
using BusBooking_MVC.Repositorys.Interfaces;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BusBooking_MVC.Repositorys.Implementations
{
    public class BusSearchRepository : IBusSearchRepository
    {
        private readonly HttpClient _client;

        public BusSearchRepository()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://localhost:5191/api/");
        }

        public async Task<List<BusSearchResult>> SearchBusesAsync(string from, string to, DateTime date, string token)
        {
            var url = $"Bus/search?from={Uri.EscapeDataString(from)}" +
                      $"&to={Uri.EscapeDataString(to)}" +
                      $"&date={date:yyyy-MM-dd}";

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<BusSearchResult>();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<BusSearchResult>>(json,
                       new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? new List<BusSearchResult>();
        }
    }
}
