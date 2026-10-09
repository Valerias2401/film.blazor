namespace Prilozshuha.ApiRequests.Models
{
    public class Movie
    {
        public int movieId { get; set; }
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string genre { get; set; } = string.Empty;
        public int releaseYear { get; set; }     
        public double rating { get; set; }        
        public string? posterUrl { get; set; }     
    }

    public class GetMoviesResponse
    {
        public bool status { get; set; }
        public List<Movie>? movies { get; set; }
    }

    public class GetGenresResponse
    {
        public bool status { get; set; }
        public List<Genre>? genres { get; set; }
    }

    public class Genre
    {
        public int genreId { get; set; }
        public string genreName { get; set; } = string.Empty;
    }
}