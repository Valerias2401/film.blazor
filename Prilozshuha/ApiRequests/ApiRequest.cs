using Prilozshuha.ApiRequests.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace Prilozshuha.ApiRequests
{
    public class ApiRequestService
    {
        private readonly HttpClient _httpClient;

        public ApiRequestService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UsersResponse> GetAllUsersAsync()
        {
            var url = "api/User/getAllUsers";
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<UsersResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result ?? new UsersResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new UsersResponse();
            }
        }

        public async Task<UserAddResponse> CreateNewUserAndLoginAsync(ReqDataUser user)
        {
            var url = "api/User/createNewUserAndLogin";
            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, user);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<UserAddResponse>(content);
                return result ?? new UserAddResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new UserAddResponse();
            }
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var url = "api/User/login";
            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, request);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<LoginResponse>(content);
                return result ?? new LoginResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new LoginResponse();
            }
        }

        public async Task<UserAddResponse> UpdateUserAsync(UserUpdateRequest user)
        {
            var url = "api/User/updateUser";
            try
            {
                var response = await _httpClient.PutAsJsonAsync(url, user);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<UserAddResponse>(content);
                return result ?? new UserAddResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new UserAddResponse();
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var url = $"api/User/deleteUser/{userId}";
            try
            {
                var response = await _httpClient.DeleteAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }


    }
}