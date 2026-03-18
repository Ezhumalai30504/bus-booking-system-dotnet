using BusBooking_MVC.Models;
using BusBooking_MVC.Repositorys.Interfaces;
using Newtonsoft.Json;
using System.Text;

namespace BusBooking_MVC.Repositorys.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly HttpClient _client;

        public AuthRepository()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://localhost:5191/api/"); 
        }

        public async Task<bool> RegisterAsync(Register model)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("Auth/register", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> VerifyOtpAsync(VerifyOtp model)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("Auth/verify-otp", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<LoginResponsive> LoginAsync(Login model)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("Auth/login", content);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LoginResponsive>(data);
        }
    }
}
