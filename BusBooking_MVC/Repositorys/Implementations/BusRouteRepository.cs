using BusBooking_MVC.Models;
using BusBooking_MVC.Repositorys.Interfaces;
using Newtonsoft.Json;
using System.Text;

namespace BusBooking_MVC.Repositorys.Implementations
{
    public class BusRouteRepository : IBusRouteRepository
    {
        private readonly HttpClient _client;

        public BusRouteRepository()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://localhost:5191/api/"); // your API port
        }

        private void AddToken(string token)
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<List<BusRoute>> GetAllAsync(string token)
        {
            AddToken(token);

            var response = await _client.GetAsync("BusRoute/ReadRoute");

            if (!response.IsSuccessStatusCode)
                return new List<BusRoute>();

            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<BusRoute>>(data);
        }

        public async Task<BusRoute> GetByIdAsync(int id, string token)
        {
            AddToken(token);

            var response = await _client.GetAsync($"BusRoute/GetById/{id}");
            var data = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<BusRoute>(data);
        }

        public async Task<bool> CreateAsync(BusRoute model, string token)
        {
            AddToken(token);

            var json = JsonConvert.SerializeObject(new
            {
                fromCity = model.FromCity,
                toCity = model.ToCity
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("BusRoute/CreateRoute", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(BusRoute model, string token)
        {
            AddToken(token);

            var json = JsonConvert.SerializeObject(new
            {
                fromCity = model.FromCity,
                toCity = model.ToCity
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"BusRoute/UpdateRoute/{model.Id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id, string token)
        {
            AddToken(token);

            var response = await _client.DeleteAsync($"BusRoute/DeleteRoute/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
