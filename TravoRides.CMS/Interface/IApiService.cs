using AlArwaSolutions.Application.Common.Responses;
using AlArwaSolutions.Application.DTOs.Authentication;
using AlArwaSolutions.Application.DTOs.Common;
using AlArwaSolutions.CMS.Models;

namespace AlArwaSolutions.CMS.Interface
{
    public interface IApiService
    {
        Task<T> GetAllAsync<T>(string url);

        Task<T> GetAsync<T>(string url);

        Task<T> PostAsync<T>(string url, T model);

        Task<T> PostAsync<T>(string url, HttpContent content);

        Task<T> PutAsync<T>(string url, T model);

        Task<T> PutAsync<T>(string url, HttpContent content);

        Task<bool> DeleteAsync(string url);

        Task<ApiResponse<LoginResponse?>> LoginAsync(LoginModel model);

        Task<LoginResponse?> RefreshTokenAsync();
        Task<bool> LogoutAsync();
    }
}
