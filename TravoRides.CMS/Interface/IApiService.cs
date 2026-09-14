using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Authentication;
using TravoRides.CMS.Models;

namespace TravoRides.CMS.Interface
{
    public interface IApiService
    {
        Task<T> GetAllAsync<T>(string url);

        Task<T> GetAsync<T>(string url);

        Task<T> PostAsync<T>(string url, object model);

        Task<T> PostAsync<T>(string url, HttpContent content);

        Task<T> PutAsync<T>(string url,  object obj);

        Task<T> PutAsync<T>(string url, HttpContent content);

        Task<bool> DeleteAsync(string url);

        Task<ApiResponse<LoginResponse?>> LoginAsync(LoginModel model);

        Task<LoginResponse?> RefreshTokenAsync();
        Task<bool> LogoutAsync();
    }
}
