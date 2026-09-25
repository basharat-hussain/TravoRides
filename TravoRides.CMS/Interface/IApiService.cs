using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Authentication;
using TravoRides.CMS.Models;

namespace TravoRides.CMS.Interface
{
    public interface IApiService
    {
        Task<T> GetAllAsync<T>(string url);

        Task<T> GetAsync<T>(string url);

        Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest obj);
        Task<T> PostAsync<T>(string url, HttpContent content);

        Task<TResponse> PutAsync<TRequest,TResponse>(string url,  TRequest obj);
        Task<T> PutAsync<T>(string url, T model);
        Task<T> PutAsync<T>(string url, HttpContent content);

        Task<bool> DeleteAsync(string url);

        Task<ApiResponse<LoginResponse?>> LoginAsync(LoginModel model);

        Task<RefreshTokenResponse?> RefreshTokenAsync();
        Task<bool> LogoutAsync();
    }
}
