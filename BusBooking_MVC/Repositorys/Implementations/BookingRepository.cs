using BusBooking_MVC.Models.Booking;
using BusBooking_MVC.Repositorys.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BusBooking_MVC.Repositorys.Implementations
{
    public class BookingRepository : IBookRepository
    {
        private readonly HttpClient _client;

        public BookingRepository()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5191/api/")
            };
        }

        

        public async Task<List<int>> GetAvailableSeatsAsync(int busId, DateTime date, string token)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var url = $"Booking/available-seats/{busId}/{date:yyyy-MM-dd}";
            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<int>();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<int>>(json,
                       new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? new List<int>();
        }

        public async Task<BookingResponse?> BookAsync(BookingRequest dto, string token)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var payload = JsonSerializer.Serialize(dto);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("Booking/book", content);
            var body = await response.Content.ReadAsStringAsync();

            // ✅ API returns JSON in both success + failure
            var result = JsonSerializer.Deserialize<BookingResponse>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result;
        }

        // ✅ USER cancel own booking
        public async Task<string> CancelByUserAsync(int bookingId, string token)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PostAsync($"Booking/cancel/{bookingId}", null);
            return await response.Content.ReadAsStringAsync();
        }

        // ✅ USER bookings list
        public async Task<List<BookingReadVm>> MyBookingsAsync(string token)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("Booking/my");
            if (!response.IsSuccessStatusCode)
                return new List<BookingReadVm>();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<BookingReadVm>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<BookingReadVm>();
        }

        // ✅ ADMIN bookings list
        public async Task<List<BookingReadVm>> AdminAllBookingsAsync(string token)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("Booking/admin/all");
            if (!response.IsSuccessStatusCode)
                return new List<BookingReadVm>();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<BookingReadVm>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<BookingReadVm>();
        }

        // ✅ ADMIN cancel any booking
        public async Task<string> CancelByAdminAsync(int bookingId, string token)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PostAsync($"Booking/admin/cancel/{bookingId}", null);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
