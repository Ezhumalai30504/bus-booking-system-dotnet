using BusBooking_MVC.Models;
using BusBooking_MVC.Repositorys.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BusBooking_MVC.Repositorys.Implementations
{
    public class BusRepository : IBusRepository
    {
        private readonly HttpClient _client;

        public BusRepository()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://localhost:5191/api/");
        }

        private void AddToken(string token)
        {
            _client.DefaultRequestHeaders.Clear();   // 🔥 ADD THIS LINE

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<List<BusRead>> GetAllAsync(string token)
        {
            AddToken(token);

            var response = await _client.GetAsync("Bus/ReadBus");

            if (!response.IsSuccessStatusCode)
                return new List<BusRead>();

            var data = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<BusRead>>(data,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<BusRead> GetByIdAsync(int id, string token)
        {
            AddToken(token);

            var response = await _client.GetAsync($"Bus/GetById/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            var data = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<BusRead>(data,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<bool> AddAsync(BusCreate dto, string token)
        {
            AddToken(token);

            var content = new StringContent(
                JsonSerializer.Serialize(dto),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync("Bus/Add", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(BusUpdate dto, string token)
        {
            AddToken(token);

            var content = new StringContent(
                JsonSerializer.Serialize(dto),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PutAsync("Bus/update", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id, string token)
        {
            AddToken(token);

            var response = await _client.DeleteAsync($"Bus/Delete/{id}");

            return response.IsSuccessStatusCode;
        }

        public async Task<List<BusRead>> SearchAsync(string from, string to, DateTime date, string token)
        {
            AddToken(token);

            var response = await _client.GetAsync(
                $"Bus/search?from={from}&to={to}&date={date:yyyy-MM-dd}");

            if (!response.IsSuccessStatusCode)
                return new List<BusRead>();

            var data = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<BusRead>>(data,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<List<BusRead>> GetByRouteIdAndDateAsync(int routeId, DateTime travelDate, string token)
        {
            AddToken(token);

            var dateStr = travelDate.ToString("yyyy-MM-dd");
            var url = $"Bus/byRoute/{routeId}?travelDate={Uri.EscapeDataString(dateStr)}";

            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<BusRead>();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<BusRead>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<BusRead>();
        }
    }
}
