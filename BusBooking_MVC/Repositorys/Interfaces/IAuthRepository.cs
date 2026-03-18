using BusBooking_MVC.Models;

namespace BusBooking_MVC.Repositorys.Interfaces
{
    public interface IAuthRepository
    {
        Task<bool> RegisterAsync(Register model);
        Task<bool> VerifyOtpAsync(VerifyOtp model);
        Task<LoginResponsive> LoginAsync(Login model);
    }
}
