using BusBooking_MVC.Models;

namespace BusBooking_MVC.Repositorys.Interfaces
{
    public interface IBusSearchRepository
    {
        Task<List<BusSearchResult>> SearchBusesAsync(string from, string to, DateTime date, string token);
    }
}
