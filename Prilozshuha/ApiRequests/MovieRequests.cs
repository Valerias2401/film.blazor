using Prilozshuha.ApiRequests.Models;
using Prilozshuha.ApiRequests.Services;
using System.Net.Http.Json;
using System.Text.Json;

namespace Prilozshuha.ApiRequests
{
    public class MovieRequests
    {
        private readonly HttpClient _httpClient;
        private readonly UserService _userService;

        public MovieRequests(HttpClient httpClient, UserService userService)
        {
            _httpClient = httpClient;
            _userService = userService;
        }

        public async Task<GetMoviesResponse> GetAllMoviesAsync()
        {
            var response = await _httpClient.GetAsync("api/Movie");
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<GetMoviesResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new GetMoviesResponse();
        }

        public async Task<GetGenresResponse> GetAllGenresAsync()
        {
            var response = await _httpClient.GetAsync("api/Movie/genres");
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<GetGenresResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new GetGenresResponse();
        }

        public async Task<UserAddResponse> AddMovieAsync(Movie movie)
        {
            if (_userService.CurrentUser != null)
            {
                _httpClient.DefaultRequestHeaders.Remove("X-User-Id");
                _httpClient.DefaultRequestHeaders.Add("X-User-Id", _userService.CurrentUser.id_User.ToString());
            }
            var genresResponse = await GetAllGenresAsync();
            var genre = genresResponse.genres?.FirstOrDefault(g => g.genreName == movie.genre);

            if (genre == null)
            {
                return new UserAddResponse { status = false, message = "Жанр не найден" };
            }
            var movieRequest = new
            {
                Title = movie.title,
                Description = movie.description,
                GenreId = genre.genreId,
                ReleaseYear = movie.releaseYear,
                Rating = movie.rating,
                PosterUrl = movie.posterUrl
            };

            var response = await _httpClient.PostAsJsonAsync("api/Movie", movieRequest);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UserAddResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new UserAddResponse();
        }

        public async Task<UserAddResponse> UpdateMovieAsync(int movieId, Movie movie)
        {
            if (_userService.CurrentUser != null)
            {
                _httpClient.DefaultRequestHeaders.Remove("X-User-Id");
                _httpClient.DefaultRequestHeaders.Add("X-User-Id", _userService.CurrentUser.id_User.ToString());
            }

            var genresResponse = await GetAllGenresAsync();
            var genre = genresResponse.genres?.FirstOrDefault(g => g.genreName == movie.genre);

            if (genre == null)
            {
                return new UserAddResponse { status = false, message = "Жанр не найден" };
            }
            var movieRequest = new
            {
                Title = movie.title,
                Description = movie.description,
                GenreId = genre.genreId,
                ReleaseYear = movie.releaseYear,
                Rating = movie.rating,
                PosterUrl = movie.posterUrl
            };
            var response = await _httpClient.PutAsJsonAsync($"api/Movie/{movieId}", movieRequest);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UserAddResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new UserAddResponse();
        }
        public async Task<UserAddResponse> DeleteMovieAsync(int movieId)
        {
            if (_userService.CurrentUser != null)
            {
                _httpClient.DefaultRequestHeaders.Remove("X-User-Id");
                _httpClient.DefaultRequestHeaders.Add("X-User-Id", _userService.CurrentUser.id_User.ToString());
            }
            var response = await _httpClient.DeleteAsync($"api/Movie/{movieId}");
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UserAddResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new UserAddResponse();
        }
    }
}