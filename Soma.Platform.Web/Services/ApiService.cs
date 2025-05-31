using Soma.Platform.Core.DTOs;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Soma.Platform.Web.Services;

public interface IApiService
{
    Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data);
    Task<ApiResponse<T>> GetAsync<T>(string endpoint);
    void SetAuthToken(string token);
    void ClearAuthToken();
}

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        
        var baseUrl = _configuration["ApiService:BaseUrl"] ?? "https://localhost:7073";
        _httpClient.BaseAddress = new Uri(baseUrl);
    }

    public void SetAuthToken(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
    }

    public void ClearAuthToken()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return new ApiResponse<T> { Success = true, Data = result };
            }
            else
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return new ApiResponse<T> 
                { 
                    Success = false, 
                    ErrorMessage = errorResponse?.Message ?? "An error occurred" 
                };
            }
        }
        catch (Exception ex)
        {
            return new ApiResponse<T> 
            { 
                Success = false, 
                ErrorMessage = $"Network error: {ex.Message}" 
            };
        }
    }

    public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return new ApiResponse<T> { Success = true, Data = result };
            }
            else
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return new ApiResponse<T> 
                { 
                    Success = false, 
                    ErrorMessage = errorResponse?.Message ?? "An error occurred" 
                };
            }
        }
        catch (Exception ex)
        {
            return new ApiResponse<T> 
            { 
                Success = false, 
                ErrorMessage = $"Network error: {ex.Message}" 
            };
        }
    }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
}

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
}